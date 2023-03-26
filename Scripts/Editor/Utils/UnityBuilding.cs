using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Types;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Utils.Extensions;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Utils
{
    internal static class UnityBuilding
    {
        private const string TargetKey = "${TARGET}";
        private const string DefaultTargetPath = "Builds/" + TargetKey;

        public static void Build<T>(BuildingGroupSettings<T> @group, bool runTest = true) where T : BuildingTargetSettings
        {
            if (runTest && BuildingSettings.Singleton.RunTests)
            {
                UnityTesting.RunTests(@group, false);
            }
            else
            {
                Build(BuildBehavior.BuildOnly, @group, runTest: false);
            }
        }

        public static void Build<T>(BuildBehavior behavior, BuildingGroupSettings<T> data, bool runTest = true, bool switchTarget = true) where T : BuildingTargetSettings
        {
            var buildingSettings = BuildingSettings.Singleton;
            if (runTest && buildingSettings.RunTests)
            {
                UnityTesting.RunTests(data, behavior, false);
            }
            else
            {
                var oldBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                var oldBuildTarget = EditorUserBuildSettings.activeBuildTarget;
                if (switchTarget)
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(buildingSettings.ToBuildTargetGroup(), buildingSettings.ToBuildTarget());
                }

                try
                {
                    RunBuild(data);
                }
                finally
                {
                    if (switchTarget)
                    {
                        EditorUserBuildSettings.SwitchActiveBuildTarget(oldBuildTargetGroup, oldBuildTarget);
                    }
                }
            }

            void RunBuild(BuildingGroupSettings<T> groupSettings)
            {
                var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildingSettings.ToBuildTarget());
                var cppCompilerConfiguration = buildingSettings.SelectedScriptingBackend == ScriptingBackend.IL2CPP
                    ? CalculateConfiguration(groupSettings.Settings.ScriptingBackend)
                    : (Il2CppCompilerConfiguration?)null;
                if (cppCompilerConfiguration.HasValue)
                {
                    PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.IL2CPP);
                    PlayerSettings.SetIl2CppCompilerConfiguration(buildTargetGroup, cppCompilerConfiguration.Value);
                    PlayerSettings.SetIncrementalIl2CppBuild(buildTargetGroup, groupSettings.Settings.IL2CPPIncrementBuild);
#if UNITY_2021_2_OR_NEWER
                    EditorUserBuildSettings.il2CppCodeGeneration = groupSettings.Settings.IL2CPPCodeGeneration;
#endif
                }
                else
                {
                    PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.Mono2x);
                }

                if (groupSettings is BuildingGroupSettings<BuildingTargetSettingsAndroid> androidSettings)
                {
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;
                    EditorUserBuildSettings.buildAppBundle = androidSettings.Settings.TargetArchive == AndroidTargetArchive.ApplicationBundle;
                }

                var targetPath = DefaultTargetPath.Replace(TargetKey, buildingSettings.SelectedTargetPlatform.ToString()) + "/" + groupSettings.Path;
                var appName = buildingSettings.AppName + GetExtension(buildingSettings.SelectedTargetPlatform);
                var options = new BuildPlayerOptions
                {
                    scenes = KnownScenes,
                    target = buildingSettings.ToBuildTarget(),
                    locationPathName = targetPath + "/" + appName,
                    options = CalculateOptions(buildingSettings.BuildingFlags, behavior, buildingSettings.Clean, buildingSettings.ShowFolder),
                    extraScriptingDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(',')
                        .Concat(groupSettings.Settings.AdditionalScriptingDefineSymbols)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToArray()
                };

                if (buildingSettings.Clean && Directory.Exists(targetPath) && behavior != BuildBehavior.BuildAssetBundleOnly)
                {
                    Debug.Log("[BUILD] Clean output folders...");
                    Directory.Delete(targetPath, true);
                }

                var oldBuildTarget = EditorUserBuildSettings.activeBuildTarget;
                var oldBuildGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                try
                {
                    if (behavior != BuildBehavior.BuildAssetBundleOnly)
                    {
                        Debug.Log("[BUILD] Start build...");
                        var buildReport = BuildPipeline.BuildPlayer(options);
                        if (buildReport.summary.result != BuildResult.Succeeded)
                        {
                            Debug.LogError("[BUILD] Build has failed: " + buildReport.summary.result);
                            EditorUtility.DisplayDialog("Build", "Build has failed", "OK");
                            return;
                        }

                        Debug.Log("[BUILD] Build finished");
                    }

                    if (behavior == BuildBehavior.BuildAssetBundleOnly || buildingSettings.BuildAssetBundles)
                    {
                        Debug.Log("[BUILD] Start Asset Bundle Build...");
                        // try
                        // {
                        //     var filteredItems = AssetBundleSettings.Singleton.Items
                        //         .Where(x => assetData == null ? x.BuildAssetBundle : assetData.BuildStates[x.AssetBundleName])
                        //         .GroupBy(x => x.BuildSubPath)
                        //         .ToDictionary(x => x.Key, x => x.ToList());
                        //     foreach (var item in filteredItems)
                        //     {
                        //         var assetPathDict = item.Value.ToDictionary(
                        //             x => x.AssetBundleName,
                        //             x => AssetDatabase.GetAssetPathsFromAssetBundle(x.AssetBundleName));
                        //
                        //         if (!Directory.Exists(targetPath + "/" + item.Key))
                        //         {
                        //             Directory.CreateDirectory(targetPath + "/" + item.Key);
                        //         }
                        //
                        //         var manifest = BuildPipeline.BuildAssetBundles(targetPath + "/" + item.Key,
                        //             assetPathDict.Select(x => new AssetBundleBuild { assetBundleName = x.Key, assetNames = x.Value }).ToArray(),
                        //             BuildAssetBundleOptions.ForceRebuildAssetBundle, buildingSettings.BuildingData.BuildTarget);
                        //
                        //         if (manifest == null)
                        //         {
                        //             Debug.LogError("[BUILD] Asset Bundle build failed");
                        //             EditorUtility.DisplayDialog("Asset Bundle Build", "Asset Bundle Build failed for " + string.Join(',', item.Value.Select(x => x.AssetBundleName)), "OK");
                        //             return;
                        //         }
                        //     }
                        //
                        //     Debug.Log("[BUILD] Asset Bundles finished");
                        //
                        //     if (behavior == BuildBehavior.BuildAssetBundleOnly && buildingSettings.ShowFolder)
                        //     {
                        //         Application.OpenURL("file:///" + Environment.CurrentDirectory + "/" + targetPath);
                        //     }
                        // }
                        // finally
                        // {
                        //     EditorUtility.ClearProgressBar();
                        // }
                    }
                }
                finally
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(oldBuildGroup, oldBuildTarget);
                }
            }
        }

        private static string[] KnownScenes => EditorBuildSettings.scenes.Select(x => x.path).ToArray();

        private static BuildOptions CalculateOptions(BuildingFlags buildFlag, BuildBehavior behavior, bool clean, bool showFolder)
        {
            var options = BuildOptions.None;
            // if (buildingType.Compress)
            // {
            //     options |= BuildOptions.CompressWithLz4HC;
            // }
            //
            // if (buildingType.AllowDebugging)
            // {
            //     options |= BuildOptions.AllowDebugging;
            // }
            //
            // if (buildingType.DevelopmentBuild)
            // {
            //     options |= BuildOptions.Development;
            // }

            if (buildFlag.HasFlag(BuildingFlags.CodeCoverage))
            {
                options |= BuildOptions.EnableCodeCoverage;
            }

            if (buildFlag.HasFlag(BuildingFlags.StrictMode))
            {
                options |= BuildOptions.StrictMode;
            }

            if (buildFlag.HasFlag(BuildingFlags.UseProfiler))
            {
                options |= BuildOptions.ConnectWithProfiler;
                options |= BuildOptions.EnableDeepProfilingSupport;
            }

            if (showFolder)
            {
                options |= BuildOptions.ShowBuiltPlayer;
            }

            if (buildFlag.HasFlag(BuildingFlags.WaitForConnection))
            {
                options |= BuildOptions.WaitForPlayerConnection;
            }

            if (buildFlag.HasFlag(BuildingFlags.ConnectToHost))
            {
                options |= BuildOptions.ConnectToHost;
            }

            if (buildFlag.HasFlag(BuildingFlags.DetailedReport))
            {
                options |= BuildOptions.DetailedBuildReport;
            }

#if UNITY_2021_2_OR_NEWER
            if (buildFlag.HasFlag(BuildingFlags.SymlinkSources))
            {
                options |= BuildOptions.SymlinkSources;
            }
#endif

            switch (behavior)
            {
                case BuildBehavior.BuildOnly:
                    break;
                case BuildBehavior.BuildAndRun:
                    options |= BuildOptions.AutoRunPlayer;
                    break;
                case BuildBehavior.BuildScriptsOnly:
                    options |= BuildOptions.BuildScriptsOnly;
                    break;
                case BuildBehavior.BuildAssetBundleOnly:
                    break;
                default:
                    throw new NotImplementedException();
            }

#if UNITY_2021_2_OR_NEWER
            if (clean)
            {
                options |= BuildOptions.CleanBuildCache;
            }
#endif

            return options;
        }

        private static Il2CppCompilerConfiguration CalculateConfiguration(IL2CPPBackend backend)
        {
            return backend switch
            {
                IL2CPPBackend.Debug => Il2CppCompilerConfiguration.Debug,
                IL2CPPBackend.Master => Il2CppCompilerConfiguration.Master,
                IL2CPPBackend.Release => Il2CppCompilerConfiguration.Release,
                _ => throw new NotImplementedException()
            };
        }

        private static string GetExtension(TargetPlatform targetPlatform)
        {
            switch (targetPlatform)
            {
                case TargetPlatform.Linux:
                case TargetPlatform.MacOS:
                case TargetPlatform.WebGL:
                case TargetPlatform.IOS:
                    return "";
                case TargetPlatform.Windows:
                    return ".exe";
                case TargetPlatform.Android:
                    return EditorUserBuildSettings.buildAppBundle ? ".aab" : ".apk";

                default:
                    throw new ArgumentOutOfRangeException(nameof(targetPlatform), targetPlatform, null);
            }
        }

        public enum BuildBehavior
        {
            BuildOnly,
            BuildAndRun,
            BuildAssetBundleOnly,
            BuildScriptsOnly,
        }
    }
}
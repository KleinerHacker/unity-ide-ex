using System;
using System.Linq;
using System.Reflection;
using Unity.CodeEditor;
using UnityCommonEx.Runtime.common_ex.Scripts.Runtime.Utils.Extensions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Types;
using UnityToolbarExtender;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Provider
{
    [InitializeOnLoad]
    public static class BuildingToolbar
    {
        private static readonly GenericMenu BuildMenu = new GenericMenu();

        private static readonly BuildingSettings BuildingSettings;
        private static readonly SerializedObject BuildingSerializedObject;

        private static readonly AssetBundleSettings AssetBundleSettings;
        private static readonly SerializedObject AssetBundleSerializedObject;

        private static bool _blockRecompile;
        private static int _targetPlatformIndex = -1;
        private static int _architectureIndex = -1;
        private static int _scriptingBackendIndex = -1;

        static BuildingToolbar()
        {
            BuildingSettings = BuildingSettings.Singleton;
            BuildingSerializedObject = BuildingSettings.SerializedSingleton;

            AssetBundleSettings = AssetBundleSettings.Singleton;
            AssetBundleSerializedObject = AssetBundleSettings.SerializedSingleton;

            ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
            ToolbarExtender.RightToolbarGUI.Insert(0, OnRightToolbarGUI);

            // BuildMenu.AddItem(new GUIContent("Build"), false, () => Build(UnityBuilding.BuildBehavior.BuildOnly));
            // BuildMenu.AddItem(new GUIContent("Build and Run"), false, () => Build(UnityBuilding.BuildBehavior.BuildAndRun));
            // BuildMenu.AddSeparator(null);
            // BuildMenu.AddItem(new GUIContent("Build only all Asset Bundles"), false, () => Build(UnityBuilding.BuildBehavior.BuildAssetBundleOnly));
            // BuildMenu.AddItem(new GUIContent("Build only selected Asset Bundles..."), false, () => SelectAssetBundles(UnityBuilding.BuildBehavior.BuildAssetBundleOnly));
            // BuildMenu.AddSeparator(null);
            // BuildMenu.AddItem(new GUIContent("Run Tests"), false, RunTests);

            CompilationPipeline.compilationStarted += _ => _blockRecompile = true;
            CompilationPipeline.compilationFinished += _ => _blockRecompile = false;
        }

        private static void OnLeftToolbarGUI()
        {
            BuildingSerializedObject.Update();

            GUILayout.FlexibleSpace();

            GUILayout.Space(5f);

            var targets = Enum.GetValues(typeof(TargetPlatform))
                .Cast<TargetPlatform>().ToArray();
            if (_targetPlatformIndex < 0)
            {
                _targetPlatformIndex = targets.IndexOf(x => x == EditorUserBuildSettings.activeBuildTarget.ToTargetPlatform());
            }

            _targetPlatformIndex = EditorGUILayout.Popup(_targetPlatformIndex, targets.Select(x => x.ToString()).ToArray(), ToolbarStyles.popupStyle, ToolbarLayouts.popupSmallLayout);
            var targetFieldInfo = typeof(TargetPlatform).GetField((_targetPlatformIndex < 0 ? TargetPlatform.Windows : targets[_targetPlatformIndex]).ToString());
            var targetSettings = targetFieldInfo.GetCustomAttribute<SupportedSettingsAttribute>();

            var architectures = Enum.GetValues(typeof(TargetArchitecture))
                .Cast<TargetArchitecture>()
                .Where(x => x is not (TargetArchitecture.All or TargetArchitecture.None or TargetArchitecture.ClientAll or TargetArchitecture.ServerAll))
                .Where(x => targetSettings.TargetArchitecture.HasFlag(x))
                .ToArray();
            if (_architectureIndex < 0)
            {
                _architectureIndex = architectures.IndexOf(x => x == EditorUserBuildSettings.activeBuildTarget.ToTargetArchitecture(EditorUserBuildSettings.standaloneBuildSubtarget));
            }

            _architectureIndex = EditorGUILayout.Popup(_architectureIndex, architectures.Select(x => x.ToString()).ToArray(), ToolbarStyles.popupStyle, ToolbarLayouts.popupSmallLayout);

            var backends = Enum.GetValues(typeof(ScriptingBackend))
                .Cast<ScriptingBackend>()
                .Where(x => x is not (ScriptingBackend.All or ScriptingBackend.None))
                .Where(x => targetSettings.ScriptingBackend.HasFlag(x))
                .ToArray();
            if (_scriptingBackendIndex < 0)
            {
                _scriptingBackendIndex = backends.IndexOf(x => x == (PlayerSettings.GetIncrementalIl2CppBuild(EditorUserBuildSettings.selectedBuildTargetGroup) ? ScriptingBackend.IL2CPP : ScriptingBackend.Mono));
            }

            _scriptingBackendIndex = EditorGUILayout.Popup(_scriptingBackendIndex, backends.Select(x => x.ToString()).ToArray(), ToolbarStyles.popupStyle, ToolbarLayouts.popupSmallLayout);

            if (GUILayout.Button(new GUIContent("", (Texture2D)EditorGUIUtility.IconContent("d_Refresh").image, "Reset to active target"), ToolbarStyles.commandButtonStyle))
            {
                _targetPlatformIndex = targets.IndexOf(x => x == EditorUserBuildSettings.activeBuildTarget.ToTargetPlatform());
                _architectureIndex = architectures.IndexOf(x => x == EditorUserBuildSettings.activeBuildTarget.ToTargetArchitecture(EditorUserBuildSettings.standaloneBuildSubtarget));
                _scriptingBackendIndex = backends.IndexOf(x => x == (PlayerSettings.GetIncrementalIl2CppBuild(EditorUserBuildSettings.selectedBuildTargetGroup) ? ScriptingBackend.IL2CPP : ScriptingBackend.Mono));
            }

            GUILayout.Space(5f);

            GUILayout.Label("Flags: ", ToolbarStyles.labelStyle);
            BuildingSettings.BuildingFlags = (BuildingFlags)EditorGUILayout.EnumFlagsField(BuildingSettings.BuildingFlags, ToolbarStyles.popupStyle, ToolbarLayouts.popupSmallLayout);

            GUILayout.Space(5f);

            BuildingSettings.Clean = GUILayout.Toggle(BuildingSettings.Clean, new GUIContent(EditorGUIUtility.IconContent("Grid.EraserTool").image, "Clean complete build cache"), ToolbarStyles.commandButtonStyle);
            BuildingSettings.ShowFolder = GUILayout.Toggle(BuildingSettings.ShowFolder, new GUIContent(EditorGUIUtility.IconContent("d_FolderOpened Icon").image, "Open the build folder"), ToolbarStyles.commandButtonStyle);
            BuildingSettings.BuildAssetBundles = GUILayout.Toggle(BuildingSettings.BuildAssetBundles, new GUIContent(EditorGUIUtility.IconContent("d_Profiler.NetworkOperations").image, "Build Asset Bundles"), ToolbarStyles.commandButtonStyle);
            BuildingSettings.RunTests = GUILayout.Toggle(BuildingSettings.RunTests, new GUIContent(EditorGUIUtility.IconContent("FilterSelectedOnly").image, "Run tests before build starts"), ToolbarStyles.commandButtonStyle);

            GUILayout.Space(5f);

            EditorGUI.BeginDisabledGroup(_blockRecompile);
            if (GUILayout.Button(new GUIContent("", (Texture2D)EditorGUIUtility.IconContent("preAudioLoopOff").image, "Rebuild Scripts"), ToolbarStyles.commandButtonStyle))
            {
                _blockRecompile = true;
                CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.CleanBuildCache);
            }

            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button(new GUIContent("", (Texture2D)EditorGUIUtility.IconContent("d_Settings").image, "Build the project"), ToolbarStyles.commandButtonStyle))
            {
                BuildMenu.ShowAsContext();
            }

            BuildingSerializedObject.ApplyModifiedProperties();
        }

        private static void OnRightToolbarGUI()
        {
            BuildingSerializedObject.Update();

            if (GUILayout.Button(new GUIContent("", (Texture2D)EditorGUIUtility.IconContent("winbtn_win_restore").image, "Open Project"), ToolbarStyles.commandButtonStyle))
            {
                CodeEditor.CurrentEditor.OpenProject();
            }

            BuildingSerializedObject.ApplyModifiedProperties();
        }

        // private static void Build(UnityBuilding.BuildBehavior behavior)
        // {
        //     AssetDatabase.SaveAssets();
        //     UnityBuilding.Build(behavior);
        // }

        private static void RunTests()
        {
            AssetDatabase.SaveAssets();
            //UnityTesting.RunTests(BuildingSettings.BuildingData);
        }

        // private static void SelectAssetBundles(UnityBuilding.BuildBehavior behavior)
        // {
        //     var window = ScriptableObject.CreateInstance<AssetBundleBuildWindow>();
        //     window.ShowModalUtility();
        //     if (!window.Result)
        //         return;
        //
        //     AssetDatabase.SaveAssets();
        //     //UnityBuilding.Build(behavior, assetData: new AssetData { BuildStates = window.BuildStates });
        // }

        private static class ToolbarLayouts
        {
            public static readonly GUILayoutOption[] popupLayout;
            public static readonly GUILayoutOption[] popupSmallLayout;

            static ToolbarLayouts()
            {
                popupLayout = new[]
                {
                    GUILayout.Width(150f)
                };
                popupSmallLayout = new[]
                {
                    GUILayout.Width(100f)
                };
            }
        }

        private static class ToolbarStyles
        {
            public static readonly GUIStyle commandButtonStyle;
            public static readonly GUIStyle popupStyle;
            public static readonly GUIStyle labelStyle;
            public static readonly GUIStyle toggleStyle;

            static ToolbarStyles()
            {
                commandButtonStyle = "AppCommand";

                popupStyle = new GUIStyle("Popup")
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleLeft,
                    imagePosition = ImagePosition.TextOnly,
                    fontStyle = FontStyle.Normal,
                    stretchWidth = false,
                    fixedHeight = 20f,
                    margin = new RectOffset(5, 5, 5, 5)
                };

                labelStyle = new GUIStyle("Label")
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleLeft,
                    imagePosition = ImagePosition.TextOnly,
                    fontStyle = FontStyle.Normal,
                    fixedHeight = 20f,
                    wordWrap = false,
                    margin = new RectOffset(5, 5, 5, 5)
                };

                toggleStyle = new GUIStyle("Toggle")
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleLeft,
                    imagePosition = ImagePosition.TextOnly,
                    fontStyle = FontStyle.Normal,
                    fixedHeight = 20f,
                    wordWrap = false,
                    margin = new RectOffset(5, 5, 5, 5)
                };
            }
        }

        private sealed class EditorToolDelegate : EditorTool
        {
            private Action _action;
            private GUIContent _guiContent = new GUIContent();

            public override GUIContent toolbarIcon => _guiContent;

            public void Setup(Texture2D icon, String tooltip, Action action)
            {
                _guiContent = new GUIContent(icon, tooltip);
                _action = action;
            }

            public override void OnActivated() => _action?.Invoke();
        }
    }
}
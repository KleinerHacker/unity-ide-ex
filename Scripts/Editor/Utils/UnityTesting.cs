using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Types;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Utils.Extensions;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Utils
{
    [InitializeOnLoad]
    internal static class UnityTesting
    {
        private const string FilenameGlobalTestSettings = "global-test.settings";
        private static readonly string FileGlobalTestSettings = Path.GetTempPath() + Path.DirectorySeparatorChar + FilenameGlobalTestSettings;

        private static readonly TestRunnerApi TestRunnerApi;
        private static readonly CallbackHandler<BuildingTargetSettingsWindows> WindowsCallbackHandler = new();
        private static readonly CallbackHandler<BuildingTargetSettingsLinux> LinuxCallbackHandler = new();
        private static readonly CallbackHandler<BuildingTargetSettingsMacOS> MacOSCallbackHandler = new();
        private static readonly CallbackHandler<BuildingTargetSettingsAndroid> AndroidCallbackHandler = new();
        private static readonly CallbackHandler<BuildingTargetSettingsIOS> IOSCallbackHandler = new();
        private static readonly CallbackHandler<BuildingTargetSettingsWebGL> WebGLCallbackHandler = new();

        private static TargetPlatform _currentTargetPlatform;
        private static bool _testOnly;
        private static BuildTargetGroup _targetGroup;
        private static BuildTarget _target;

        static UnityTesting()
        {
            //Load data after reloading assembly
            LoadData();

            WindowsCallbackHandler.LoadData();
            LinuxCallbackHandler.LoadData();
            MacOSCallbackHandler.LoadData();
            AndroidCallbackHandler.LoadData();
            IOSCallbackHandler.LoadData();
            WebGLCallbackHandler.LoadData();

            TestRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            TestRunnerApi.RegisterCallbacks(WindowsCallbackHandler);
            TestRunnerApi.RegisterCallbacks(LinuxCallbackHandler);
            TestRunnerApi.RegisterCallbacks(MacOSCallbackHandler);
            TestRunnerApi.RegisterCallbacks(AndroidCallbackHandler);
            TestRunnerApi.RegisterCallbacks(IOSCallbackHandler);
            TestRunnerApi.RegisterCallbacks(WebGLCallbackHandler);

            EditorApplication.playModeStateChanged += change =>
            {
                if (change == PlayModeStateChange.ExitingPlayMode)
                {
                    EditorUtility.ClearProgressBar();
                }
            };
        }

        private static void InitTest<T>(BuildingGroupSettings<T> groupSettings, UnityBuilding.BuildBehavior? behavior, bool testOnly) where T : BuildingTargetSettings
        {
            _currentTargetPlatform = groupSettings.Settings.Platform;
            _testOnly = testOnly;

            _targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            _target = EditorUserBuildSettings.activeBuildTarget;
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildingSettings.Singleton.ToBuildTargetGroup(), BuildingSettings.Singleton.ToBuildTarget());

            switch (groupSettings.Settings.Platform)
            {
                case TargetPlatform.Windows:
                    WindowsCallbackHandler.Behavior = behavior;
                    WindowsCallbackHandler.GroupSettings = ChangeType<BuildingGroupSettings<BuildingTargetSettingsWindows>>(groupSettings);
                    WindowsCallbackHandler.StoreData();
                    break;
                case TargetPlatform.Linux:
                    LinuxCallbackHandler.Behavior = behavior;
                    LinuxCallbackHandler.GroupSettings = ChangeType<BuildingGroupSettings<BuildingTargetSettingsLinux>>(groupSettings);
                    LinuxCallbackHandler.StoreData();
                    break;
                case TargetPlatform.MacOS:
                    MacOSCallbackHandler.Behavior = behavior;
                    MacOSCallbackHandler.GroupSettings = ChangeType<BuildingGroupSettings<BuildingTargetSettingsMacOS>>(groupSettings);
                    MacOSCallbackHandler.StoreData();
                    break;
                case TargetPlatform.Android:
                    AndroidCallbackHandler.Behavior = behavior;
                    AndroidCallbackHandler.GroupSettings = ChangeType<BuildingGroupSettings<BuildingTargetSettingsAndroid>>(groupSettings);
                    AndroidCallbackHandler.StoreData();
                    break;
                case TargetPlatform.IOS:
                    IOSCallbackHandler.Behavior = behavior;
                    IOSCallbackHandler.GroupSettings = ChangeType<BuildingGroupSettings<BuildingTargetSettingsIOS>>(groupSettings);
                    IOSCallbackHandler.StoreData();
                    break;
                case TargetPlatform.WebGL:
                    WebGLCallbackHandler.Behavior = behavior;
                    WebGLCallbackHandler.GroupSettings = ChangeType<BuildingGroupSettings<BuildingTargetSettingsWebGL>>(groupSettings);
                    WebGLCallbackHandler.StoreData();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Store data as pre-step of reload of assembly
            StoreData();

            TX ChangeType<TX>(object value)
            {
                var convertedValue = (TX)Convert.ChangeType(value, typeof(TX));
                if (convertedValue == null && value != null)
                    throw new InvalidOperationException("Conversion failed from type " + value.GetType().FullName + " to " + typeof(TX).FullName);

                return convertedValue;
            }
        }

        public static void RunTests<T>(BuildingGroupSettings<T> data, bool testOnly) where T : BuildingTargetSettings
        {
            Debug.Log("[TEST-RUN] Start tests");

            InitTest(data, null, testOnly);
            TestRunnerApi.Execute(new ExecutionSettings(new Filter { testMode = TestMode.PlayMode }));
        }

        public static void RunTests<T>(BuildingGroupSettings<T> data, UnityBuilding.BuildBehavior? behavior, bool testOnly) where T : BuildingTargetSettings
        {
            Debug.Log("[TEST-RUN] Start tests");

            InitTest(data, behavior, testOnly);
            TestRunnerApi.Execute(new ExecutionSettings(new Filter { testMode = TestMode.PlayMode }));
        }

        private static void StoreData()
        {
            File.WriteAllLines(
                FileGlobalTestSettings,
                new[]
                {
                    _currentTargetPlatform.ToString(),
                    _testOnly.ToString(),
                    _targetGroup.ToString(),
                    _target.ToString()
                },
                Encoding.UTF8
            );
        }

        private static void LoadData()
        {
            if (!File.Exists(FileGlobalTestSettings))
                return;

            var lines = File.ReadAllLines(FileGlobalTestSettings, Encoding.UTF8);
            if (lines.Length != 4)
                throw new InvalidOperationException("lines count must be 4");

            _currentTargetPlatform = Enum.Parse<TargetPlatform>(lines[0]);
            _testOnly = bool.Parse(lines[1]);
            _targetGroup = Enum.Parse<BuildTargetGroup>(lines[2]);
            _target = Enum.Parse<BuildTarget>(lines[3]);
        }

        private sealed class CallbackHandler<T> : ICallbacks where T : BuildingTargetSettings
        {
            private const string FilenameLocalTestSettings = "local-test.settings";
            private static readonly string FileLocalTestSettings = Path.GetTempPath() + Path.DirectorySeparatorChar + typeof(T).Name + "-" + FilenameLocalTestSettings;

            public BuildingGroupSettings<T> GroupSettings { get; set; }
            public UnityBuilding.BuildBehavior? Behavior { get; set; }

            public void RunStarted(ITestAdaptor testsToRun)
            {
                if (GroupSettings?.Settings.Platform != _currentTargetPlatform)
                    return;

                EditorUtility.DisplayProgressBar("Run Tests", "Test is running now", -1f);
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                if (GroupSettings?.Settings.Platform != _currentTargetPlatform)
                    return;

                EditorUtility.ClearProgressBar();

                Debug.Log("[TEST-RUN] Finished test run with success: " + result.PassCount + ", skipped: " + result.SkipCount + ", failed: " + result.FailCount);
                if (result.TestStatus == TestStatus.Failed)
                {
                    EditorUtility.DisplayDialog("Test failures", "There are test failures: " + result.TestStatus, "OK");
                    EditorUserBuildSettings.SwitchActiveBuildTarget(_targetGroup, _target);
                    return;
                }

                try
                {
                    if (!_testOnly)
                    {
                        if (Behavior.HasValue)
                        {
                            UnityBuilding.Build(Behavior.Value, GroupSettings, false, false);
                        }
                        else
                        {
                            UnityBuilding.Build(GroupSettings, false);
                        }
                    }
                }
                finally
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(_targetGroup, _target);
                }
            }

            public void TestStarted(ITestAdaptor test)
            {
                if (GroupSettings?.Settings.Platform != _currentTargetPlatform)
                    return;

                var index = GetTestIndex(test);
                //EditorUtility.DisplayProgressBar("Run Tests", "Test is running now: " + test.Name + " (" + index + " / " + max + ")", (float) index / (float)max);
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                if (GroupSettings?.Settings.Platform != _currentTargetPlatform)
                    return;

                //Debug.Log("[TEST-RUN] Finished test with success: " + result.PassCount + ", skipped: " + result.SkipCount + ", failed: " + result.FailCount);
                //EditorUtility.ClearProgressBar();
            }

            /// <summary>
            /// Store data as pre-step of reloading assembly
            /// </summary>
            internal void StoreData()
            {
                var json = JsonConvert.SerializeObject(GroupSettings.Settings);
                File.WriteAllLines(
                    FileLocalTestSettings,
                    new[]
                    {
                        Behavior.HasValue ? Behavior.Value.ToString() : "",
                        GroupSettings.Name,
                        GroupSettings.Path,
                        json
                    },
                    Encoding.UTF8
                );
            }

            /// <summary>
            /// Load data after reload assembly
            /// </summary>
            internal void LoadData()
            {
                if (!File.Exists(FileLocalTestSettings))
                    return;

                var lines = File.ReadAllLines(FileLocalTestSettings, Encoding.UTF8);
                if (lines.Length != 4)
                    throw new InvalidOperationException("lines count must be 2");

                Behavior = string.IsNullOrWhiteSpace(lines[0]) ? null : Enum.Parse<UnityBuilding.BuildBehavior>(lines[0]);
                var name = lines[1];
                var path = lines[2];
                var data = lines[3];
                var settings = JsonConvert.DeserializeObject<T>(data);

                GroupSettings = new BuildingGroupSettings<T>
                {
                    Name = name,
                    Path = path,
                    Settings = settings
                };

                File.Delete(FileLocalTestSettings);
            }

            private static int GetTestIndex(ITestAdaptor test)
            {
                var parent = test;
                while (parent.Parent != null)
                {
                    parent = parent.Parent;
                }

                var counter = 0;
                Count(parent, ref counter);

                return counter;

                bool Count(ITestAdaptor root, ref int counter)
                {
                    if (root.UniqueName == test.UniqueName)
                        return true;

                    counter++;
                    foreach (var child in root.Children)
                    {
                        if (Count(child, ref counter))
                            return true;
                    }

                    return false;
                }
            }
        }
    }
}
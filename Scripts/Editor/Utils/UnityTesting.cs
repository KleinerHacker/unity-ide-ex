using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Utils
{
    internal static class UnityTesting
    {
        private static TestRunnerApi _testRunnerApi;

        private static void InitTest<T>(BuildingGroupSettings<T> groupSettings, UnityBuilding.BuildBehavior? behavior) where T : BuildingTargetSettings
        {
            if (_testRunnerApi == null)
            {
                _testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            }

            _testRunnerApi.RegisterCallbacks(new CallbackHandler<T>(groupSettings, behavior));
        }

        public static void RunTests<T>(BuildingGroupSettings<T> data) where T : BuildingTargetSettings
        {
            Debug.Log("Start tests");

            InitTest(data, null);
            _testRunnerApi.Execute(new ExecutionSettings(new Filter { testMode = TestMode.PlayMode }));
        }

        public static void RunTests<T>(BuildingGroupSettings<T> data, UnityBuilding.BuildBehavior? behavior) where T : BuildingTargetSettings
        {
            Debug.Log("Start tests");

            InitTest(data, behavior);
            _testRunnerApi.Execute(new ExecutionSettings(new Filter { testMode = TestMode.PlayMode }));
        }

        private sealed class CallbackHandler<T> : ICallbacks where T : BuildingTargetSettings
        {
            private int max;
            private readonly BuildingGroupSettings<T> groupSettings;
            private readonly UnityBuilding.BuildBehavior? behavior;

            public CallbackHandler(BuildingGroupSettings<T> groupSettings, UnityBuilding.BuildBehavior? behavior)
            {
                this.groupSettings = groupSettings;
                this.behavior = behavior;
            }

            public void RunStarted(ITestAdaptor testsToRun)
            {
                EditorUtility.DisplayProgressBar("Run Tests", "Test is running now", -1f);
                max = GetTestCount(testsToRun);
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                EditorUtility.ClearProgressBar();

                Debug.Log("Finished test with success: " + result.PassCount + ", skipped: " + result.SkipCount + ", failed: " + result.FailCount);
                if (result.TestStatus == TestStatus.Failed)
                {
                    EditorUtility.DisplayDialog("Test failures", "There are test failures: " + result.TestStatus, "OK");
                    return;
                }

                _testRunnerApi.UnregisterCallbacks(this);
                if (behavior.HasValue)
                {
                    //UnityBuilding.Build(behavior.Value, groupSettings, false);   TODO 
                }
                else
                {
                    UnityBuilding.Build(groupSettings, false);
                }
            }

            public void TestStarted(ITestAdaptor test)
            {
                var index = GetTestIndex(test);
                //EditorUtility.DisplayProgressBar("Run Tests", "Test is running now: " + test.Name + " (" + index + " / " + max + ")", (float) index / (float)max);
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                EditorUtility.ClearProgressBar();
            }

            private static int GetTestCount(ITestAdaptor test)
            {
                var parent = test;
                while (parent.Parent != null)
                {
                    parent = parent.Parent;
                }

                return parent.TestCaseCount;
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
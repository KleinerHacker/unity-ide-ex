using UnityEditor;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Utils;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Actions
{
    public static class BuildingAction
    {
        [MenuItem("Build/Build Project &F9", false, 0)]
        public static void BuildProject()
        {
            AssetDatabase.SaveAssets();
            UnityBuilding.Build(UnityBuilding.BuildBehavior.BuildOnly);
        }
        
        [MenuItem("Build/Build && Run Project %&F9", false, 1)]
        public static void BuildAndRunProject()
        {
            AssetDatabase.SaveAssets();
            UnityBuilding.Build(UnityBuilding.BuildBehavior.BuildAndRun);
        }
        
        [MenuItem("Build/Build Scripts Only #F8", false, 2)]
        public static void ScriptsOnlyProject()
        {
            AssetDatabase.SaveAssets();
            UnityBuilding.Build(UnityBuilding.BuildBehavior.BuildScriptsOnly);
        }
    }
}
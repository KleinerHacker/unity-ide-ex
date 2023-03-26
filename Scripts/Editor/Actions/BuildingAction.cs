using UnityEditor;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Utils;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Actions
{
    public static class BuildingAction
    {
        [MenuItem("Build/Build Project &F10", false, 0)]
        public static void BuildProject()
        {
            AssetDatabase.SaveAssets();
            //UnityBuilding.Build(UnityBuilding.BuildBehavior.BuildOnly);
        }

        [MenuItem("Build/Build && Run Project &#F10", false, 1)]
        public static void BuildAndRunProject()
        {
            AssetDatabase.SaveAssets();
            //UnityBuilding.Build(UnityBuilding.BuildBehavior.BuildAndRun);
        }

        [MenuItem("Build/Build Asset Bundles Only", false, 2)]
        public static void BuildAssetBundlesOnly()
        {
            AssetDatabase.SaveAssets();
            //UnityBuilding.Build(UnityBuilding.BuildBehavior.BuildAssetBundleOnly);
        }

        [MenuItem("Build/Build Scripts Only", false, 3)]
        public static void ScriptsOnlyProject()
        {
            AssetDatabase.SaveAssets();
            //UnityBuilding.Build(UnityBuilding.BuildBehavior.BuildScriptsOnly);
        }
    }
}
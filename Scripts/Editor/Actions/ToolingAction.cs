using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets.Misc;
using Debug = UnityEngine.Debug;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Actions
{
    public static class ToolingAction
    {
        [MenuItem("Build/Build Unity Packages", false, 1000)]
        public static void AutoGenerateUnityPackages()
        {
            var folder = EditorUtility.OpenFolderPanel("Auto Generate Unity Packages", null, null);
            if (string.IsNullOrEmpty(folder))
                return;

            try
            {
                var packageAssets = FindAssetsByType<UnityPackageAsset>();
                Debug.Log("Found: " + packageAssets.Count);
                for (var i = 0; i < packageAssets.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("Auto Generate Unity Packages", packageAssets[i].PackageName, (float)i / packageAssets.Count);
                
                    var packageAsset = packageAssets[i];
                    AssetDatabase.ExportPackage(packageAsset.AssetPaths, folder + "/" + packageAsset.PackageName + ".unitypackage", ExportPackageOptions.Recurse);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            Process.Start(folder);
        }
        
        private static List<T> FindAssetsByType<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            return guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .Where(asset => asset != null).ToList();
        }
    }
}
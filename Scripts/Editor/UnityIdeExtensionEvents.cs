using UnityAssetLoader.Runtime.asset_loader.Scripts.Runtime;
using UnityEditor;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor
{
    internal static class UnityIdeExtensionEvents
    {
        [InitializeOnLoadMethod]
        public static void Initlialize()
        {
            AssetResourcesLoader.LoadFromResources<BuildingSettings>("");
            AssetResourcesLoader.LoadFromResources<AssetBundleSettings>("");
        }
    }
}
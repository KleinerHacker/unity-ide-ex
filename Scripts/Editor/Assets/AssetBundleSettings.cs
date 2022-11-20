using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditorEx.Runtime.editor_ex.Scripts.Runtime.Assets;
using UnityEngine;
using UnityEngine.Serialization;
using UnityExtension.Runtime.extension.Scripts.Runtime.Assets;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Utils;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets
{
    internal sealed class AssetBundleSettings : ProviderAsset<AssetBundleSettings>
    {
        #region Static Access

        public static AssetBundleSettings Singleton => GetSingleton("asset-bundles", "asset-bundles.asset", "Editor/Resources");

        public static SerializedObject SerializedSingleton => GetSerializedSingleton("asset-bundles", "asset-bundles.asset", "Editor/Resources");

        #endregion

        #region Inspector Data

        [SerializeField]
        private AssetBundleItem[] items = Array.Empty<AssetBundleItem>();

        #endregion

        #region Properties

        public AssetBundleItem[] Items
        {
            get => items;
            internal set => items = value;
        }

        #endregion
    }

    [Serializable]
    internal sealed class AssetBundleItem
    {
        #region Inspector Data

        [SerializeField]
        private string assetBundleName;

        [SerializeField]
        private BuildAssetBundleOptions options = BuildAssetBundleOptions.ForceRebuildAssetBundle;

        [Space]
        [SerializeField]
        private bool buildAssetBundle;

        [SerializeField]
        private string buildSubPath;

        #endregion

        #region Properties

        public string AssetBundleName
        {
            get => assetBundleName;
            internal set => assetBundleName = value;
        }

        public BuildAssetBundleOptions Options
        {
            get => options;
            internal set => options = value;
        }

        public bool BuildAssetBundle
        {
            get => buildAssetBundle;
            internal set => buildAssetBundle = value;
        }

        public string BuildSubPath
        {
            get => buildSubPath;
            internal set => buildSubPath = value;
        }

        #endregion
    }
}
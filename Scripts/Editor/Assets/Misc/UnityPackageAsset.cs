using UnityEngine;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets.Misc
{
    [CreateAssetMenu(menuName = UnityIdeExtensionConstants.Menu.Asset.MiscMenu + "/Unity Package")]
    public sealed class UnityPackageAsset : ScriptableObject
    {
        #region Inspector Data

        [SerializeField]
        private string packageName;

        [SerializeField]
        private string[] assetPaths;

        #endregion

        #region Propertes

        public string PackageName => packageName;

        public string[] AssetPaths => assetPaths;

        #endregion
    }
}
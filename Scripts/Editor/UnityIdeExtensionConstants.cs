using UnityEngine;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor
{
    internal sealed class UnityIdeExtensionConstants : MonoBehaviour
    {
        private const string Root = "IDE Extensions";
        
        public static class Menu
        {
            public static class Asset
            {
                public const string MiscMenu = Root + "/Misc";
            }
        }
    }
}

using System;
using UnityEditor;
using UnityEngine;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Types;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets
{
    [Serializable]
    public sealed class BuildingTargetSettingsAndroid : BuildingTargetSettingsMobile
    {
        #region Inspector Data

        [SerializeField]
        private AndroidTargetArchive targetArchive = AndroidTargetArchive.ApplicationPackage;

        [SerializeField]
        private AndroidArchitecture targetAndroidArchitecture = AndroidArchitecture.X86_64;

        #endregion

        #region Properties

        public override TargetPlatform Platform => TargetPlatform.Android;

        public AndroidTargetArchive TargetArchive
        {
            get => targetArchive;
            internal set => targetArchive = value;
        }

        public AndroidArchitecture TargetAndroidArchitecture
        {
            get => targetAndroidArchitecture;
            internal set => targetAndroidArchitecture = value;
        }

        #endregion
    }
}
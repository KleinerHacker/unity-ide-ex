using System;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Types;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets
{
    [Serializable]
    public sealed class BuildingTargetSettingsWindows : BuildingTargetSettingsDesktop
    {
        #region Properties

        public override TargetPlatform Platform => TargetPlatform.Windows;

        #endregion
    }
}
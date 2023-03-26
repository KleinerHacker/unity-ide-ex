using System;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Types;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets
{
    [Serializable]
    public sealed class BuildingTargetSettingsWebGL : BuildingTargetSettings
    {
        #region Properties

        public override TargetPlatform Platform => TargetPlatform.WebGL;

        #endregion
    }
}
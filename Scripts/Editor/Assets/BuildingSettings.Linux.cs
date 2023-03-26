using System;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Types;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets
{
    [Serializable]
    public sealed class BuildingTargetSettingsLinux : BuildingTargetSettingsDesktop
    {
        public override TargetPlatform Platform => TargetPlatform.Linux;
    }
}
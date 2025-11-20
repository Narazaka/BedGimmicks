using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.BedGimmicks
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ChangeMirrorProfile : UdonSharpBehaviour
    {
        public MirrorTunerManager[] MirrorTuners;
        public int ProfileIndex;

        public void Change()
        {
            foreach (var mirrorTuner in MirrorTuners)
            {
                if (mirrorTuner != null) mirrorTuner._SetProfile(ProfileIndex);
            }
        }
    }
}

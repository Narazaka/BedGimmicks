using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.BedGimmicks
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ChangeMirrorProfile : UdonSharpBehaviour
    {
        // for FukuroUdon v1 and >=v2 compatibility
        public UdonBehaviour[] MirrorTuners;
        public int ProfileIndex;

        public void Change()
        {
            foreach (var mirrorTuner in MirrorTuners)
            {
                if (mirrorTuner != null) mirrorTuner.SendCustomEvent($"SetProfile{ProfileIndex}");
            }
        }
    }
}

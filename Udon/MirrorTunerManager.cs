using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.BedGimmicks
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MirrorTunerManager : UdonSharpBehaviour
    {
        // for FukuroUdon v1 and >=v2 compatibility
        public UdonBehaviour _mirrorTuner;
        int _profileIndex;

        public void _SetProfile(int index)
        {
            _profileIndex = index;
            if (enabled && gameObject.activeInHierarchy)
            {
                ApplyProfile();
            }
        }

        void OnEnable()
        {
            ApplyProfile();
        }

        void ApplyProfile()
        {
            if (_mirrorTuner != null) _mirrorTuner.SendCustomEvent($"SetProfile{_profileIndex}");
        }
    }
}

using UdonSharp;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.BedGimmicks
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PostProcessingWeight : UdonSharpBehaviour
    {
        public PostProcessVolume PostProcessVolume;

        public float Weight = 1;

        public void OnChangeWeight()
        {
            PostProcessVolume.weight = Weight;
        }
    }
}

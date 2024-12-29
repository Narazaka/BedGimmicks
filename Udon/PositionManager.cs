using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.BedGimmicks
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PositionManager : UdonSharpBehaviour
    {
        public Transform Target;
        public Transform Min;
        public Transform Max;
        public float Rate;

        public Vector3 Value => Vector3.Lerp(Min.localPosition, Max.localPosition, Rate);

        public void OnValueChanged()
        {
            Target.localPosition = Value;
        }
    }
}

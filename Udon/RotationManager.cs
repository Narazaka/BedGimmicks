using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.BedGimmicks
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RotationManager : UdonSharpBehaviour
    {
        public Transform Target;
        public Transform Min;
        public Transform Max;
        public float Rate;

        public Quaternion Value => Quaternion.Slerp(Min.localRotation, Max.localRotation, Rate);

        public void OnValueChanged()
        {
            Target.localRotation = Value;
        }
    }
}

#if FUKUROUDON_V1
using MimyLab;
#else
using MimyLab.FukuroUdon;
#endif
using System;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace Narazaka.VRChat.BedGimmicks
{
    [DisallowMultipleComponent]
    public class BedGimmicks : MonoBehaviour, IEditorOnly
    {
        public Transform Root;

        [Serializable]
        public class ObjectElement
        {
            public Transform Positioner;
            public Transform Sizer;
            public ActiveRelayToGameObject ActiveRelay;
            public string Name => $"{Positioner.parent.name}/{Positioner.name}";
            public bool Use
            {
                get => !Positioner.gameObject.CompareTag("EditorOnly");
                set
                {
                    var tag = value ? "Untagged" : "EditorOnly";
                    if (Positioner.gameObject.CompareTag(tag) && ActiveRelay.gameObject.CompareTag(tag)) return;
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObjects(new UnityEngine.Object[] { Positioner.gameObject, ActiveRelay.gameObject }, $"Change {Name} Use to {value}");
#endif
                    Positioner.gameObject.tag = tag;
                    ActiveRelay.gameObject.tag = tag;
                }
            }
            public bool DefaultActive
            {
                get => Positioner.gameObject.activeSelf;
                set
                {
                    if (Positioner.gameObject.activeSelf == value && ActiveRelay.gameObject.activeSelf == value) return;
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObjects(new UnityEngine.Object[] { Positioner.gameObject, ActiveRelay.gameObject }, $"Change {Name} DefaultActive to {value}");
#endif
                    Positioner.gameObject.SetActive(value);
                    ActiveRelay.gameObject.SetActive(value);
                }
            }
            public void AdjustUse() => Use = Use;
            public void AdjustDefaultActive() => DefaultActive = DefaultActive;
            public void AdjustUseAndDefaultActive() { AdjustUse(); AdjustDefaultActive(); }
        }

        public MirrorsSet Mirrors;

        [Serializable]
        public class MirrorsSet
        {
            public Transform Root;
            public Transform RelayRoot;
            public ObjectElement Top;
            public ObjectElement Head;
            public ObjectElement Left;
            public ObjectElement Right;
            public ObjectElement Foot;
            public ObjectElement Bottom;

            public void AdjustUseAndDefaultActive()
            {
                Top.AdjustUseAndDefaultActive();
                Head.AdjustUseAndDefaultActive();
                Left.AdjustUseAndDefaultActive();
                Right.AdjustUseAndDefaultActive();
                Foot.AdjustUseAndDefaultActive();
                Bottom.AdjustUseAndDefaultActive();
            }
        }

        public TopMirrorManagersSet TopMirrorManagers;

        [Serializable]
        public class TopMirrorManagersSet
        {
            public Transform Root;
            public TransformRange Position;
            public TransformRange Rotation;

            public Slider PositionSlider;
            public Slider RotationSlider;
            public PositionManager PositionManager;
            public RotationManager RotationManager;

            [Serializable]
            public class TransformRange
            {
                public Transform Root;
                public Transform Min;
                public Transform Max;
            }
        }

        public CollidersSet Colliders;

        [Serializable]
        public class CollidersSet
        {
            public Transform Root;
            public Transform RelayRoot;
            public ObjectElement Bottom;

            public void AdjustUseAndDefaultActive()
            {
                Bottom.AdjustUseAndDefaultActive();
            }
        }

        public MirrorButtonsSet MirrorButtons;

        [Serializable]
        public class MirrorButtonsSet
        {
            public Transform Root;
            public Button Top;
            public Button Head;
            public Button Left;
            public Button Right;
            public Button Foot;
            public Button Bottom;
        }
    }
}

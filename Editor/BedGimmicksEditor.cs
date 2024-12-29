using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using YamlDotNet.Core.Tokens;
using System;

namespace Narazaka.VRChat.BedGimmicks.Editor
{
    [CustomEditor(typeof(BedGimmicks))]
    public class BedGimmicksEditor : UnityEditor.Editor
    {
        static Color MirrorSurfaceColor = new Color(1f, 1f, 1f, 0.2f);
        static Color MirrorEdgeColor = new Color(1f, 1f, 1f, 1f);
        static Color DisabledMirrorSurfaceColor = new Color(0f, 0f, 0f, 0.05f);
        static Color DisabledMirrorEdgeColor = new Color(0f, 0f, 0f, 1f);
        static Color ColliderSurfaceColor = new Color(0.2f, 0.9f, 0.4f, 0.05f);
        static Color ColliderEdgeColor = new Color(0.2f, 0.9f, 0.4f, 1f);
        static Color BottomEdgeColor = new Color(0.3f, 1f, 0.6f, 1f);

        BedGimmicks BedGimmicks;
        MirrorBounds Bounds;

        SerializedProperty Root;
        SerializedProperty Mirrors;
        SerializedProperty TopMirrorManagers;
        SerializedProperty Colliders;
        SerializedProperty MirrorButtons;

        bool FoldoutInternal;

        SerializedObject EditorSerializedObject;
        [SerializeField] bool Centered;
        SerializedProperty CenteredProperty;
        [SerializeField] bool FitBottom;
        SerializedProperty FitBottomProperty;

        void OnEnable()
        {
            Root = serializedObject.FindProperty(nameof(BedGimmicks.Root));
            Mirrors = serializedObject.FindProperty(nameof(BedGimmicks.Mirrors));
            TopMirrorManagers = serializedObject.FindProperty(nameof(BedGimmicks.TopMirrorManagers));
            Colliders = serializedObject.FindProperty(nameof(BedGimmicks.Colliders));
            MirrorButtons = serializedObject.FindProperty(nameof(BedGimmicks.MirrorButtons));

            EditorSerializedObject = new SerializedObject(this);
            CenteredProperty = EditorSerializedObject.FindProperty(nameof(Centered));
            FitBottomProperty = EditorSerializedObject.FindProperty(nameof(FitBottom));

            BedGimmicks = target as BedGimmicks;
            BedGimmicks.Mirrors.AdjustUseAndDefaultActive();
            BedGimmicks.Colliders.AdjustUseAndDefaultActive();

            Bounds = new MirrorBounds(BedGimmicks.Root, BedGimmicks.Mirrors, BedGimmicks.Colliders);
            EditorSerializedObject.UpdateIfRequiredOrScript();
            FitBottomProperty.boolValue = Bounds.FitBottom;
            EditorSerializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((BedGimmicks)target), typeof(BedGimmicks), false);
            EditorGUI.EndDisabledGroup();

            CheckVRCPlayersOnlyMirrorGUI();

            GizmoInfoGUI();

            EditorPropertiesGUI();

            Bounds = new MirrorBounds(BedGimmicks.Root, BedGimmicks.Mirrors, BedGimmicks.Colliders);
            Bounds.Centered = Centered;
            Bounds.FitBottom = FitBottom;

            EditorGUILayout.LabelField("Default Active");
            EditorGUI.indentLevel++;
            DefaultActiveGUI(BedGimmicks.Mirrors.Top);
            DefaultActiveGUI(BedGimmicks.Mirrors.Head);
            DefaultActiveGUI(BedGimmicks.Mirrors.Left);
            DefaultActiveGUI(BedGimmicks.Mirrors.Right);
            DefaultActiveGUI(BedGimmicks.Mirrors.Foot);
            DefaultActiveGUI(BedGimmicks.Mirrors.Bottom);
            DefaultActiveGUI(BedGimmicks.Colliders.Bottom);
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Use");
            EditorGUI.indentLevel++;
            UseGUI(BedGimmicks.Mirrors.Top);
            UseGUI(BedGimmicks.Mirrors.Head);
            UseGUI(BedGimmicks.Mirrors.Left);
            UseGUI(BedGimmicks.Mirrors.Right);
            UseGUI(BedGimmicks.Mirrors.Foot);
            UseGUI(BedGimmicks.Mirrors.Bottom);
            UseGUI(BedGimmicks.Colliders.Bottom);
            EditorGUI.indentLevel--;

            InternalPropertiesGUI();
        }

        void CheckVRCPlayersOnlyMirrorGUI()
        {
            if(Shader.Find("Mirror/VRCPlayersOnlyMirror") == null)
            {
                EditorGUILayout.HelpBox("VRCPlayersOnlyMirror shader not found!", MessageType.Error);
                var color = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Import VRCPlayersOnlyMirror from Booth", new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold }, GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
                {
                    Application.OpenURL("https://temporal.booth.pm/items/2685621");
                }
                GUI.backgroundColor = color;
            }
        }

        void GizmoInfoGUI()
        {
            var sceneView = EditorWindow.GetWindow<UnityEditor.SceneView>("Scene", false);
            if (sceneView == null)
            {
                return;
            }
            if (!sceneView.drawGizmos)
            {
                if (GUILayout.Button("Enable Gizmos"))
                {
                    sceneView.drawGizmos = true;
                }
            }
        }

        void DefaultActiveGUI(BedGimmicks.ObjectElement objectElement)
        {
            EditorGUI.BeginChangeCheck();
            var defaultActive = EditorGUILayout.Toggle(objectElement.Name, objectElement.DefaultActive);
            if (EditorGUI.EndChangeCheck())
            {
                objectElement.DefaultActive = defaultActive;
            }
        }

        void UseGUI(BedGimmicks.ObjectElement objectElement)
        {
            EditorGUI.BeginChangeCheck();
            var use = EditorGUILayout.Toggle(objectElement.Name, objectElement.Use);
            if (EditorGUI.EndChangeCheck())
            {
                objectElement.Use = use;
            }
        }

        void EditorPropertiesGUI()
        {
            EditorSerializedObject.UpdateIfRequiredOrScript();
            CenteredProperty.boolValue = EditorGUILayout.Toggle(CenteredProperty.displayName, CenteredProperty.boolValue);
            FitBottomProperty.boolValue = EditorGUILayout.Toggle(FitBottomProperty.displayName, FitBottomProperty.boolValue);
            EditorSerializedObject.ApplyModifiedProperties();
        }

        void InternalPropertiesGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            FoldoutInternal = EditorGUILayout.Foldout(FoldoutInternal, "Internal");
            if (FoldoutInternal)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(Root);
                EditorGUILayout.PropertyField(Mirrors);
                EditorGUILayout.PropertyField(TopMirrorManagers);
                EditorGUILayout.PropertyField(Colliders);
                EditorGUILayout.PropertyField(MirrorButtons);
                EditorGUI.indentLevel--;
            }
            serializedObject.ApplyModifiedProperties();
        }

        void OnSceneGUI()
        {
            Bounds = new MirrorBounds(BedGimmicks.Root, BedGimmicks.Mirrors, BedGimmicks.Colliders);
            Bounds.Centered = Centered;
            Bounds.FitBottom = FitBottom;

            var gizmosColor = Gizmos.color;
            var handlesColor = Handles.color;

            DrawMirrorHandle(BedGimmicks.Mirrors.Top, Bounds.SetTop);
            DrawMirrorHandle(BedGimmicks.Mirrors.Head, Bounds.SetHead);
            DrawMirrorHandle(BedGimmicks.Mirrors.Left, Bounds.SetLeft);
            DrawMirrorHandle(BedGimmicks.Mirrors.Right, Bounds.SetRight);
            DrawMirrorHandle(BedGimmicks.Mirrors.Foot, Bounds.SetFoot);
            if (BedGimmicks.Mirrors.Bottom.Use) DrawMirrorHandle(BedGimmicks.Mirrors.Bottom, Bounds.SetBottom);
            if (BedGimmicks.Colliders.Bottom.Use) DrawColliderHandle(BedGimmicks.Colliders.Bottom, Bounds.SetCollidersBottom);
            if (!Bounds.FitBottom && (BedGimmicks.Mirrors.Bottom.Use || BedGimmicks.Colliders.Bottom.Use))
            {
                Handles.color = BottomEdgeColor;
                DrawEdgeHandle(BedGimmicks.Colliders.Bottom, -Vector3.right, Bounds.SetBottomColliderLeft);
                DrawEdgeHandle(BedGimmicks.Colliders.Bottom, Vector3.right, Bounds.SetBottomColliderRight);
                DrawEdgeHandle(BedGimmicks.Colliders.Bottom, -Vector3.up, Bounds.SetBottomColliderHead);
                DrawEdgeHandle(BedGimmicks.Colliders.Bottom, Vector3.up, Bounds.SetBottomColliderFoot);
            }

            Gizmos.color = gizmosColor;
            Handles.color = handlesColor;
        }

        void DrawMirrorHandle(BedGimmicks.ObjectElement objectElement, Action<Vector3> onChange)
        {
            var enabled = objectElement.Use;
            Handles.color = enabled? MirrorEdgeColor : DisabledMirrorEdgeColor;
            DrawObjectElementHandle(objectElement, onChange, enabled ? MirrorSurfaceColor : DisabledMirrorSurfaceColor, enabled ? MirrorEdgeColor : DisabledMirrorEdgeColor);
        }

        void DrawColliderHandle(BedGimmicks.ObjectElement objectElement, Action<Vector3> onChange)
        {
            Handles.color = ColliderEdgeColor;
            DrawObjectElementHandle(objectElement, onChange, ColliderSurfaceColor, ColliderEdgeColor, true);
        }

        void DrawObjectElementHandle(BedGimmicks.ObjectElement objectElement, Action<Vector3> onChange, Color surfaceColor, Color edgeColor, bool invert = false)
        {
            var sizer = objectElement.Sizer;
            var x = sizer.right / 2 * sizer.localScale.x;
            var y = sizer.up / 2 * sizer.localScale.y;
            var verts = new Vector3[]
            {
                sizer.position + x + y,
                sizer.position + x - y,
                sizer.position - x - y,
                sizer.position - x + y,
            };
            Handles.DrawSolidRectangleWithOutline(verts, surfaceColor, edgeColor);
            EditorGUI.BeginChangeCheck();
            var pos = Handles.Slider(sizer.position, invert ? -sizer.forward : sizer.forward);
            if (EditorGUI.EndChangeCheck())
            {
                onChange(pos);
            }
        }

        void DrawEdgeHandle(BedGimmicks.ObjectElement objectElement, Vector3 axis, Action<Vector3> onChange)
        {
            var sizer = objectElement.Sizer;
            var x = sizer.right / 2 * sizer.localScale.x;
            var y = sizer.up / 2 * sizer.localScale.y;
            var offset = axis == Vector3.right
                ? x
                : axis == -Vector3.right
                ? -x
                : axis == Vector3.up
                ? y
                : -y;
            EditorGUI.BeginChangeCheck();
            var pos = Handles.Slider(objectElement.Sizer.position + offset, objectElement.Sizer.rotation * axis);
            if (EditorGUI.EndChangeCheck())
            {
                onChange(pos);
            }
        }

        class MirrorBounds
        {
            public bool Centered { get; set; }

            bool _FitBottom;
            public bool FitBottom
            {
                get => _FitBottom;
                set
                {
                    _FitBottom = value;
                    AdjustVariables();
                }
            }

            Transform Root { get; }
            BedGimmicks.MirrorsSet MirrorsSet { get; }
            BedGimmicks.CollidersSet CollidersSet { get; }

            Vector3 FloorCenter { get; }
            float _Height;
            float _Width;
            float _Tall;
            float _Bottom;
            float _CollidersBottom;
            Vector3 BottomColliderFloorCenter;
            float _BottomColliderWidth;
            float _BottomColliderTall;

            float Height
            {
                get => _Height;
                set
                {
                    _Height = value;
                    AdjustVariables();
                }
            }

            float HalfHeight => Height / 2;

            float Width
            {
                get => _Width;
                set
                {
                    _Width = value;
                    AdjustVariables();
                }
            }

            float HalfWidth => Width / 2;

            float Tall
            {
                get => _Tall;
                set
                {
                    _Tall = value;
                    AdjustVariables();
                }
            }

            float HalfTall => Tall / 2;

            float Bottom
            {
                get => _Bottom;
                set
                {
                    _Bottom = value;
                    AdjustVariables();
                }
            }

            float CollidersBottom
            {
                get => _CollidersBottom;
                set
                {
                    _CollidersBottom = value;
                    AdjustVariables();
                }
            }

            float BottomColliderWidth
            {
                get => _BottomColliderWidth;
                set
                {
                    _BottomColliderWidth = value;
                    AdjustVariables();
                }
            }

            float BottomColliderTall
            {
                get => _BottomColliderTall;
                set
                {
                    _BottomColliderTall = value;
                    AdjustVariables();
                }
            }

            public MirrorBounds(Transform root, BedGimmicks.MirrorsSet mirrorsSet, BedGimmicks.CollidersSet collidersSet)
            {
                Root = root;
                MirrorsSet = mirrorsSet;
                CollidersSet = collidersSet;

                FloorCenter = root.localPosition;
                _Height = mirrorsSet.Top.Positioner.localPosition.y;
                _Width = mirrorsSet.Right.Positioner.localPosition.x - mirrorsSet.Left.Positioner.localPosition.x;
                _Tall = mirrorsSet.Foot.Positioner.localPosition.z - mirrorsSet.Head.Positioner.localPosition.z;
                _Bottom = mirrorsSet.Bottom.Positioner.localPosition.y;
                _CollidersBottom = collidersSet.Bottom.Positioner.localPosition.y;

                var bottomColliderFloorCenter = collidersSet.Bottom.Positioner.localPosition;
                bottomColliderFloorCenter.y = 0;
                BottomColliderFloorCenter = bottomColliderFloorCenter;
                _BottomColliderWidth = collidersSet.Bottom.Sizer.localScale.x;
                _BottomColliderTall = collidersSet.Bottom.Sizer.localScale.y;
                _FitBottom = BottomColliderFloorCenter == Vector3.zero && _BottomColliderWidth == Width && _BottomColliderTall == Tall;
            }

            public void SetTop(Vector3 globalPosition)
            {
                Height = Vector3.Dot(Root.transform.up, globalPosition - Root.transform.position);
            }

            public void SetHead(Vector3 globalPosition)
            {
                SetAxis(globalPosition, -Vector3.forward, ref _Tall);
            }

            public void SetFoot(Vector3 globalPosition)
            {
                SetAxis(globalPosition, Vector3.forward, ref _Tall);
            }

            public void SetLeft(Vector3 globalPosition)
            {
                SetAxis(globalPosition, -Vector3.right, ref _Width);
            }

            public void SetRight(Vector3 globalPosition)
            {
                SetAxis(globalPosition, Vector3.right, ref _Width);
            }

            public void SetAxis(Vector3 globalPosition, Vector3 axis, ref float value)
            {
                var distanceFromCenter = Vector3.Dot(Root.transform.rotation * axis, globalPosition - Root.transform.position);
                if (Centered)
                {
                    value = distanceFromCenter * 2;
                    AdjustVariables();
                }
                else
                {
                    var distanceDiff = distanceFromCenter - value / 2;
                    value += distanceDiff;
                    AdjustVariables();
                    Root.transform.localPosition += axis * distanceDiff / 2;
                }
            }

            public void SetBottom(Vector3 globalPosition)
            {
                Bottom = Vector3.Dot(Root.transform.up, globalPosition - Root.transform.position);
            }

            public void SetCollidersBottom(Vector3 globalPosition)
            {
                CollidersBottom = Vector3.Dot(Root.transform.up, globalPosition - Root.transform.position);
            }

            public void SetBottomColliderLeft(Vector3 globalPosition)
            {
                SetBottomColliderAxis(globalPosition, -Vector3.right, ref _BottomColliderWidth);
            }

            public void SetBottomColliderRight(Vector3 globalPosition)
            {
                SetBottomColliderAxis(globalPosition, Vector3.right, ref _BottomColliderWidth);
            }

            public void SetBottomColliderHead(Vector3 globalPosition)
            {
                SetBottomColliderAxis(globalPosition, -Vector3.forward, ref _BottomColliderTall);
            }

            public void SetBottomColliderFoot(Vector3 globalPosition)
            {
                SetBottomColliderAxis(globalPosition, Vector3.forward, ref _BottomColliderTall);
            }

            public void SetBottomColliderAxis(Vector3 globalPosition, Vector3 axis, ref float value)
            {
                var distanceFromCenter = Vector3.Dot(Root.transform.rotation * axis, globalPosition - (Root.transform.position + Root.transform.rotation * BottomColliderFloorCenter));
                if (Centered)
                {
                    value = distanceFromCenter * 2;
                    AdjustVariables();
                }
                else
                {
                    var distanceDiff = distanceFromCenter - value / 2;
                    value += distanceDiff;
                    BottomColliderFloorCenter += axis * distanceDiff / 2;
                    AdjustVariables();
                }
            }

            string Vector3String(Vector3 v) => $"({v.x}, {v.y}, {v.z})";

            void AdjustVariables()
            {
                Undo.RecordObjects(new UnityEngine.Object[]
                {
                    Root, // constants
                    MirrorsSet.Root, // constants
                    CollidersSet.Root, // constants
                    MirrorsSet.Top.Positioner,
                    MirrorsSet.Head.Positioner,
                    MirrorsSet.Left.Positioner,
                    MirrorsSet.Right.Positioner,
                    MirrorsSet.Foot.Positioner,
                    MirrorsSet.Bottom.Positioner,
                    MirrorsSet.Top.Sizer,
                    MirrorsSet.Head.Sizer,
                    MirrorsSet.Left.Sizer,
                    MirrorsSet.Right.Sizer,
                    MirrorsSet.Foot.Sizer,
                    MirrorsSet.Bottom.Sizer,
                    CollidersSet.Bottom.Positioner,
                    CollidersSet.Bottom.Sizer,
                }, "Adjust Transforms");

                if (FitBottom)
                {
                    BottomColliderFloorCenter = Vector3.zero;
                    _BottomColliderWidth = Width;
                    _BottomColliderTall = Tall;
                }

                var topSize = new Vector3(Width, Tall, 1);
                var headSize = new Vector3(Width, Height, 1);
                var sideSize = new Vector3(Tall, Height, 1);
                var bottomSize = new Vector3(BottomColliderWidth, BottomColliderTall, 1);
                MirrorsSet.Top.Positioner.localPosition = new Vector3(0, Height, -HalfTall);
                MirrorsSet.Top.Sizer.localPosition = new Vector3(0, 0, HalfTall);
                MirrorsSet.Top.Sizer.localScale = topSize;
                MirrorsSet.Head.Positioner.localPosition = new Vector3(0, HalfHeight, -HalfTall);
                MirrorsSet.Head.Sizer.localScale = headSize;
                MirrorsSet.Left.Positioner.localPosition = new Vector3(-HalfWidth, HalfHeight, 0);
                MirrorsSet.Left.Sizer.localScale = sideSize;
                MirrorsSet.Right.Positioner.localPosition = new Vector3(HalfWidth, HalfHeight, 0);
                MirrorsSet.Right.Sizer.localScale = sideSize;
                MirrorsSet.Foot.Positioner.localPosition = new Vector3(0, HalfHeight, HalfTall);
                MirrorsSet.Foot.Sizer.localScale = headSize;
                MirrorsSet.Bottom.Positioner.localPosition = new Vector3(BottomColliderFloorCenter.x, Bottom, BottomColliderFloorCenter.z);
                MirrorsSet.Bottom.Sizer.localScale = bottomSize;
                CollidersSet.Bottom.Positioner.localPosition = new Vector3(BottomColliderFloorCenter.x, CollidersBottom, BottomColliderFloorCenter.z);
                CollidersSet.Bottom.Sizer.localScale = bottomSize;
                EnsureConstants();
            }

            void EnsureConstants()
            {
                Root.localRotation = Quaternion.identity;
                Root.localScale = Vector3.one;
                MirrorsSet.Root.localPosition = Vector3.zero;
                MirrorsSet.Root.localRotation = Quaternion.identity;
                MirrorsSet.Root.localScale = Vector3.one;
                CollidersSet.Root.localPosition = Vector3.zero;
                CollidersSet.Root.localRotation = Quaternion.identity;
                CollidersSet.Root.localScale = Vector3.one;

                MirrorsSet.Top.Positioner.localRotation = Quaternion.identity;
                MirrorsSet.Head.Positioner.localRotation = Quaternion.identity;
                MirrorsSet.Left.Positioner.localRotation = Quaternion.identity;
                MirrorsSet.Right.Positioner.localRotation = Quaternion.identity;
                MirrorsSet.Foot.Positioner.localRotation = Quaternion.identity;
                MirrorsSet.Bottom.Positioner.localRotation = Quaternion.identity;
                CollidersSet.Bottom.Positioner.localRotation = Quaternion.identity;

                MirrorsSet.Top.Positioner.localScale = Vector3.one;
                MirrorsSet.Head.Positioner.localScale = Vector3.one;
                MirrorsSet.Left.Positioner.localScale = Vector3.one;
                MirrorsSet.Right.Positioner.localScale = Vector3.one;
                MirrorsSet.Foot.Positioner.localScale = Vector3.one;
                MirrorsSet.Bottom.Positioner.localScale = Vector3.one;
                CollidersSet.Bottom.Positioner.localScale = Vector3.one;

                // Top Sizer pos is variable
                MirrorsSet.Head.Sizer.localPosition = Vector3.zero;
                MirrorsSet.Left.Sizer.localPosition = Vector3.zero;
                MirrorsSet.Right.Sizer.localPosition = Vector3.zero;
                MirrorsSet.Foot.Sizer.localPosition = Vector3.zero;
                MirrorsSet.Bottom.Sizer.localPosition = Vector3.zero;
                CollidersSet.Bottom.Sizer.localPosition = Vector3.zero;

                MirrorsSet.Top.Sizer.localRotation = Quaternion.Euler(-90, 0, 0);
                MirrorsSet.Head.Sizer.localRotation = Quaternion.Euler(0, 180, 0);
                MirrorsSet.Left.Sizer.localRotation = Quaternion.Euler(0, -90, 0);
                MirrorsSet.Right.Sizer.localRotation = Quaternion.Euler(0, 90, 0);
                MirrorsSet.Foot.Sizer.localRotation = Quaternion.Euler(0, 0, 0);
                MirrorsSet.Bottom.Sizer.localRotation = Quaternion.Euler(90, 0, 0);
                CollidersSet.Bottom.Sizer.localRotation = Quaternion.Euler(90, 0, 0);
            }
        }
    }
}

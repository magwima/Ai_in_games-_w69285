using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace K_PathFinder.Samples
{
    public class SimplePatrolPath : MonoBehaviour, IEnumerable<Vector3>
    {
        [SerializeField]
        public List<Vector3> points = new List<Vector3>(); // now serialized and initialized
        public LayerMask mask = 1;
        public Color color = Color.red;

        void Awake()
        {
            Debug.Log("Points at Awake: " + (points != null ? points.Count.ToString() : "null"));
        }

        void OnDrawGizmos()
        {
            if (Count != 0 && Count > 1)
            {
                Gizmos.color = color;
                for (int i = 0; i < Count - 1; i++)
                {
                    Gizmos.DrawLine(points[i], points[i + 1]);
                }
                Gizmos.DrawLine(points[Count - 1], points[0]);
            }
        }

        public Vector3 this[int index]
        {
            get { return points[index]; }
            set { points[index] = value; }
        }

        public int Count
        {
            get
            {
                if (points == null)
                    return 0;
                return points.Count;
            }
        }

        public IEnumerator<Vector3> GetEnumerator()
        {
            return points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return points.GetEnumerator();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SimplePatrolPath))]
    public class PositionHandleExampleEditor : Editor {
        float groundedDistance = 2f;
        List<Vector3> list = new List<Vector3>();

        Tool lastTool = Tool.None;
        void OnEnable() {
            lastTool = Tools.current;
            Tools.current = Tool.None;
        }

        void OnDisable() {
            Tools.current = lastTool;
        }

        public override void OnInspectorGUI() {
            SimplePatrolPath patrol = (SimplePatrolPath)target;
            if (patrol == null)
                return;

            var serializedObject = new SerializedObject(patrol);
            var layersProperty = serializedObject.FindProperty("mask");
            EditorGUILayout.PropertyField(layersProperty, true);
            if (GUILayout.Button("Set points grounded")) {
                GroundPatrolPath(patrol, groundedDistance);
            }
            groundedDistance = EditorGUILayout.FloatField("Max ground distance", groundedDistance);

            patrol.color = EditorGUILayout.ColorField(patrol.color);
            if (GUI.changed) {
                Undo.RecordObject(patrol, "Patrol serialization");
            }

            if (GUILayout.Button("Add Point")) {
                int count = patrol.Count;
                List<Vector3> newPoints = new List<Vector3>();

                switch (count) {
                    case 0:
                        newPoints.Add(patrol.transform.position);
                        break;
                    case 1:
                        newPoints.Add(patrol[0]);
                        newPoints.Add(patrol[0] + Vector3.right);
                        break;
                    default:
                        for (int i = 0; i < count; i++) {
                            newPoints.Add(patrol[i]);
                        }
                        newPoints.Add(patrol[count - 1] + (patrol[count - 1] - patrol[count - 2]).normalized);
                        break;
                }

                Undo.RecordObject(patrol, "Add point to patrol path");
                patrol.points = newPoints;
                GroundPatrolPath(patrol, groundedDistance);
            }

            bool remove = false;
            int removeIndex = 0;
            for (int i = 0; i < patrol.Count; i++) {
                GUILayout.BeginHorizontal();
                GUILayout.Label(string.Format("[{0}]", i));
                patrol[i] = EditorGUILayout.Vector3Field(string.Empty, patrol[i]);
                if (GUILayout.Button("Remove")) {
                    remove = true;
                    removeIndex = i;
                }
                GUILayout.EndHorizontal();
            }

            if (remove) {
                List<Vector3> temp = new List<Vector3>(patrol.points);
                temp.RemoveAt(removeIndex);
                Undo.RecordObject(patrol, "Remove point at patrol path");
                patrol.points = temp;
            }
        }

        protected virtual void OnSceneGUI() {
            SimplePatrolPath patrol = (SimplePatrolPath)target;
            if (patrol == null)
                return;

            EditorGUI.BeginChangeCheck();
            list.Clear();
            list.AddRange(patrol);

            for (int i = 0; i < patrol.Count; i++) {
                list[i] = Handles.PositionHandle(list[i], Quaternion.identity);
                Handles.Label(list[i], i.ToString());
            }

            if (EditorGUI.EndChangeCheck()) {
                patrol.points.Clear();
                Undo.RecordObject(patrol, "Change patrol path point position");
                patrol.points.AddRange(list);
            }
        }

        public static void GroundPatrolPath(SimplePatrolPath target, float height) {
            List<Vector3> list = new List<Vector3>();

            for (int i = 0; i < target.Count; i++) {
                RaycastHit hit;
                if (Physics.Raycast(target[i] + new Vector3(0, height, 0), Vector3.down, out hit, height * 2, target.mask.value))
                    list.Add(hit.point);
                else
                    list.Add(target[i]);
            }

            Undo.RecordObject(target, "Ground patrol path");
            target.points = list;
        }
    }
#endif
}

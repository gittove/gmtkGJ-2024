using System;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(SnapMarker))]
public class Snapper : Editor
{
    private static float IncrementPos = 5f;
    private static float IncrementRotation = 90;

    private SerializedObject _this;
    
    private void OnEnable()
    {
        _this = new SerializedObject(this);
    }

    [MenuItem("Game/Rotate ^r")]
    private static void Rotate()
    {
        var selected = Selection.GetFiltered<GameObject>(SelectionMode.TopLevel);
        foreach (var obj in selected)
        {
            if (!obj.TryGetComponent<SnapMarker>(out _))
            {
                continue;
            }
            var pos = obj.transform.position;
            pos.x = Mathf.Round(pos.x / IncrementPos) * IncrementPos;
            pos.z = Mathf.Round(pos.z / IncrementPos) * IncrementPos;
            obj.transform.position = pos;

            var rotation = obj.transform.rotation.eulerAngles;
            rotation.y = Mathf.Round(rotation.y / IncrementRotation) * IncrementRotation + IncrementRotation;
            obj.transform.rotation = Quaternion.Euler(rotation);
        }
    }

    private void OnSceneGUI()
    {
        var selected = Selection.GetFiltered<GameObject>(SelectionMode.TopLevel);
        foreach (var obj in selected)
        {
            if (!obj.TryGetComponent<SnapMarker>(out _))
            {
                continue;
            }
            var pos = obj.transform.position;
            pos.x = Mathf.Round(pos.x / IncrementPos) * IncrementPos;
            pos.y = 0;
            pos.z = Mathf.Round(pos.z / IncrementPos) * IncrementPos;
            obj.transform.position = pos;

            var rotation = obj.transform.rotation.eulerAngles;
            rotation.y = Mathf.Round(rotation.y / IncrementRotation) * IncrementRotation;
            obj.transform.rotation = Quaternion.Euler(rotation);
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(_this.FindProperty(nameof(IncrementPos)));
    }
}
using System;
using UnityEditor;
using UnityEngine;

public class Snapper : Editor
{
    private void OnSceneGUI()
    {
        var selected = Selection.gameObjects;
        foreach (Transform transform in Selection.transforms)
        {
            
        }
    }
}

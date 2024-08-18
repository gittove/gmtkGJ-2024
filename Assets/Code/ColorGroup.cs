using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Color Group")]
public class ColorGroup : ScriptableObject
{
    public ReplaceableMaterial Group;
    
    [Serializable]
    public struct ReplaceableMaterial
    {
        public string MaterialName;
        public Color[] Colors;
    }
}
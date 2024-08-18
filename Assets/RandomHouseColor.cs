using UnityEngine;

public class RandomHouseColor : MonoBehaviour
{
    [SerializeField] private ColorGroup[] Replaceables;

    private void Start()
    {
        var mesh = GetComponent<MeshRenderer>();
        var materials = mesh.materials;

        foreach (var mat in materials)
        foreach (var group in Replaceables)
        {
            var matName = mat.name.ToLower();
            var colorName = group.Group.MaterialName.ToLower();
            if (!matName.Contains(colorName))
            {
                continue;
            }
            
            var colors = group.Group.Colors;
            var color = colors[Random.Range(0, colors.Length)];
            mat.color = color;
        }
    }
}
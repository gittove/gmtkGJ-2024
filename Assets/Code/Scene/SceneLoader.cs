using UnityEditor.SceneManagement;
using UnityEngine;

[ExecuteInEditMode]
public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string[] _scenes;
    
    [ContextMenu("Load Scenes")]
    private void Awake()
    {
        for (int i = 0; i < _scenes.Length; i++)
        {
            EditorSceneManager.OpenScene($"Assets/Scenes/{_scenes[i]}.unity", OpenSceneMode.Additive);
        }
    }
}
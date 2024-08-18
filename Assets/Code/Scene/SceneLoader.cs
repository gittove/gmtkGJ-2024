#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string[] _scenes;
    
    [ContextMenu("Load Scenes")]
    private void Awake()
    {
        for (int i = 0; i < _scenes.Length; i++)
        {
#if UNITY_EDITOR
            EditorSceneManager.OpenScene($"Assets/Scenes/{_scenes[i]}.unity", OpenSceneMode.Additive);
#else
            SceneManager.LoadScene(_scenes[i], LoadSceneMode.Additive);
#endif
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement; // needed for scene loading

// Keeps important managers alive across scenes
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // stays alive between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method for switching scenes
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

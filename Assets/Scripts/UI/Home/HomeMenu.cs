using UnityEngine;

public class HomeMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCreateCharater()
    {
        // Load the character creation scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterCreation");
    }

    public void OnLoadCharacter()
    {
        // Load the character creation scene
       // UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterCreation");
    }

    public void OnExit()
    {
        // Exit the game
        Application.Quit();
    }
}

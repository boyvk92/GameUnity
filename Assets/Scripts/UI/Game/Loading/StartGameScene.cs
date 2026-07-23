using System.Collections;
using UnityEngine;

public class StartGameScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(WaitAndStartGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DongPhuScene");
    }

    private IEnumerator WaitAndStartGame()
    {
        yield return new WaitForSeconds(1f);
        StartGame();
    }
}

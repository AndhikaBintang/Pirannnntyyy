using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER");
        SceneManager.LoadScene("GameOver");  // Pastikan nama scene sesuai
    }
}

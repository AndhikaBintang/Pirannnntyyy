using UnityEngine;
using UnityEngine.UI; // Hapus ini jika belum ada UI

public class ScoreManager : MonoBehaviour
{
    public Transform player; // drag player ke sini
    public Text scoreText;   // optional jika pakai UI

    private float startX;
    public float currentScore;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player belum di assign ke ScoreManager");
            enabled = false;
            return;
        }

        startX = player.position.x;
    }

    void Update()
    {
        // hitung jarak yang ditempuh
        currentScore = player.position.x - startX;

        if (currentScore < 0)
            currentScore = 0;

        // tampilkan jika pakai UI
        if (scoreText != null)
            scoreText.text = "Score: " + Mathf.FloorToInt(currentScore).ToString();
    }
}

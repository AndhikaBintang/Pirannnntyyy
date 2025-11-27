using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;

    public float currentScore;

    void Update()
    {
        // waktu bermain sejak scene dimulai
        currentScore = Time.timeSinceLevelLoad;

        // tampilkan skor (dibulatkan)
        if (scoreText != null)
            scoreText.text = "Time: " + Mathf.FloorToInt(currentScore).ToString() + "s";
    }
}

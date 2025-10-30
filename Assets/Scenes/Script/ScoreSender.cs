using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ScoreSender : MonoBehaviour
{
    public string apiUrl = "http://localhost:5000/api/score"; // Sesuaikan dengan API kamu

    // Panggil function ini saat Game Over
    public void SendScore(string playerName, float score)
    {
        StartCoroutine(SendScoreRoutine(playerName, score));
    }

    IEnumerator SendScoreRoutine(string playerName, float score)
    {
        ScoreData data = new ScoreData();
        data.playerName = playerName;
        data.score = Mathf.FloorToInt(score);

        string json = JsonUtility.ToJson(data);
        Debug.Log("Sending JSON: " + json);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Score berhasil dikirim ke server!");
        }
        else
        {
            Debug.LogError("Gagal kirim skor: " + request.error);
        }
    }
}

[System.Serializable]
public class ScoreData
{
    public string playerName;
    public float score;
}

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

// Ini akan menjadi Singleton agar mudah diakses dari mana saja
public class ApiClient : MonoBehaviour
{
    public static ApiClient Instance;

    //GANTI URL INI DENGAN URL ANDA DARI VISUAL STUDIO (lihat di browser)
    private const string BASE_URL = "https://localhost:7046";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opsional: agar tidak hancur saat ganti scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- Fungsi LOAD STATE ---
    // Menggunakan Coroutine untuk menunggu respons web
    public IEnumerator LoadState(string playerName, System.Action<GameState> onSuccess, System.Action<string> onError)
    {
        string url = $"{BASE_URL}/api/gamestate/{playerName}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            // Kirim request dan tunggu...
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Sukses
                string jsonResponse = www.downloadHandler.text;
                GameState state = JsonUtility.FromJson<GameState>(jsonResponse);
                onSuccess?.Invoke(state);
            }
            else
            {
                // Error (misalnya 404 Not Found)
                onError?.Invoke($"Error {www.responseCode}: {www.error}");
            }
        }
    }

    // --- Fungsi SAVE STATE ---
    public IEnumerator SaveState(GameState stateData, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{BASE_URL}/api/gamestate/{stateData.playerName}";
        string jsonData = JsonUtility.ToJson(stateData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest www = UnityWebRequest.Put(url, bodyRaw))
        {
            // Kita harus set header ini untuk request PUT/POST
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Sukses (API akan mengembalikan 204 No Content)
                onSuccess?.Invoke();
            }
            else
            {
                // Error
                onError?.Invoke($"Error {www.responseCode}: {www.error}");
            }
        }
    }

    // --- Fungsi SUBMIT SCORE (dari PDF) ---
    public IEnumerator SubmitScore(string playerName, int score, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{BASE_URL}/api/scores";
        string jsonData = $"{{ \"playerName\": \"{playerName}\", \"score\": {score} }}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        // Perhatikan: Submit Skor menggunakan POST (membuat data baru)
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Sukses (API akan mengembalikan 201 Created)
                onSuccess?.Invoke();
            }
            else
            {
                // Error
                onError?.Invoke($"Error {www.responseCode}: {www.error}");
            }
        }
    }
}
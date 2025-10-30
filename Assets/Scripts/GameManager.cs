using UnityEngine;
using TMPro; // Gunakan ini jika Anda pakai TextMeshPro untuk UI

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    public string playerName = "Dewa"; // Nama default
    public int health = 100;
    public int score = 0;
    public Transform playerTransform; // Seret objek Player Anda ke sini

    [Header("UI (Opsional)")]
    public TMP_InputField playerNameInput; // Input field untuk nama
    public TextMeshProUGUI feedbackText; // Teks untuk menampilkan "Loading..."

    void Start()
    {
        // Pastikan ApiClient ada di scene
        if (ApiClient.Instance == null)
        {
            new GameObject("ApiClient").AddComponent<ApiClient>();
        }
    }

    // --- Fungsi ini akan dipanggil oleh Tombol UI ---

    public void OnSaveButtonPress()
    {
        if (playerTransform == null) return;

        // 1. Ambil nama dari UI
        if (playerNameInput != null && !string.IsNullOrWhiteSpace(playerNameInput.text))
        {
            playerName = playerNameInput.text;
        }

        // 2. Siapkan data state
        GameState currentState = new GameState
        {
            playerName = this.playerName,
            health = this.health,
            positionX = playerTransform.position.x,
            positionY = playerTransform.position.y,
            positionZ = playerTransform.position.z,
            currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            // Anda bisa tambahkan inventory, dll.
        };

        // 3. Panggil API (via Coroutine)
        SetFeedback("Menyimpan...");
        StartCoroutine(ApiClient.Instance.SaveState(currentState,
            () => {
                // Ini adalah callback onSuccess
                SetFeedback($"Game disimpan untuk {playerName}!");
            },
            (errorMsg) => {
                // Ini adalah callback onError
                SetFeedback($"Gagal menyimpan: {errorMsg}");
            }
        ));
    }

    public void OnLoadButtonPress()
    {
        // 1. Ambil nama dari UI
        if (playerNameInput != null && !string.IsNullOrWhiteSpace(playerNameInput.text))
        {
            playerName = playerNameInput.text;
        }

        // 2. Panggil API
        SetFeedback($"Mencari save data untuk {playerName}...");
        StartCoroutine(ApiClient.Instance.LoadState(playerName,
            (loadedState) => {
                // Ini adalah callback onSuccess
                SetFeedback("Load berhasil! Menerapkan state...");

                // 3. Terapkan state ke game
                this.health = loadedState.health;
                this.score = 0; // Reset skor, atau muat dari state jika ada
                playerTransform.position = new Vector3(
                    loadedState.positionX,
                    loadedState.positionY,
                    loadedState.positionZ
                );
            },
            (errorMsg) => {
                // Ini adalah callback onError
                SetFeedback($"Gagal load: {errorMsg}");
            }
        ));
    }

    // Panggil ini saat game over
    public void OnGameOver()
    {
        SetFeedback("Mengirim skor...");
        StartCoroutine(ApiClient.Instance.SubmitScore(this.playerName, this.score,
            () => {
                SetFeedback("Skor berhasil dikirim!");
            },
            (errorMsg) => {
                SetFeedback($"Gagal kirim skor: {errorMsg}");
            }
        ));
    }

    // Fungsi helper untuk UI
    void SetFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
        Debug.Log(message); // Selalu tampilkan di konsol
    }
}
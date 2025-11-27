using UnityEngine;
using System.IO; // Untuk operasi file

public class ScreenshotCapturer : MonoBehaviour
{
    public int superSize = 2; // Faktor perkalian resolusi (1=normal, 2=2x, dll.)
    public string folderName = "Screenshots"; // Nama folder tempat menyimpan

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // Tekan 'P' untuk mengambil screenshot
        {
            TakeScreenshot();
        }
    }

    void TakeScreenshot()
    {
        string folderPath = Path.Combine(Application.dataPath, folderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = Path.Combine(folderPath, "Screenshot_" + timestamp + ".png");

        ScreenCapture.CaptureScreenshot(fileName, superSize);
        Debug.Log("Screenshot saved to: " + fileName);
    }
}
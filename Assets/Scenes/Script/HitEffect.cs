using System.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;

public class HitEffect : MonoBehaviour
{
    public Image redOverlay;
    public float fadeSpeed = 3f;
    public CameraShake camShake;

    public void PlayHitEffect()
    {
        if (camShake != null)
            StartCoroutine(camShake.Shake(0.15f, 0.15f));

        StartCoroutine(BlinkRed());
    }

    IEnumerator BlinkRed()
    {
        redOverlay.color = new Color(1, 0, 0, 0.4f);

        yield return new WaitForSeconds(0.1f);

        while (redOverlay.color.a > 0f)
        {
            redOverlay.color = Color.Lerp(redOverlay.color, new Color(1, 0, 0, 0), Time.deltaTime * fadeSpeed);
            yield return null;
        }
    }
}

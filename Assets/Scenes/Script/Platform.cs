using UnityEngine;

public class Platform : MonoBehaviour
{
    [Tooltip("Jika >0 maka dipakai, else akan auto hitung dari bounds.")]
    public float width = 0f;

    [HideInInspector]
    public int prefabIndex = -1; // diisi oleh spawner saat Instantiate/Pool

    public float GetWidth()
    {
        if (width > 0f) return width;

        // coba renderer
        var rend = GetComponentInChildren<Renderer>();
        if (rend != null) return rend.bounds.size.x;

        // coba collider 3D
        var col = GetComponent<Collider>();
        if (col != null) return col.bounds.size.x;

        // coba collider2D
        var col2 = GetComponent<Collider2D>();
        if (col2 != null) return col2.bounds.size.x;

        return 1f; // fallback
    }
}

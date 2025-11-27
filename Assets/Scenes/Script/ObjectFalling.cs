using System.Collections;
using UnityEngine;

public class ObjectFalling : MonoBehaviour
{
    public int damageAmount = 10; // damage ke player
    [HideInInspector] public SpawnerManager spawner;

    private void OnTriggerEnter(Collider other)
    {
        HealthSystem health = other.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.TakeDamage(damageAmount);   // Kurangi health 10
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}

using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    public int damage = 20;
    public LayerMask targetLayers;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            var health = other.GetComponent<BossHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
}
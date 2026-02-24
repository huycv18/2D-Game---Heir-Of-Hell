using UnityEngine;

public class ExplosionEnemy : Enemy
{
    [SerializeField] private GameObject explosionPrefabs;

    private bool hasExploded = false; // chống spam nổ

    protected override void OnEnable()
    {
        base.OnEnable();
        hasExploded = false; // reset khi RoomSpawner tái sử dụng
    }

    private void CreateExplosion()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (explosionPrefabs != null)
            Instantiate(explosionPrefabs, transform.position, Quaternion.identity);

        audioManager?.PlayEnemyExplosionSound();
    }

    protected override void Die()
    {
        CreateExplosion();
        base.Die();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded) return;
        if (collision.CompareTag("Player"))
        {
            // Chạm Player → nổ → chết luôn
            Die();
        }
    }
}

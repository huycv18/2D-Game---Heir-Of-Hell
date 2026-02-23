using UnityEngine;

public class ExplosionEnemy : Enemy
{
    [SerializeField] private GameObject explosionPrefabs;
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
        if (audioManager == null)
            audioManager = FindAnyObjectByType<AudioManager>();
    }

    private void CreateExplosion()
    {
        if (explosionPrefabs != null)
        {
            Instantiate(
                explosionPrefabs,
                transform.position,
                Quaternion.identity
            );
        }
        audioManager?.PlayEnemyExplosionSound();
    }

    protected override void Die()
    {
        CreateExplosion();
        base.Die();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CreateExplosion();
        }
    }
}

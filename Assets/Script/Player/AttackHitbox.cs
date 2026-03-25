using UnityEngine;
using System.Collections.Generic;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private float damage = 40f;

    // Danh sách enemy đã bị hit trong 1 lần swing, tránh hit nhiều lần
    private List<Enemy> hitEnemies = new List<Enemy>();

    private void OnEnable()
    {
        // Reset danh sách mỗi lần hitbox được bật lên (mỗi lần swing)
        hitEnemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                hitEnemies.Add(enemy);
                enemy.TakeDamage(damage);

                float dir = collision.transform.position.x > transform.position.x ? 1 : -1;
                enemy.ApplyKnockback(dir, 8f);
            }
        }
    }
}
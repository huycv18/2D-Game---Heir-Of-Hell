using UnityEngine;

public class EnergyEnemy : Enemy
{
    [SerializeField] private GameObject energyObject;
    [SerializeField] private float stayDamageInterval = 0.5f;
    private float stayTimer = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            stayTimer = 0f;
            collision.GetComponent<PlayerController>()?.TakeDamage(enterDamage, transform.position);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            stayTimer += Time.deltaTime;
            if (stayTimer >= stayDamageInterval)
            {
                stayTimer = 0f;
                collision.GetComponent<PlayerController>()?.TakeDamage(stayDamage, transform.position);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            stayTimer = 0f;
    }

    protected override void Die()
    {
        if (energyObject != null)
            Instantiate(energyObject, transform.position, Quaternion.identity);
            // Không Destroy thủ công — ItemPickup tự Destroy khi Player nhặt
        base.Die();
    }
}

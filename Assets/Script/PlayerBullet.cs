using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float timeDestroy = 0.5f;

    private int direction = 1;

    public void SetDirection(int dir)
    {
        direction = dir;

        
        if (dir < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void Start()
    {
        Destroy(gameObject, timeDestroy);
    }

    void Update()
    {
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Enemy"))
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage();
        }

        Destroy(gameObject);
    }
}

}

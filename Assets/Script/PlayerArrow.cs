using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 14f;   // mũi tên bay chậm hơn đạn
    [SerializeField] private float timeDestroy = 1.2f; // bay lâu hơn

    private int direction = 1;

    public void SetDirection(int dir)
    {
        direction = dir;

        // lật sprite theo hướng bắn
        if (dir < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void Start()
    {
        Destroy(gameObject, timeDestroy);
    }

    void Update()
    {
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);
    }
}

using UnityEngine;

public class Spinner : MonoBehaviour
{
    [Tooltip("Tốc độ xoay (độ/giây). Số âm để quay theo chiều kim đồng hồ, số dương ngược lại.")]
    [SerializeField] private float rotationSpeed = -200f;

    // Update is called once per frame
    void Update()
    {
        // Xoay quanh trục Z cho UI 2D
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}

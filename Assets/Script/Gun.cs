using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Fire")]
    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private float shotDelay = 0.15f;

    [Header("Ammo")]
    [SerializeField] private int maxAmmo = 24;
    public int currentAmmo;

    private float nextShot;
    private Transform player;

    void Start()
    {
        player = transform.root;   // lấy Player cha
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        Shot();
        Reload();
    }

    void Shot()
    {
        if (Input.GetMouseButtonDown(0) && currentAmmo > 0 && Time.time > nextShot)
        {
            nextShot = Time.time + shotDelay;

            GameObject bulletObj = Instantiate(
                bulletPrefabs,
                firePos.position,
                Quaternion.identity
            );

            PlayerBullet bullet = bulletObj.GetComponent<PlayerBullet>();

            // xác định hướng bắn theo player
            int direction = player.localScale.x > 0 ? 1 : -1;
            bullet.SetDirection(direction);

            currentAmmo--;
        }
    }

    void Reload()
    {
        if (Input.GetMouseButtonDown(1) && currentAmmo < maxAmmo)
        {
            currentAmmo = maxAmmo;
        }
    }
}

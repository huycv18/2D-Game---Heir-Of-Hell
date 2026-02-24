using UnityEngine;

public class Bow : MonoBehaviour
{
    [Header("Fire")]
    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject arrowPrefabs;
    [SerializeField] private float shotDelay = 0.6f;   

    [Header("Ammo")]
    [SerializeField] private int maxAmmo = 12;         
    public int currentAmmo;

    private float nextShot;
    private Transform player;

    void Start()
    {
        player = transform.root;
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

            GameObject arrowObj = Instantiate(
                arrowPrefabs,
                firePos.position,
                Quaternion.identity
            );

            PlayerArrow arrow = arrowObj.GetComponent<PlayerArrow>();

            
            int direction = player.localScale.x > 0 ? 1 : -1;
            arrow.SetDirection(direction);

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

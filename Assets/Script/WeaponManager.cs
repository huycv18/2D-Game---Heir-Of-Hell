using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    public GameObject sword;
    public GameObject gun;
    public GameObject bow;

    void Start()
    {
        EquipSword();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            EquipSword();

        if (Input.GetKeyDown(KeyCode.Q))
            EquipGun();

        if (Input.GetKeyDown(KeyCode.R))
            EquipBow();
    }

    void EquipSword()
    {
        sword.SetActive(true);
        gun.SetActive(false);
        bow.SetActive(false);
    }

    void EquipGun()
    {
        sword.SetActive(false);
        gun.SetActive(true);
        bow.SetActive(false);
    }

    void EquipBow()
    {
        sword.SetActive(false);
        gun.SetActive(false);
        bow.SetActive(true);
    }
}

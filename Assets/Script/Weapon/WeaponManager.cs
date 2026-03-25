using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Objects")]
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject bow;

    private Animator animator;

    private enum WeaponState { Melee, Gun, Bow }
    private WeaponState currentWeapon = WeaponState.Melee;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        EnableMelee();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Đang cầm súng → cất đi, không thì rút súng
            if (currentWeapon == WeaponState.Gun)
                EnableMelee();
            else
                EquipGun();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            // Đang cầm cung → cất đi, không thì rút cung
            if (currentWeapon == WeaponState.Bow)
                EnableMelee();
            else
                EquipBow();
        }
        // Phím T đã được dùng bởi TutorialController — không dùng ở đây nữa
    }

    private void EquipGun()
    {
        gun.SetActive(true);
        bow.SetActive(false);
        currentWeapon = WeaponState.Gun;
        animator.SetBool("CanCombat", false);
    }

    private void EquipBow()
    {
        gun.SetActive(false);
        bow.SetActive(true);
        currentWeapon = WeaponState.Bow;
        animator.SetBool("CanCombat", false);
    }

    private void EnableMelee()
    {
        gun.SetActive(false);
        bow.SetActive(false);
        currentWeapon = WeaponState.Melee;
        animator.SetBool("CanCombat", true);
    }
}

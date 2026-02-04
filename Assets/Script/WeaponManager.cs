using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Objects")]
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject bow;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Mặc định dùng chém
        EnableMelee();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EquipGun();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            EquipBow();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            EnableMelee();
        }
    }

    private void EquipGun()
    {
        gun.SetActive(true);
        bow.SetActive(false);

        animator.SetBool("CanCombat", false);
    }

    private void EquipBow()
    {
        gun.SetActive(false);
        bow.SetActive(true);

        animator.SetBool("CanCombat", false);
    }

    private void EnableMelee()
    {
        gun.SetActive(false);
        bow.SetActive(false);

        animator.SetBool("CanCombat", true);
    }
}

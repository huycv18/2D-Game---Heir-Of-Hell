using UnityEngine;

public class BossEnemy : Enemy
{
[SerializeField] private GameObject bulletPrefabs;
[SerializeField] private Transform firePoint;
[SerializeField] private float speedDanThuong = 20f;
[SerializeField] private float speedDanVongTron = 10f;
[SerializeField] private float hpValue = 100f;
[SerializeField] private GameObject miniEnemy;
[SerializeField] private float skillCooldown = 2f;
private float nextSkillTime = 0f;
protected override void Update()
{
    base.Update();

    if (Time.time >= nextSkillTime)
    {
        SuDungSkill();
    }
}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.TakeDamage(enterDamage);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.TakeDamage(stayDamage);
        }
    }
   private void BanDanThuong()
{
    if (player != null)
    {
        Vector3 directionToPlayer = player.transform.position - firePoint.position;
        directionToPlayer.Normalize();

        GameObject bullet = Instantiate(bulletPrefabs, firePoint.position, Quaternion.identity);

        EnemyBullet enemyBullet = bullet.AddComponent<EnemyBullet>();
        enemyBullet.SetMovementDirection(directionToPlayer * speedDanThuong);
    }
}

private void BanDanVongTron()
{
    const int bulletCount = 12;
    float angleStep = 360f / bulletCount;

    for (int i = 0; i < bulletCount; i++)
    {
        float angle = i * angleStep;

        Vector3 bulletDirection = new Vector3(
            Mathf.Cos(Mathf.Deg2Rad * angle),
            Mathf.Sin(Mathf.Deg2Rad * angle),
            0f
        );

        GameObject bullet = Instantiate(bulletPrefabs, transform.position, Quaternion.identity);

        EnemyBullet enemyBullet = bullet.AddComponent<EnemyBullet>();
        enemyBullet.SetMovementDirection(bulletDirection * speedDanVongTron);
    }
}

private void HoiMau(float hpAmount)
{
    currentHP = Mathf.Min(currentHP + hpAmount, maxHP);
    UpdateHpBar();
}

private void SinhMiniEnemy()
{
    Instantiate(miniEnemy, transform.position, Quaternion.identity);
}

private void DichChuyen()
{
    if (player != null)
    {
        transform.position = player.transform.position;
    }
}
private void ChonSkillNgauNhien()
{
    int randomSkill = Random.Range(0, 5);

    switch (randomSkill)
    {
        case 0:
            BanDanThuong();
            break;

        case 1:
            BanDanVongTron();
            break;

        case 2:
            HoiMau(hpValue);
            break;

        case 3:
            SinhMiniEnemy();
            break;

        case 4:
            DichChuyen();
            break;
    }
}

private void SuDungSkill()
{
    nextSkillTime = Time.time + skillCooldown;
    ChonSkillNgauNhien();
}

}
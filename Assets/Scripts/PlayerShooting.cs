using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Giữ nguyên class Guns để quản lý vị trí súng và hiệu ứng
[System.Serializable]
public class Guns
{
    public GameObject rightGun, leftGun, centralGun;
    [HideInInspector] public ParticleSystem leftGunVFX, rightGunVFX, centralGunVFX; 
}

public class PlayerShooting : MonoBehaviour {

    public static PlayerShooting instance;

    [Header("General Settings")]
    [Tooltip("Tốc độ bắn: số càng lớn bắn càng nhanh")]
    public float fireRate = 3f; // Chỉnh mặc định là 3 cho dễ bắn

    // THAY ĐỔI 1: Dùng Mảng (Array) để chứa nhiều loại đạn khác nhau
    [Header("Projectile Types")]
    [Tooltip("Kéo các Prefab đạn vào đây (Element 0: Laser, Element 1: Rocket...)")]
    public GameObject[] projectileOptions; 
    private int currentProjectileIndex = 0; // Biến theo dõi loại đạn đang dùng

    [Header("Weapon Power")]
    [Tooltip("Cấp độ vũ khí hiện tại (1-4)")]
    [Range(1, 4)]       
    public int weaponPower = 1; 
    [HideInInspector] public int maxweaponPower = 4; 

    public Guns guns;
    [HideInInspector] public float nextFire;
    bool shootingIsActive = true; 

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        // Lấy component ParticleSystem nếu có (tránh lỗi null nếu quên gắn)
        if(guns.leftGun) guns.leftGunVFX = guns.leftGun.GetComponent<ParticleSystem>();
        if(guns.rightGun) guns.rightGunVFX = guns.rightGun.GetComponent<ParticleSystem>();
        if(guns.centralGun) guns.centralGunVFX = guns.centralGun.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        // THAY ĐỔI 2: Thêm phím tắt để đổi loại đạn (Phím C)
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeBulletType();
        }

        if (shootingIsActive)
        {
            if (Time.time > nextFire)
            {
                MakeAShot();                                                         
                nextFire = Time.time + 1 / fireRate;
            }
        }
    }

    // Hàm đổi loại đạn
    void ChangeBulletType()
    {
        if (projectileOptions.Length > 1)
        {
            currentProjectileIndex++;
            if (currentProjectileIndex >= projectileOptions.Length)
            {
                currentProjectileIndex = 0; // Quay lại loại đầu tiên
            }
            Debug.Log("Đã đổi sang loại đạn: " + projectileOptions[currentProjectileIndex].name);
        }
    }

    // Method thực hiện bắn
    void MakeAShot() 
    {
        // Kiểm tra xem có đạn trong mảng không để tránh lỗi
        if (projectileOptions.Length == 0) return;

        // Lấy loại đạn hiện tại dựa trên Index
        GameObject currentProjectile = projectileOptions[currentProjectileIndex];

        switch (weaponPower) 
        {
            case 1: // Bắn 1 tia giữa
                CreateShot(currentProjectile, guns.centralGun.transform.position, Vector3.zero);
                if(guns.centralGunVFX) guns.centralGunVFX.Play();
                break;

            case 2: // Bắn 2 tia hai bên
                CreateShot(currentProjectile, guns.rightGun.transform.position, Vector3.zero);
                if(guns.leftGunVFX) guns.leftGunVFX.Play();
                CreateShot(currentProjectile, guns.leftGun.transform.position, Vector3.zero);
                if(guns.rightGunVFX) guns.rightGunVFX.Play();
                break;

            case 3: // Bắn 3 tia (1 giữa + 2 xéo nhẹ)
                CreateShot(currentProjectile, guns.centralGun.transform.position, Vector3.zero);
                CreateShot(currentProjectile, guns.rightGun.transform.position, new Vector3(0, 0, -5)); // Xoay -5 độ
                if(guns.leftGunVFX) guns.leftGunVFX.Play();
                CreateShot(currentProjectile, guns.leftGun.transform.position, new Vector3(0, 0, 5));   // Xoay 5 độ
                if(guns.rightGunVFX) guns.rightGunVFX.Play();
                break;

            case 4: // Bắn 5 tia (xòe rộng)
                CreateShot(currentProjectile, guns.centralGun.transform.position, Vector3.zero);
                CreateShot(currentProjectile, guns.rightGun.transform.position, new Vector3(0, 0, -5));
                if(guns.leftGunVFX) guns.leftGunVFX.Play();
                CreateShot(currentProjectile, guns.leftGun.transform.position, new Vector3(0, 0, 5));
                if(guns.rightGunVFX) guns.rightGunVFX.Play();
                CreateShot(currentProjectile, guns.leftGun.transform.position, new Vector3(0, 0, 15));  // Xoay 15 độ
                CreateShot(currentProjectile, guns.rightGun.transform.position, new Vector3(0, 0, -15)); // Xoay -15 độ
                break;
        }
    }

    // Hàm tạo đạn (đổi tên cho gọn và dễ hiểu)
    void CreateShot(GameObject bulletPrefab, Vector3 pos, Vector3 rot) 
    {
        Instantiate(bulletPrefab, pos, Quaternion.Euler(rot));
    }
}
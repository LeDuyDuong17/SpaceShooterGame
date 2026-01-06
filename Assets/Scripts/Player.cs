using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject destructionFX;
    public static Player instance;

    private bool isInvincible = false;     
    private SpriteRenderer spriteRenderer; 

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void GetDamage(int damage)
    {
        if (isInvincible) 
            return;

        if (LevelController.instance != null)
        {
            LevelController.instance.LoseLife();

            if (LevelController.instance.playerLives <= 0)
            {
                Destruction();
            }
            else
            {
                StartCoroutine(InvincibilityRoutine());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyProjectile"))
        {
            GetDamage(1);
            if(other.CompareTag("EnemyProjectile"))
                Destroy(other.gameObject); 
        }
    }

    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;    
        float duration = 2.0f;  
        float blinkTime = 0.15f; 
        float timer = 0;

        while (timer < duration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkTime);
            timer += blinkTime;
        }

        spriteRenderer.enabled = true;
        isInvincible = false;
    }

    void Destruction()
    {
        if(destructionFX != null)
            Instantiate(destructionFX, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }
}
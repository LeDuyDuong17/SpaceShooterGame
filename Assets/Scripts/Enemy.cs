using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour 
{
    [Header("Enemy Stats")]
    public int health;
    public int scoreValue = 10; // Điểm số nhận được khi diệt quái này

    [Header("Prefabs")]
    public GameObject Projectile;
    public GameObject destructionVFX;
    public GameObject hitEffect;
    
    [HideInInspector] public int shotChance; 
    [HideInInspector] public float shotTimeMin, shotTimeMax; 

    private void Start()
    {
        Invoke("ActivateShooting", Random.Range(shotTimeMin, shotTimeMax));
    }

    void ActivateShooting() 
    {
        if (Random.value < (float)shotChance / 100)                             
        {                         
            Instantiate(Projectile, gameObject.transform.position, Quaternion.identity);             
        }
    }

    public void GetDamage(int damage) 
    {
        health -= damage;           
        if (health <= 0)
            Destruction();
        else
            if(hitEffect != null) Instantiate(hitEffect, transform.position, Quaternion.identity, transform);
    }    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Projectile.GetComponent<Projectile>() != null)
                Player.instance.GetDamage(Projectile.GetComponent<Projectile>().damage);
            else
                Player.instance.GetDamage(1);
        }
    }

    void Destruction()                           
    {        
        // Cộng điểm trước khi hủy object
        if (LevelController.instance != null)
        {
            LevelController.instance.AddScore(scoreValue);
        }

        if (destructionVFX != null)
            Instantiate(destructionVFX, transform.position, Quaternion.identity); 
            
        Destroy(gameObject);
    }
}
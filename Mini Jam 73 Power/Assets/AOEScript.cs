using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEScript : MonoBehaviour
{
    private PlayerScript playerScript;
    public float damage;
    public float duration = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerScript>();
    }
    private void Update()
    {
        duration -= Time.deltaTime;
        if (duration < 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            float damageStep = damage * Time.deltaTime;
            playerScript.CalculatePowerDrain(damageStep);
            collision.collider.GetComponent<EnemyScript>().TakeDamage(damageStep);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            float damageStep = damage * Time.deltaTime;
            playerScript.CalculatePowerDrain(damageStep);
            EnemyScript enemyScript = collision.gameObject.GetComponent<EnemyScript>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damageStep);
            }
        }
    }

}

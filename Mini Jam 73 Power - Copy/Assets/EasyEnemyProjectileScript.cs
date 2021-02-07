using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyEnemyProjectileScript : MonoBehaviour
{
    public int damage;
    public int speed;
    private GameObject player;
    private PlayerScript playerScript;
    public BoxCollider2D boxCollider;
    private void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerScript>();
    }
    private void Update()
    {
        transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        var hits = Physics2D.BoxCastAll(transform.position, boxCollider.size, 0, Vector2.zero);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.name.Contains("Obstacle"))
            {
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            playerScript.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

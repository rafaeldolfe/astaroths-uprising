using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileScript : MonoBehaviour
{
    private GlobalEventManager gem;

    public float damage;
    public float speed;
    private GameObject player;
    private PlayerScript playerScript;
    public BoxCollider2D boxCollider;
    private GameObject latestCollisionObject;

    void Awake()
    {
        List<Type> depTypes = ProgramUtils.GetMonoBehavioursOnType(this.GetType());
        List<MonoBehaviour> deps = new List<MonoBehaviour>
            {
                (gem = FindObjectOfType(typeof(GlobalEventManager)) as GlobalEventManager),
            };
        if (deps.Contains(null))
        {
            throw ProgramUtils.DependencyException(deps, depTypes);
        }
        gem.StartListening("PlayerProjectileHitSuccess", PlayEffects);
        gem.StartListening("PlayerProjectileHitFail", DontPlayEffects);
    }

    private void OnDestroy()
    {
        gem.StopListening("PlayerProjectileHitSuccess", PlayEffects);
        gem.StopListening("PlayerProjectileHitFail", DontPlayEffects);
    }

    private void DontPlayEffects(GameObject projectile, List<object> arg2)
    {
        if (latestCollisionObject != projectile)
        {
            return;
        }
        Destroy(gameObject);
    }

    private void PlayEffects(GameObject projectile, List<object> arg2)
    {
        if (latestCollisionObject != projectile)
        {
            return;
        }
        Destroy(gameObject);
        // TODO ADD PARTICLE EFFECTS!
        return;
    }

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
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.CompareTag("Enemy"))
        {
            latestCollisionObject = collision.collider.gameObject;
            gem.TriggerEvent("PlayerProjectileCollidedWith", collision.collider.gameObject, new List<object> { damage });
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Triggered...");
    }
}

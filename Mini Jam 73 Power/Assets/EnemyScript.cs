using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyScript : MonoBehaviour
{
    private GlobalEventManager gem;

    public GameObject healthBarPrefab;
    protected GameObject healthBar;
    protected Transform healthBarScaler;
    protected GameObject player;

    protected State state = State.Idling;
    public enum State
    {
        Chasing,
        Idling
    }
    public float speed = 1f;
    public float maxHealth;
    public float health;
    public float HEALTH_BAR_OFFSET;
    protected void Awake()
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
        gem.StartListening("PlayerProjectileCollidedWith", PlayerHitMe);
    }

    protected void OnDestroy()
    {
        Destroy(healthBar);
        gem.StopListening("PlayerProjectileCollidedWith", PlayerHitMe);
    }

    protected void Start()
    {
        player = GameObject.Find("Player");
        GameObject healthBar = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
        this.healthBar = healthBar;
        this.healthBarScaler = healthBar.transform.GetChild(1);
    }
    private void PlayerHitMe(GameObject target, List<object> parameters)
    {
        if (gameObject != target)
        {
            return;
        }
        float damage = ProgramUtils.GetParameter<float>(parameters, 0);
        StartCoroutine(StartDrainingHealth(damage));
        gem.TriggerEvent("PlayerProjectileHitSuccess", gameObject, new List<object> { damage });
    }

    private IEnumerator StartDrainingHealth(float damage)
    {
        float damageTakenAtm = 0;
        float damageStep = damage / Constants.PROJECTILE_DRAIN_TIME;
        while (damageTakenAtm < damage)
        {
            yield return new WaitForEndOfFrame();
            this.health -= damageStep * Time.deltaTime;
            damageTakenAtm += damageStep * Time.deltaTime;
        }
    }

    internal void TakeDamage(float damage)
    {
        this.health -= damage;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.transform.position = transform.position + new Vector3(0, HEALTH_BAR_OFFSET, 0);
        healthBarScaler.localScale = new Vector3(this.health / maxHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }
    protected void Update()
    {
        if (health < 0)
        {
            Destroy(gameObject);
            return;
        }
        UpdateHealthBar();
        //transform.LookAt(targetPosFlattened);
    }
    protected IEnumerator RandomChaseMovement()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            float time = UnityEngine.Random.Range(0f, 3.0f);
            int mult = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
            yield return MoveDirectionForTime(mult, time);
        }
    }
    private IEnumerator MoveDirectionForTime(int multiplier, float time)
    {
        float startTime = Time.time;
        while (startTime + time > Time.time)
        {
            while (state != State.Chasing)
            {
                yield return new WaitForSeconds(1);
            }
            yield return new WaitForEndOfFrame();
            Vector3 dir = player.transform.position - transform.position;
            Debug.DrawLine(transform.position, player.transform.position, Color.blue, 3);
            Vector2 perp = PerpendicularClockwise(new Vector2(dir.x, dir.y)).normalized;
            Debug.DrawRay(transform.position, perp, Color.red, 5);
            transform.position = transform.position + new Vector3(perp.x, perp.y) * speed * multiplier * Time.deltaTime;
        }
        yield return null;
    }
    public Vector2 PerpendicularClockwise(Vector2 vector2)
    {
        return new Vector2(vector2.y, -vector2.x);
    }
}

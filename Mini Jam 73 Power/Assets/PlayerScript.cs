using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float health;
    public float power;

    private Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public float speed;

    public TextMeshProUGUI healthNumber;
    public TextMeshProUGUI powerNumber;

    private GlobalEventManager gem;
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
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gem.StartListening("PlayerProjectileHitSuccess", BeginDrain);
        healthNumber = GameObject.Find("UIHealthNumber").GetComponent<TextMeshProUGUI>();
        powerNumber = GameObject.Find("UIPowerNumber").GetComponent<TextMeshProUGUI>();
    }
    private void OnDestroy()
    {
        gem.StopListening("PlayerProjectileHitSuccess", BeginDrain);
    }
    private void BeginDrain(GameObject enemy, List<object> parameters)
    {
        float damage = ProgramUtils.GetParameter<float>(parameters, 0);
        StartCoroutine(StartDrainingHealth(enemy, damage));
    }
    private IEnumerator StartDrainingHealth(GameObject enemy, float damage)
    {
        float damageDrained = 0;
        float damageStep = damage / Constants.PROJECTILE_DRAIN_TIME;
        while (damageDrained < damage && enemy != null)
        {
            yield return new WaitForEndOfFrame();
            CalculatePowerDrain(damageStep * Time.deltaTime);
            damageDrained += damageStep * Time.deltaTime;
        }
    }

    public void CalculatePowerDrain(float v)
    {
        this.health += 0.20f * v;
        this.power += 0.05f * v;
        MapManager.PlayerProgress += 0.10f * v;
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        healthNumber.text = Mathf.RoundToInt(this.health).ToString();
        powerNumber.text = Mathf.RoundToInt(this.power).ToString();
    }

    private void Update()
    {
        if (animator.GetBool("dead"))
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            animator.SetBool("dead", true);
            return;
        }
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            animator.SetBool("walking", true);
        }
        else
        {
            animator.SetBool("walking", false);
        }
        if (horizontal < 0)
        {
            spriteRenderer.flipX = true;
        }
        if (horizontal > 0)
        {
            spriteRenderer.flipX = false;
        }

        //rigidbody.AddForce(new Vector2(horizontal* Time.deltaTime * speed, vertical * Time.deltaTime * speed));

        transform.position = transform.position + new Vector3(horizontal * Time.deltaTime * speed, vertical * Time.deltaTime * speed);
    }

    internal void TakeDamage(float damage)
    {
        health -= damage;
        UpdateTexts();
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log($"triggered with : {other.name}");
    }
}

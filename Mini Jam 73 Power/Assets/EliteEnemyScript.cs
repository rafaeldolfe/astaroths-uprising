using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteEnemyScript : EnemyScript
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public float cooldown = 0.0f;
    public float cooldownReset = 2.0f;
    public float damage = 1000.0f;
    private float originalSpeed;
    private float rampUpSpeed = 1.0f;

    private void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }
    private void OnDestroy()
    {
        base.OnDestroy();
    }
    void Start()
    {
        base.Start();
        StartCoroutine(RandomWalking());
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        originalSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        transform.eulerAngles = new Vector3(0, 0, 0);
        if (cooldown <= 0)
        {
            cooldown = 0;
        }
        else
        {
            cooldown -= Time.deltaTime;
        }
        if (cooldown <= 0 && Vector3.Distance(player.transform.position, transform.position) < 4)
        {
            cooldown = cooldownReset;
            StartCoroutine(Attack());
        }
        if (Vector3.Distance(player.transform.position, transform.position) < 10)
        {
            if (speed < 3)
            {
                speed += Time.deltaTime * 0.125f;
            }
            if (state == State.Idling)
            {
                state = State.Chasing;
            }
            if (player.transform.position.x > transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
            if (Math.Abs(player.transform.position.x - transform.position.x) > 1 || Math.Abs(player.transform.position.y - transform.position.y) > 2.5f)
            {
                animator.SetBool("walking", true);
                //move if distance from target is greater than 1
                Vector3 dirToPlayer = transform.position - player.transform.position;
                transform.position = transform.position - dirToPlayer.normalized * speed * Time.deltaTime;
            }
        }
        else
        {
            state = State.Idling;
            speed = originalSpeed;
        }
    }
    private IEnumerator Attack()
    {
        animator.SetTrigger("attack");
        yield return new WaitForSeconds(0.25f);
        Collider2D[] colliders;
        if (transform.position.x <= player.transform.position.x)
        {
            colliders = Physics2D.OverlapBoxAll(transform.position + new Vector3(0.5f, 0, 0), new Vector2(1, 3.5f), 0);
            var test = GameObject.CreatePrimitive(PrimitiveType.Cube);
            test.transform.position = transform.position + new Vector3(0.5f, 0, 0);
            test.transform.localScale = new Vector3(1, 3.5f, 0);
        }
        else
        {
            colliders = Physics2D.OverlapBoxAll(transform.position + new Vector3(-0.5f, 0, 0), new Vector2(1, 3.5f), 0);
            var test = GameObject.CreatePrimitive(PrimitiveType.Cube);
            test.transform.position = transform.position + new Vector3(-0.5f, 0, 0);
            test.transform.localScale = new Vector3(1, 3.5f, 0);
        }

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                collider.gameObject.GetComponent<PlayerScript>().TakeDamage(damage);
                break;
            }
        }
    }
    private IEnumerator RandomWalking()
    {
        while (true)
        {
            animator.SetBool("walking", false);
            yield return new WaitForSeconds(3);
            Vector2 vec = ProgramUtils.GetRandomDirectionNormalized();
            float time = 5;
            yield return RandomDirectionWalk(vec, time);
        }
    }
    private IEnumerator RandomDirectionWalk(Vector2 vec, float time)
    {
        if (vec.x < 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
        float startTime = Time.time;
        while (startTime + time > Time.time)
        {
            if (state != State.Idling)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
            animator.SetBool("walking", true);
            transform.position = transform.position + new Vector3(vec.x, vec.y) * speed * Time.deltaTime;
        }
        yield return null;
    }
}


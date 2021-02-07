using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyEnemyScript : EnemyScript
{
    public GameObject projectile;
    private Coroutine Shooting;
    private Animator animator;
    void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        base.OnDestroy();
    }

    private void Start()
    {
        base.Start();
        StartCoroutine(RandomWalking());
        StartCoroutine(RandomChaseMovement());
    }

    private void Update()
    {
        base.Update();
        if (Vector3.Distance(player.transform.position, transform.position) < 10)
        {
            if (state == State.Idling)
            {
                state = State.Chasing;
                StartCoroutine(ShootProjectiles());
            }
            transform.eulerAngles = new Vector3(0, 0, -transform.eulerAngles.z);
            transform.LookAt(player.transform.position, transform.up);
            transform.Rotate(new Vector3(0, -90, 0), Space.Self);//correcting the original rotation 
        }
        else
        {
            state = State.Idling;
        }
        if (Vector3.Distance(player.transform.position, transform.position) < 4)
        {
            //move if distance from target is greater than 1
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
    }
    private IEnumerator RandomWalking()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            Vector2 vec = ProgramUtils.GetRandomDirectionNormalized();
            float time = UnityEngine.Random.Range(1.0f, 3.0f);
            yield return RandomDirectionWalk(vec, time);
        }
    }
    private IEnumerator RandomDirectionWalk(Vector2 vec, float time)
    {
        CustomLookAt(new Vector2(transform.position.x, transform.position.y) + vec);
        float startTime = Time.time;
        while (startTime + time > Time.time)
        {
            if (state != State.Idling)
            {
                yield return new WaitForSeconds(3);
            }
            yield return new WaitForEndOfFrame();
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        yield return null;
    }
    public IEnumerator ShootProjectiles()
    {
        while (state == State.Chasing)
        {
            yield return new WaitForSeconds(0.75f);
            animator.SetTrigger("shoot");
            yield return new WaitForSeconds(0.25f);
            ShootProjectile();
        }
    }

    private void ShootProjectile()
    {
        GameObject projectile = Instantiate(this.projectile, transform.position, transform.rotation);
        projectile.transform.eulerAngles = new Vector3(0, 0, -transform.eulerAngles.z);
        projectile.transform.LookAt(player.transform.position, transform.up);
        projectile.transform.Rotate(new Vector3(0, -90, 0), Space.Self);//correcting the original rotation 
    }
    private void CustomLookAt(Vector3 target)
    {
        transform.eulerAngles = new Vector3(0, 0, -transform.eulerAngles.z);
        transform.LookAt(target, transform.up);
        transform.Rotate(new Vector3(0, -90, 0), Space.Self);//correcting the original rotation 
    }
}

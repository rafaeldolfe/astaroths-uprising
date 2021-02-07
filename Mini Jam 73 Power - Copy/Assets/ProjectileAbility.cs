using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAbility : MonoBehaviour
{
    private GlobalEventManager gem;
    private PlayerScript ps;
    private Animator animator;

    public float cooldown;
    public float cooldownReset;
    public GameObject projectile;
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
        ps = GetComponent<PlayerScript>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (animator.GetBool("dead"))
        {
            return;
        }
        if (cooldown <= 0)
        {
            cooldown = 0;
        }
        else
        {
            cooldown -= Time.deltaTime;
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            AttemptToShootDrainProjectile();
        }
    }

    private void AttemptToShootDrainProjectile()
    {
        if (cooldown > 0)
        {
            return;
        }
        animator.SetTrigger("Projectile");
        StartCoroutine(ShootProjectile());
        cooldown = cooldownReset;
    }
    private IEnumerator ShootProjectile()
    {
        yield return new WaitForSeconds(0.25f);
        GameObject clone = Instantiate(projectile, transform.position, Quaternion.identity);


        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.farClipPlane;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        worldPosition.z = 0;

        CustomLookAt(clone.transform, worldPosition);
        PlayerProjectileScript script = clone.GetComponent<PlayerProjectileScript>();
        script.damage = ps.power * Constants.PROJECTILE_DAMAGE_POWER_RATIO;
    }
    private void CustomLookAt(Transform trans, Vector3 target)
    {
        trans.eulerAngles = new Vector3(0, 0, -trans.eulerAngles.z);
        trans.LookAt(target, trans.up);
        trans.Rotate(new Vector3(0, -90, 0), Space.Self);//correcting the original rotation 
    }
}

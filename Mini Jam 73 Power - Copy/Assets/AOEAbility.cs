using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEAbility : MonoBehaviour
{
    private GlobalEventManager gem;
    private PlayerScript ps;
    private Animator animator;

    public float cooldown;
    public float cooldownReset = 5.0f;
    public GameObject AOE;
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
    bool debug = true;
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
        if ((ps.power > 100 || debug) && Input.GetMouseButtonDown(1))
        {
            AttemptToAOEDrain();
        }
    }

    private void AttemptToAOEDrain()
    {
        if (cooldown > 0)
        {
            return;
        }
        animator.SetTrigger("AOE");
        GameObject clone = Instantiate(AOE, transform);

        AOEScript script = clone.GetComponent<AOEScript>();
        script.damage = ps.power * Constants.AOE_DAMAGE_POWER_RATIO;
        cooldown = cooldownReset;
    }
    private void CustomLookAt(Transform trans, Vector3 target)
    {
        trans.eulerAngles = new Vector3(0, 0, -trans.eulerAngles.z);
        trans.LookAt(target, trans.up);
        trans.Rotate(new Vector3(0, -90, 0), Space.Self);//correcting the original rotation 
    }
}

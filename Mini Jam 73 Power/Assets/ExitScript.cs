using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    public float health = 10000;

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
        gem.StartListening("PlayerProjectileCollidedWith", PlayerHitMe);

    }

    private void PlayerHitMe(GameObject target, List<object> parameters)
    {
        if (gameObject != target)
        {
            return;
        }
        float damage = ProgramUtils.GetParameter<float>(parameters, 0);
        this.health -= damage;
        gem.TriggerEvent("PlayerProjectileHitFail", gameObject, new List<object> { damage });
    }

    private void Update()
    {
        if (health < 0)
        {
            Destroy(gameObject);
        }
    }

    internal void TakeDamage(float damage)
    {
        if (damage < 1000)
        {
            return;
        }
        this.health -= damage;
    }
}

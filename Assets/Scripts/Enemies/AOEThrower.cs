using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEThrower : ShootingEnemy
{
    protected override void Shoot(Vector3 targetPosition)
    {
        isShooting = true;

        //wait for attack time

        StartCoroutine(LaunchAOEAttack(targetPosition));
    }

    protected override void PostShootAction()
    {
        //wait a bit
        StartCoroutine(Wait(attackCooldown));
    }

    protected override void ExecuteAfterWaiting()
    {
        isShooting = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeEnemy : ShootingEnemy
{
    protected override void ExecuteAfterWaiting()
    {
        isShooting = false;
    }

    protected override void PostShootAction()
    {
        //wait a bit
        StartCoroutine(Wait(attackCooldown));
    }

    protected override void Shoot(Vector3 targetPosition)
    {
        isShooting = true;
        StartCoroutine(LaunchAOEAttack(this.transform.position));
    }

}

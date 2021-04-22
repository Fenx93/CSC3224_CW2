using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEnemy : ShootingEnemy
{
    protected override void PostShootAction()
    {
        //Self-destruct
        Object.Destroy(this.gameObject);
    }

    protected override void Shoot(Vector3 targetPosition)
    {
        isShooting = true;
        //wait for attack time
        StartCoroutine(LaunchAOEAttack(this.transform.position));
    }

    //pretty useless here - bomb shoots once. HEHE.
    protected override void ExecuteAfterWaiting() { }
}

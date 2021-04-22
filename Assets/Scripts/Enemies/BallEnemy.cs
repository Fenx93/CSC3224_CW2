using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEnemy : ShootingEnemy
{
    [SerializeField]
    private float attackPreparationTime = 1f;
    private Vector3 _targetPosition;

    private bool attacking = false;

    protected override void Shoot(Vector3 targetPosition)
    {
        if (!attacking)
        {
            attacking = true;
            _targetPosition = targetPosition;
            StartCoroutine(Wait(attackPreparationTime));
        }
    }

    private IEnumerator RollTowardsTarget(Vector3 targetPosition)
    {
        Vector3 startingPos = transform.position;

        float elapsedTime = 0;

        while (elapsedTime < attackTime)
        {
            transform.position = Vector3.Lerp(startingPos, targetPosition, (elapsedTime / attackTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        PostShootAction();
    }
    protected override void PostShootAction()
    {
        StartCoroutine(WaitAndShoot(attackCooldown));
    }
    protected override void ExecuteAfterWaiting()
    {
        Debug.Log("Continuing shooting");
        StartCoroutine(RollTowardsTarget(_targetPosition));
    }

    private IEnumerator WaitAndShoot(float time)
    {
        Debug.Log("Waiting started");
        float currentTime = 0.0f;
        do
        {
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);
        Debug.Log("Waiting ended");
        attacking = false;
    }

}

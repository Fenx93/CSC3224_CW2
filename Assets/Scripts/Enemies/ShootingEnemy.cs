using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootingEnemy : Enemy
{
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackTime = 1.0f;
    [SerializeField] protected GameObject aoePrefab;
    [SerializeField] protected float aoeSize = 3f;
    [SerializeField] protected float aoeGrowTime = 1f;

    [SerializeField] protected float shootingCooldown = 1f;

    protected bool isShooting = false;

    private GameObject associatedAOE;
    // Update is called once per frame
    void Update()
    {
        if (!gameController.isPaused)
        {
            if (!isShooting)
            {
                //keep finding closest target
                target = FindClosestTarget();
                var distance = Vector2.Distance(target.transform.position, transform.position);

                //if in the range and in the arena - shoot
                if (distance <= attackRange && IsInArena())
                {
                    Shoot(target.transform.position);
                }
                //if out of range or outside the arena - get closer
                else
                {
                    float step = speed * Time.deltaTime; // calculate distance to move
                    transform.position = Vector2.MoveTowards(transform.position, target.transform.position, step);
                }
            }
        }
    }

    private bool IsInArena()
    {
        var pos = this.transform.position;
        return Mathf.Abs(pos.x) < gameController.arenaWidth 
            && Mathf.Abs(pos.y) < gameController.arenaHeight;
    }

    protected abstract void Shoot(Vector3 targetPosition);

    protected abstract void PostShootAction();

    protected IEnumerator LaunchAOEAttack(Vector3 targetPosition)
    {
        var aoe = Instantiate(aoePrefab, targetPosition, Quaternion.identity);
        associatedAOE = aoe;
        Vector3 originalScale = aoe.transform.localScale;
        Vector3 targetScale = new Vector3(aoeSize, aoeSize, 1);

        float currentTime = 0.0f;

        do
        {
            aoe.transform.localScale = Vector3.Lerp(originalScale, targetScale, currentTime / aoeGrowTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= aoeGrowTime);

        aoe.GetComponent<AOE>().Explode();
        Destroy(aoe);
        PostShootAction();
    }

    protected override void Die()
    {
        // TO-DO: change to independant aoe, which works as normal even if the caster has died
        Destroy(associatedAOE);
        base.Die();
    }
}

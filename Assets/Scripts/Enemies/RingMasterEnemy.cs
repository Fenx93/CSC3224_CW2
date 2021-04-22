using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingMasterEnemy : ShootingEnemy
{
    [SerializeField]
    private GameObject closeAttack, farAttack;
    private Color attackPatternColor;

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
        RandomlyChooseAttackPattern();
        StartCoroutine(LaunchCloseAOEAttack(this.transform.position));
    }

    private void RandomlyChooseAttackPattern()
    {
        var random = Random.Range(0, 10);
        aoePrefab = random < 5 ? closeAttack : farAttack;
        attackPatternColor = random < 5 ? Color.cyan : Color.magenta;
        spriteRenderer.color = attackPatternColor;
    }


    private IEnumerator LaunchCloseAOEAttack(Vector3 targetPosition)
    {
        var aoe = Instantiate(aoePrefab, targetPosition, Quaternion.identity);
        aoe.transform.localScale = aoe.transform.localScale * attackRange * aoeSize;
        Color originalColor = Color.white;

        float currentTime = 0.0f;
        var aoeSpriteRenderer = aoe.GetComponent<SpriteRenderer>();

        do
        {
            aoeSpriteRenderer.color = Color.Lerp(originalColor, attackPatternColor, currentTime / aoeGrowTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= aoeGrowTime);

        Destroy(aoe);
        PostShootAction();
    }
}

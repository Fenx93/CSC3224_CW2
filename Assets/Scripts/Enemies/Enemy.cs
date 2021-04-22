using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    protected int hp, attack;
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float attackCooldown = 1f;
    [SerializeField]
    protected int score = 100;

    protected GameplayController gameController;
    protected AudioController audioController;
    protected GameObject target;
    protected SpriteRenderer spriteRenderer;
    private Color baseColor;

    protected void Awake()
    {
        audioController = FindObjectOfType<AudioController>();
        gameController = FindObjectOfType<GameplayController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.material.color;
    }

    protected GameObject FindClosestTarget()
    {
        return Vector2.Distance(gameController.playerOne.transform.position, transform.position) < Vector2.Distance(gameController.playerTwo.transform.position, transform.position) ?
            gameController.playerOne : gameController.playerTwo;
    }


    protected IEnumerator Wait(float time)
    {
        float currentTime = 0.0f;
        do
        {
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);
        ExecuteAfterWaiting();
    }

    protected abstract void ExecuteAfterWaiting();

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            player.TakeDamage();
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Line"))
        {
            TakeDamage();
        }
    }

    //pass damage value to this method?
    protected void TakeDamage()
    {
        hp--;
        if (hp <= 0)
        {
            Die();
        }
        else
        {
            audioController.EnemyDamagedSound();
            StartCoroutine(DamagePulsing());
        }
    }

    protected virtual void Die()
    {
        audioController.EnemyDestroyedSound();
        gameController.AddScore(score);
        Destroy(this.gameObject);
    }

    protected IEnumerator DamagePulsing()
    {
        float currentTime = 0.0f;
        var colorStart = Color.red;
        var colorEnd = baseColor;
        do
        {
            currentTime += Time.deltaTime;
            spriteRenderer.material.color = Color.Lerp(colorStart, colorEnd, currentTime);
            yield return null;
        } while (spriteRenderer.material.color != colorEnd);
    }
}

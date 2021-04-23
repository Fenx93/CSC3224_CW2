using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private int playerId;

    private Vector2 movement;
    private GameplayController gameController;
    private SpriteRenderer spriteRenderer;
    private AudioController audioController;
    private Color baseColor;

    private bool isInvulnerable = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.material.color;
        gameController = FindObjectOfType<GameplayController>();
        audioController = FindObjectOfType<AudioController>();
    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void Update()
    {
        if (gameController.canPlayersMove)
        {
            if (playerId == 1)
            {
                movement.x = Input.GetAxis("Horizontal1");
                movement.y = Input.GetAxis("Vertical1");
            }
            else
            {
                movement.x = Input.GetAxis("Horizontal2");
                movement.y = Input.GetAxis("Vertical2");
            }
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed);
    }

    public void TakeDamage()
    {
        if (!gameController.playersInvulnerable && !isInvulnerable)
        {
            audioController.PlayerDamageSound();
            spriteRenderer.material.color = Color.red;
            gameController.PlayerTakesDamage();
            isInvulnerable = true;
            StartCoroutine(Invulnerability());
        }
    }


    protected IEnumerator Invulnerability()
    {
        float currentTime = 0.0f;
        var colorStart = baseColor;
        var colorEnd = Color.red;
        do
        {
            currentTime += Time.deltaTime;
            spriteRenderer.material.color = Color.Lerp(colorStart, colorEnd, Mathf.PingPong(currentTime * 2, 1));
            if (spriteRenderer.material.color == colorEnd)
            {
                var colTemp = colorStart;
                colorStart = colorEnd;
                colorEnd = colTemp;
            }
            yield return null;
        } while (currentTime <= gameController.playerinvulnerabilityTime);

        spriteRenderer.material.color = baseColor;
        isInvulnerable = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : Enemy
{
    protected override void ExecuteAfterWaiting()
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        if (!gameController.isPaused)
        {
            target = FindClosestTarget();
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, step);
        }
    }
}

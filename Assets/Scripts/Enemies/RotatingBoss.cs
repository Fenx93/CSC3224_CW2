using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingBoss : Enemy
{
    protected override void ExecuteAfterWaiting()
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        //if (!gameController.isPaused)
        //{
            //target = FindClosestTarget();
            float step = speed * Time.deltaTime * 4; // calculate distance to move
            //transform.position = Vector2.MoveTowards(transform.position, target.transform.position, step);
            transform.Rotate(Vector3.back, step, Space.World);
        //}
    }
}

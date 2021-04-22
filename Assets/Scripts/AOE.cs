using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE : MonoBehaviour
{
    private HashSet<GameObject> _inrange = new HashSet<GameObject>();

    public void Explode()
    {
        foreach (var player in _inrange)
        {
            player.GetComponent<PlayerController>().TakeDamage();
        }
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (!c.gameObject.CompareTag("Player")) return;

        _inrange.Add(c.gameObject);
    }

    private void OnTriggerExit2D(Collider2D c)
    {
        if (!c.gameObject.CompareTag("Player")) return;
        _inrange.Remove(c.gameObject);
    }

    private bool IsInRange(GameObject go)
    {
        return go != null && _inrange.Contains(go);
    }
}

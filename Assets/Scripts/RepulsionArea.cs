using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class RepulsionArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Agent"))
            return;

        other.GetComponent<Agent>().Scare(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Agent"))
            return;

        other.GetComponent<Agent>().Unscare();
    }
}

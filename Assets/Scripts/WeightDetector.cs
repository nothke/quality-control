using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightDetector : MonoBehaviour
{
    HashSet<Rigidbody> enteredBodies = new HashSet<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
    {
        var rb = collision.rigidbody;

        if (rb)
        {
            enteredBodies.Add(rb);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        var rb = collision.rigidbody;

        if (rb)
        {
            enteredBodies.Remove(rb);
        }
    }

    public float GetTotalWeight()
    {
        float totalMass = 0;
        foreach (var body in enteredBodies)
        {
            totalMass += body.mass;
        }

        return totalMass;
    }

    //private void Update()
    //{
    //    Debug.Log(GetTotalWeight());
    //}
}

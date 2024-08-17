using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    Rigidbody _rb;
    Rigidbody rb { get { if (!_rb) _rb = GetComponent<Rigidbody>(); return _rb; } }

    public float strength = 100;

    private void OnTriggerStay(Collider other)
    {
        var otherRb = other.attachedRigidbody;

        if(otherRb)
        {
            Vector3 dir = otherRb.position - rb.position;
            float dirSq = Vector3.SqrMagnitude(dir);
            float force = strength * (1.0f / dirSq);

            otherRb.AddForce(-dir * force);
            rb.AddForce(dir * force);
        }
    }
}

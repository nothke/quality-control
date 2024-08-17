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

        if (otherRb && otherRb.isKinematic == false)
        {
            Vector3 dir = otherRb.position - transform.position;
            float dirSq = Vector3.SqrMagnitude(dir);
            float forceMagnitude = strength * (1.0f / dirSq);
            Vector3 force = dir.normalized * forceMagnitude;

            otherRb.AddForce(-force);

            if (rb)
                rb.AddForce(force);
        }
    }
}

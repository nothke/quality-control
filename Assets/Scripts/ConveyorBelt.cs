using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float speed = 1;

    List<ContactPoint> contactPoints = new List<ContactPoint>();

    private void OnCollisionStay(Collision collision)
    {
        Vector3 targetSpeed = speed * Vector3.forward;

        Vector3 vel = collision.relativeVelocity;

        int count = collision.GetContacts(contactPoints);

        if (collision.rigidbody)
        {
            for (int i = 0; i < count; i++)
            {
                
            }
        }
    }
}

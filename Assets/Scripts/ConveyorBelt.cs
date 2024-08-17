using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    Rigidbody _rb;
    Rigidbody rb { get { if (!_rb) _rb = GetComponent<Rigidbody>(); return _rb; } }

    public float speed = 1;

    Vector3 startPosition;

    private void Start()
    {
        startPosition = rb.position;
    }

    private void FixedUpdate()
    {
        rb.position = startPosition;
        Vector3 targetSpeed = speed * transform.forward;
        rb.MovePosition(rb.position + targetSpeed * Time.deltaTime);
    }
}

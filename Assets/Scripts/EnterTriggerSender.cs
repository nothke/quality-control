using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnterTriggerSender : MonoBehaviour
{
    public UnityEvent onEnterEvent;
    public UnityEvent onExitEvent;

    public Collider triggeredCollider;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered: " + other.name);
        triggeredCollider = other;
        onEnterEvent.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited: " + other.name);
        triggeredCollider = other;
        onExitEvent.Invoke();
    }
}

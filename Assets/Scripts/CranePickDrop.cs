using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CranePickDrop : MonoBehaviour
{
    public SlidingCrane crane;
    public Magnet magnet;
    public Transform dropTarget;

    public Rigidbody handlingBody;

    public float magnetStrength;

    public enum State
    {
        Idle,
        Catching,
        Tansporting,
        Finished,
    }

    public State state;

    public void OnTriggerEnterSignalReceived(EnterTriggerSender sender)
    {
        var otherRb = sender.triggeredCollider.attachedRigidbody;

        if (otherRb && otherRb.isKinematic == false && state == State.Idle)
        {
            handlingBody = otherRb;
            state = State.Catching;
        }
    }

    public void OnTriggerExitSignalReceived(EnterTriggerSender sender)
    {
        var otherRb = sender.triggeredCollider;
    }

    void Start()
    {
        magnetStrength = magnet.strength;
    }

    void Update()
    {
        if (state == State.Catching)
        {
            Debug.Assert(handlingBody, this);

            magnet.strength = magnetStrength;

            crane.testTgt = handlingBody.transform;
            if (magnet.IsCloseTo(handlingBody, 2f))
            {
                crane.testTgt = dropTarget;
                state = State.Tansporting;
            }
        }

        else if (state == State.Tansporting)
        {
            if (magnet.IsCloseToPlanar(dropTarget))
            {
                magnet.strength = 0;
                state = State.Idle;
            }
        }
    }
}

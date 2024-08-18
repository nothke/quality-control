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
        WaitingToCatch,
        Tansporting,
        Finished,
    }

    public State state;

    float timer = 0;

    public void OnTriggerEnterSignalReceived(EnterTriggerSender sender)
    {
        var otherRb = sender.triggeredCollider.attachedRigidbody;

        if (otherRb && otherRb.isKinematic == false && state == State.Idle)
        {
            handlingBody = otherRb;
            crane.testTgt = handlingBody.transform;
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
            Debug.Assert(magnet, this);

            magnet.strength = magnetStrength;

            if (magnet.IsCloseTo(handlingBody, 2f))
            {
                crane.testTgt = null;

                state = State.WaitingToCatch;
                timer = 3;
            }
        }
        else if (state == State.WaitingToCatch)
        {
            timer -= Time.deltaTime;

            if (timer < 0)
            {
                state = State.Tansporting;
                crane.testTgt = dropTarget;
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

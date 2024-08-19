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
        WaitBeforeDrop,
        Finished,
    }

    public State state;

    float timer = 0;

    public void OnTriggerEnterSignalReceived(EnterTriggerSender sender)
    {
        var otherRb = sender.triggeredCollider.attachedRigidbody;

        if (!otherRb.GetComponent<Product>())
        {
            return;
        }
        
        if (otherRb && otherRb.isKinematic == false && state == State.Idle)
        {
            handlingBody = otherRb;
            crane.targetTransform = handlingBody.transform;
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
        crane.heightMotion.AccelerateTo(1);
    }

    void Update()
    {
        if (state == State.Catching)
        {
            Debug.Assert(handlingBody, this);

            if (handlingBody == null)
            {
                state = State.Idle;
                return;
            }

            if (magnet.IsCloseToPlanar(handlingBody, 10f))
            {
                crane.heightMotion.AccelerateTo(0);
            }

            if (magnet.IsCloseTo(handlingBody, 5f))
            {
                magnet.strength = magnetStrength;
            }

            if (magnet.IsCloseTo(handlingBody, 2f))
            {
                crane.targetTransform = null;

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
                crane.targetTransform = dropTarget;
                crane.heightMotion.AccelerateTo(1);
            }
        }

        else if (state == State.Tansporting)
        {
            if (magnet.IsCloseToPlanar(dropTarget, 10f))
            {
                crane.heightMotion.AccelerateTo(0);
            }

            if (magnet.IsCloseToPlanar(dropTarget, 3f))
            {
                state = State.WaitBeforeDrop;
                timer = 3;
            }
        }
        else if (state == State.WaitBeforeDrop)
        {
            timer -= Time.deltaTime;

            if (timer < 0)
            {
                magnet.strength = 0;
                state = State.Idle;
                crane.heightMotion.AccelerateTo(1);
            }
        }
    }
}

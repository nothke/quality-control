using Nothke.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingCrane : MonoBehaviour
{
    public InertialInterpolator xMotion = InertialInterpolator.Default();
    public InertialInterpolator yMotion = InertialInterpolator.Default();

    public Transform xTransform;
    public Transform yTransform;

    public float yRange = 10;
    public float xRange = 5;

    [Range(0f, 1f)]
    public float xStart = 0.5f;
    
    public Transform testTgt;
    public Vector3 target;

    public Transform cradleTransform;
    public Rigidbody cradleRb;

    private void Start()
    {
        target = transform.position;
        xMotion.progress = xStart;
    }

    void FixedUpdate()
    {
        if (testTgt)
            target = testTgt.position;
        
        Vector3 localTarget = transform.InverseTransformPoint(target);

        Vector2 targetPlanar = new Vector2(localTarget.x, localTarget.z);

        float xTgt = Mathf.InverseLerp(-xRange, xRange, targetPlanar.x);
        float yTgt = Mathf.InverseLerp(0, yRange, targetPlanar.y);

        xMotion.AccelerateTo(xTgt);
        yMotion.AccelerateTo(yTgt);

        float x = Mathf.Lerp(-xRange, xRange, xMotion.progress);
        float y = yMotion.progress * yRange - 3;

        xMotion.Update(Time.deltaTime);
        yMotion.Update(Time.deltaTime);

        yTransform.localPosition = new Vector3(0, 0, yMotion.progress * yRange);
        xTransform.localPosition = new Vector3(x, 0, 0);

        //cradleRb.MovePosition(cradleTransform.position);
    }
}

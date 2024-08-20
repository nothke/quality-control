using Nothke.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingCrane : MonoBehaviour
{
    public InertialInterpolator xMotion = InertialInterpolator.Default();
    public InertialInterpolator yMotion = InertialInterpolator.Default();
    public InertialInterpolator heightMotion = InertialInterpolator.Default();

    public Transform xTransform;
    public Transform yTransform;

    public float yRange = 10;
    public float xRange = 5;
    public float heightRange = 10;

    public float heightMax = -0.7f;
    public float heightMin = -10;

    /// <summary>
    /// if targetTrasnform is set, it will go to it. If null it will go to targetPoint
    /// </summary>
    public Transform targetTransform;

    /// <summary>
    /// if targetTrasnform is set, it will go to it. If null it will go to targetPoint
    /// </summary>
    public Vector3 targetPoint;

    public Transform cradleTransform;
    public Rigidbody cradleRb;

    public AudioSource longMotionAudio;
    public AudioSource longMotionAudio2;
    public float longMotionPitchMult = 1;
    public AudioSource sideMotionAudio;
    public float sideMotionPitchMult = 1;
    public AudioSource heightMotionAudio;
    public float heightMotionPitchMult = 1;

    private void Start()
    {
        targetPoint = transform.position;
        xMotion.progress = 0.5f;

        

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            heightMotion.AccelerateTo(0);
        if (Input.GetKeyDown(KeyCode.F))
            heightMotion.AccelerateTo(1);

        longMotionAudio.pitch = yMotion.velocity * longMotionPitchMult;
        longMotionAudio2.pitch = yMotion.velocity * (longMotionPitchMult + 0.05f);
        sideMotionAudio.pitch = xMotion.velocity * sideMotionPitchMult;
        heightMotionAudio.pitch = heightMotion.velocity * heightMotionPitchMult;
    }

    void FixedUpdate()
    {
        if (targetTransform)
            targetPoint = targetTransform.position;


        Vector3 localTarget = transform.InverseTransformPoint(targetPoint);

        Vector2 targetPlanar = new Vector2(localTarget.x, localTarget.z);

        float xTgt = Mathf.InverseLerp(-xRange, xRange, targetPlanar.x);
        float yTgt = Mathf.InverseLerp(0, yRange, targetPlanar.y);

        xMotion.AccelerateTo(xTgt);
        yMotion.AccelerateTo(yTgt);

        float x = Mathf.Lerp(-xRange, xRange, xMotion.progress);
        float y = yMotion.progress * yRange - 3;

        xMotion.Update(Time.deltaTime);
        yMotion.Update(Time.deltaTime);
        heightMotion.Update(Time.deltaTime);

        yTransform.localPosition = new Vector3(0, 0, yMotion.progress * yRange);
        xTransform.localPosition = new Vector3(x, 0, 0);

        //cradleRb.MovePosition(cradleTransform.position);

        cradleRb.transform.localPosition = new Vector3(0, Mathf.Lerp(heightMin, heightMax, heightMotion.progress), 0);
    }
}

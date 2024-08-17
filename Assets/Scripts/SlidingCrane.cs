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

    void Start()
    {
        yMotion.AccelerateTo(1);
        xMotion.AccelerateTo(1);
    }

    // Update is called once per frame
    void Update()
    {
        xMotion.Update(Time.deltaTime);
        yMotion.Update(Time.deltaTime);

        yTransform.localPosition = new Vector3(0, 0, yMotion.progress * yRange);
        xTransform.localPosition = new Vector3(xMotion.progress *  xRange, 0, 0);
    }
}

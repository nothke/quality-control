using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nothke.Utils
{
    public class HandSway : MonoBehaviour
    {
        public float springRate = 10;
        public float softClampAngle = 30;

        Quaternion lastRotation;

        void Update()
        {
            Quaternion target = transform.parent.rotation;

            lastRotation = Quaternion.Slerp(lastRotation, target, Time.deltaTime * springRate);
            lastRotation = SoftClampRotation(target, lastRotation, softClampAngle);

            transform.rotation = lastRotation;
        }

        public static Quaternion SoftClampRotation(Quaternion origin, Quaternion target, float limitAngleDegrees)
        {
            float angle = Quaternion.Angle(origin, target);
            float softAngle = Mathf.Atan(angle * Mathf.PI / 2 / limitAngleDegrees) / Mathf.PI * 2 * limitAngleDegrees;

            return Quaternion.RotateTowards(origin, target, softAngle); // note: uses degrees
        }
    }
}
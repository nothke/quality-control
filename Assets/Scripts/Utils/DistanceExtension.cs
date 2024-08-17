using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DistanceExtension
{
    public static Vector2 Planar(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static bool IsCloseTo(this Component c1, Component c2, float range = 1f)
    {
        return Vector3.Distance(c1.transform.position, c2.transform.position) < range;
    }

    public static bool IsCloseToPlanar(this Component c1, Component c2, float range = 1f)
    {
        return Vector2.Distance(c1.transform.position.Planar(), c2.transform.position.Planar()) < range;
    }
}

using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using RigidTransform = Unity.Mathematics.RigidTransform;
using quaternion = Unity.Mathematics.quaternion;

public static class Grid
{
    public static float CellSize = 0.25f;
    public static float HalfCellSize => CellSize / 2;
    
    public static int3 ToSnappedDirection(this float3 direction)
    {
        float3 absDirection = abs(direction);
        int axisIndex = absDirection.x >= absDirection.y
            ? absDirection.x >= absDirection.z ? 0 : 2
            : absDirection.y >= absDirection.z ? 1 : 2;

        int3 snappedDirection = 0;
        snappedDirection[axisIndex] = direction[axisIndex] > 0f ? 1 : -1;
        return snappedDirection;
    }
    
    public static int3x3 ToSnappedOrientation(this quaternion rotation)
    {
        float3x3 orientation = float3x3(rotation);

        int3 snappedRight = ToSnappedDirection(orientation.c0);

        int3 upMask = (int3)(snappedRight == 0);
        int3 snappedUp = ToSnappedDirection(orientation.c1 * upMask);

        int3 forwardMask = upMask * (int3)(snappedUp == 0);
        int3 snappedForward = ToSnappedDirection(orientation.c2 * forwardMask);

        return int3x3(snappedRight, snappedUp, snappedForward);
    }

    public static float3 ToMeters(int3 size) => (float3)size * HalfCellSize;
    
    private static RigidTransform GetParentSpace(Transform t)
    {
        Transform parent = t.parent;
        GridUnit parentUnit = parent != null ? parent.GetComponentInParent<GridUnit>() : null;
        if (parentUnit == null)
        {
            return RigidTransform.identity;
        }

        Transform parentTransform = parentUnit.transform;
        RigidTransform parentSpace = RigidTransform(parentTransform.rotation, parentTransform.position);

        float3 corner = parentUnit.CenterOffset - ToMeters(parentUnit.Size);
        parentSpace.pos = transform(parentSpace, corner);
        return parentSpace;
    }
    
    public static void SnapUnit(Transform t, int3 size, float3 centerOffset)
    {
        RigidTransform parentSpace = GetParentSpace(t);

        RigidTransform rt = RigidTransform(t.rotation, t.position);

        rt = mul(inverse(parentSpace), rt);

        int3x3 snappedOrientation = rt.rot.ToSnappedOrientation();
        rt.rot = quaternion.LookRotation(snappedOrientation.c2, snappedOrientation.c1);

        float3 nudge = float3(1f);
        if (size.x % 2 == 0)
            nudge += snappedOrientation.c0;
        if (size.y % 2 == 0)
            nudge += snappedOrientation.c1;
        if (size.z % 2 == 0)
            nudge += snappedOrientation.c2;
        nudge *= 0.5f;

        float3 centerOffsetW = mul(rt.rot, centerOffset);
        float3 centerW = rt.pos + centerOffsetW;
        centerW = (round(centerW / CellSize - nudge) + nudge) * CellSize;
        rt.pos = centerW - centerOffsetW;

        rt = mul(parentSpace, rt);

        t.SetPositionAndRotation(rt.pos, rt.rot);
    }

}
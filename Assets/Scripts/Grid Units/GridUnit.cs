using UnityEngine;
using UnityEditor;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using static Grid;
using Color = UnityEngine.Color;

#if UNITY_EDITOR

[ExecuteAlways]
[SelectionBase]
[DisallowMultipleComponent]
public class GridUnit : MonoBehaviour
{
    
    public int3 Size = new (1, 1, 1);
    private Vector3 SizeInMeters => new (Size.x * CellSize, Size.y * CellSize, Size.z * CellSize);
    
    public float3 CenterOffset;

    [SerializeField, HideInInspector]
    private int3 cachedSize = new (1, 1, 1);
    [SerializeField, HideInInspector]
    private float3 cachedOffset;
    
    private bool isDirty => transform.hasChanged || 
                            !all(cachedOffset != CenterOffset) || 
                            !all(cachedSize == Size);

    public void Update()
    {
        if (!isDirty)
        {
            return;
        }
        
        SnapUnit(transform, Size, CenterOffset);

        transform.hasChanged = false;
            
        cachedOffset = CenterOffset;
        cachedSize = Size;
    }

    [DrawGizmo(GizmoType.InSelectionHierarchy)]
    private static void DrawGizmo(GridUnit unit, GizmoType type)
    {
        Transform t = unit.transform;
        quaternion rotation = t.rotation;
        var position = (float3)t.position + mul(rotation, unit.CenterOffset);
        
        Gizmos.color = Color.white;
        Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, unit.SizeInMeters);
    }
}

#endif

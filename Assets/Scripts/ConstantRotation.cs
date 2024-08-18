using UnityEngine;

public class ConstantRotation: MonoBehaviour
{
    public Vector3 Axis = Vector3.up;

    public float RotationRate = 1f;

    public void Update()
    {
        var rotation = transform.rotation;
        
        var delta = Quaternion.AngleAxis(RotationRate * Time.deltaTime, Axis);
        
        transform.SetPositionAndRotation(transform.position, rotation * delta);
    }
}
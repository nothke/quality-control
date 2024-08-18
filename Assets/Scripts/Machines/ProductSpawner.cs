using System.Collections.Generic;
using UnityEngine;

public class ProductSpawner: MonoBehaviour
{
    public List<Transform> PossibleOrientations;
    public Vector2 yRotation;
    
    public void SpawnProduct(ProductType type)
    {
        var randomIndex = Random.Range(0, PossibleOrientations.Count);
        var randomOrientation = PossibleOrientations[randomIndex];
        
        var rotation = Quaternion.AngleAxis(Random.Range(yRotation.x, yRotation.y), Vector3.up) * 
            randomOrientation.rotation;
        
        ProductType.SpawnProduct(type, transform, randomOrientation.position, rotation);
    }
}
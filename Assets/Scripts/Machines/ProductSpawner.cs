using System.Collections.Generic;
using UnityEngine;

public class ProductSpawner: MonoBehaviour
{
    public List<Transform> PossibleOrientations;
    public Vector2 yRotation;
    
    public AudioClip spawnClip;
    
    public void SpawnProduct(ProductType type)
    {
        var randomIndex = Random.Range(0, PossibleOrientations.Count);
        var randomOrientation = PossibleOrientations[randomIndex];
        
        var rotation = Quaternion.AngleAxis(Random.Range(yRotation.x, yRotation.y), Vector3.up) * 
            randomOrientation.rotation;
        
        ProductType.SpawnProduct(type, transform, randomOrientation.position, rotation);
        NAudio.Play(spawnClip, transform.position);
    }
    
    public void SpawnProduct(ProductType type, DefectType defect)
    {
        var randomIndex = Random.Range(0, PossibleOrientations.Count);
        var randomOrientation = PossibleOrientations[randomIndex];
        
        var rotation = Quaternion.AngleAxis(Random.Range(yRotation.x, yRotation.y), Vector3.up) * 
                       randomOrientation.rotation;
        
        ProductType.SpawnProduct(type, defect, transform, randomOrientation.position, rotation);
    }
}
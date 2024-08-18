using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Product Type", menuName = "Data/Product Type", order = 0)]
public class ProductType : ScriptableObject
{
    public Product Prefab;

    [Serializable]
    public struct DefectProbability
    {
        public DefectType Defect;
        [Range(0,100)]
        public int Probability;
    }
    
    public AudioClip[] normalClips;
    public AudioClip[] defectiveClips;
    
    public DefectProbability[] DefectProbabilities;
    
    public static void SpawnProduct(ProductType type, Transform parent, Vector3 position, Quaternion rotation)
    {
        var newProduct = Instantiate(type.Prefab, position, rotation, parent);
        newProduct.Type = type;
        newProduct.ApplyDefect(type.SelectDefect());
    }
    
    public DefectType SelectDefect()
    {
        float randomValue = Random.Range(0, 100);
        float sum = 0;
        
        for (var i = 0; i < DefectProbabilities.Length; i++)
        {
            var defect = DefectProbabilities[i];

            sum += defect.Probability;
            if (randomValue <= sum)
            {
                return defect.Defect;
            }
        }

        return DefectType.None;
    }

    public AudioClip SelectClip(bool defective)
    {
        var clips = defective ? defectiveClips : normalClips;

        int index = Random.Range(0, clips.Length);

        return clips[index];
    }
}
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Product Spawn Schedule", menuName = "Data/Product Spawn Schedule", order = 0)]
public class ProductSpawnSchedule : ScriptableObject
{
    [Serializable]
    public struct Product
    {
        public GameObject Prefab;
        [Range(0,100)]
        public int Probability;
    }

    public Product[] Products;
    
    public GameObject GetRandomProduct()
    {
        float randomValue = Random.Range(0, 100);
        float sum = 0;
        
        for (var i = 0; i < Products.Length; i++)
        {
            var product = Products[i];

            sum += product.Probability;
            if (randomValue <= sum)
            {
                return product.Prefab;
            }
        }

        return null;
    }
}
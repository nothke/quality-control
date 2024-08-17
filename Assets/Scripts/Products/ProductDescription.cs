using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Product Description", menuName = "Data/Product Description", order = 0)]
public class ProductDescription : ScriptableObject
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
        float randomValue = Random.value;
        float sum = 0;
        
        for (var i = 0; i < Products.Length; i++)
        {
            var product = Products[i];

            sum += product.Probability / 100f;
            if (randomValue <= sum)
            {
                return product.Prefab;
            }
        }

        return null;
    }
}
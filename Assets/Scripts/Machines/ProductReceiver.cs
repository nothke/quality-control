using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductReceiver : MonoBehaviour
{ 
    private void OnTriggerEnter(Collider otherCollider)
    {
        var product = otherCollider.GetComponentInParent<Product>();

        if (product != null)
        {
            Scoreboard.Instance.ScoreProduct(product);
            
            Destroy(product.gameObject);
        }
    }
}

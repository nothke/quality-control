using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductReceiver : MonoBehaviour
{
    ///HashSet<Rigidbody> enteredBodies = new HashSet<Rigidbody>();

    public int normalProductCount;
    public int defectiveProductCount;

    private void OnTriggerEnter(Collider otherCollider)
    {
        var product = otherCollider.GetComponentInParent<Product>();

        if (product != null)
        {
            if (product.Defect != DefectType.None)
            {
                defectiveProductCount++;
            }
            else
            {
                normalProductCount++;
            }
            
            Destroy(product.gameObject);
        }
    }
}

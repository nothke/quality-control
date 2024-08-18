using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductReceiver : MonoBehaviour
{
    ///HashSet<Rigidbody> enteredBodies = new HashSet<Rigidbody>();

    public int normalProductCount;
    public int defectiveProductCount;

    private void OnCollisionEnter(Collision collision)
    {
        var rb = collision.rigidbody;

        if (!rb)
        {
            return;
        }
        
        //enteredBodies.Add(rb);

        if (rb.TryGetComponent(out Product product))
        {
            if (product.Defect != DefectType.None)
            {
                defectiveProductCount++;
            }
            else
            {
                normalProductCount++;
            }
            
            Destroy(rb.gameObject);
        }
    }
}

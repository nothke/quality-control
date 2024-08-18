using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StationaryDefectDetector : MonoBehaviour
{
    public List<Product> _knownProducts;

    public AudioClip goodSound;
    public AudioClip badSound;
    
    private void OnTriggerEnter(Collider other)
    {
        var product = other.GetComponentInParent<Product>();

        if (product == null)
        {
            return;
        }
         
        if (_knownProducts.Contains(product))
        {
            return;
        }

        _knownProducts.Add(product);

        NAudio.Play(product.Defect == DefectType.None ? goodSound : badSound, transform.position);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Product product))
        {
            if (_knownProducts.Contains(product))
            {
                _knownProducts.Remove(product);
            }
        }
    }
}

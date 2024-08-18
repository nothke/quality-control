using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StationaryDefectDetector : MonoBehaviour
{
    public UnityEvent AlarmEvent;

    [Range(0, 100)]
    public int FalsePositiveChance;
    [Range(0, 100)]
    public int FalseNegativeChance;
    
    public List<Product> _knownProducts;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Product product))
        {
            if (_knownProducts.Contains(product))
            {
                return;
            }

            _knownProducts.Add(product);

            if (product.Defect == DefectType.None)
            {
                var falseNegativeRoll = Random.Range(0, 100);
                
                if (falseNegativeRoll > FalseNegativeChance)
                {
                    AlarmEvent.Invoke();
                }
            }
            else
            {
                var falsePositiveRoll = Random.Range(0, 100);
            
                if (falsePositiveRoll < FalsePositiveChance)
                {
                    AlarmEvent.Invoke();
                }
            }
        }
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

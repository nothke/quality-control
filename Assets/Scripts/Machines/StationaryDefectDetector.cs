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
    
    public List<Product> _detectedDefects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Product product))
        {
            if (_detectedDefects.Contains(product))
            {
                return;
            }

            _detectedDefects.Add(product);

            if (other.TryGetComponent(out DefectiveProduct defectiveProduct))
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
        if (other.TryGetComponent(out DefectiveProduct product))
        {
            if (_detectedDefects.Contains(product))
            {
                _detectedDefects.Remove(product);
            }
        }
    }
}

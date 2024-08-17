using System;
using System.Collections.Generic;
using UnityEngine;

public class ProductSpawner : MonoBehaviour
{
    public enum ProductionPhaseType
    {
        Pause = 0,
        Production = 1,
    }
    
    [Serializable]
    public struct ProductionPhase
    {
        public ProductionPhaseType Type;
        public ProductDescription Description;
        [Min(1f)]
        public float Duration;
        [Min(1f)]
        public float TotalSpawnCount;
        
        public float SpawnInterval => Duration / TotalSpawnCount;
    }
    
    public List<ProductionPhase> ProductionPhases;

    public float _remainingDuration;
    public float _spawnTimer;
    
    private void Update()
    {
        if (ProductionPhases.Count == 0)
        {
            enabled = false;
            return;
        }
        
        var currentPhase = ProductionPhases[0];

        if (_remainingDuration <= 0)
        {
            _remainingDuration = currentPhase.Duration;
            _spawnTimer = 0f;
        }
        
        _remainingDuration -= Time.deltaTime;

        if (currentPhase.Type != ProductionPhaseType.Pause)
        {
            _spawnTimer -= Time.deltaTime;

            if (_spawnTimer >= 0f)
            {
                if (_remainingDuration <= 0)
                {
                    ProductionPhases.RemoveAt(0);
                }
                
                return;
            }

            _spawnTimer = currentPhase.SpawnInterval;
        
            Instantiate(currentPhase.Description.GetRandomProduct(), transform.position, Quaternion.identity);
        }
        
        if (_remainingDuration <= 0)
        {
            ProductionPhases.RemoveAt(0);
        }
    }
}   
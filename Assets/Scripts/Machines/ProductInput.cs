using System;
using System.Collections.Generic;
using UnityEngine;

public class ProductInput : MonoBehaviour, IResetable
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
        public ProductType ProductType;
        [Min(1f)]
        public float Duration;
        [Min(1f)]
        public float TotalSpawnCount;
        
        public float SpawnInterval => Duration / TotalSpawnCount;
    }
    
    public List<ProductionPhase> ProductionPhases;
    private List<ProductionPhase> RuntimeProductionPhases;

    public ProductSpawner Spawner;
    
    public float _remainingDuration;
    public float _spawnTimer;

    public void Start()
    {
        ResetMachine();
    }
    
    public void ResetMachine()
    {
        enabled = true;
        _remainingDuration = 0f;
        _spawnTimer = 0f;

        RuntimeProductionPhases = new(ProductionPhases);
    }
    
    private void Update()
    {
        if (RuntimeProductionPhases.Count == 0)
        {
            enabled = false;
            return;
        }
        
        var currentPhase = RuntimeProductionPhases[0];

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
                    RuntimeProductionPhases.RemoveAt(0);
                }
                
                return;
            }

            _spawnTimer = currentPhase.SpawnInterval;
        
            Spawner.SpawnProduct(currentPhase.ProductType);
        }
        
        if (_remainingDuration <= 0)
        {
            RuntimeProductionPhases.RemoveAt(0);
        }
    }
}   
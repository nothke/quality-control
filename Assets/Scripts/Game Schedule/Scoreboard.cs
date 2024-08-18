using System;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard: MonoBehaviour
{
    public static Scoreboard Instance;
    public LevelObjective CurrentObjective;

    public Dictionary<ProductType, Vector2Int> ProductCounts = new ();

    public float _timeLeft;
    public bool _running;
    
    public void Start()
    {
        if (!Instance)
        {
            Instance = this;
        }

        _running = true;
        SetObjective(CurrentObjective);
    }

    public void SetObjective(LevelObjective objective)
    {
        CurrentObjective = objective;

        ProductCounts = new();

        _timeLeft = objective.TimeLimit;
    }

    public void ScoreProduct(Product product)
    {
        if (!ProductCounts.ContainsKey(product.Type))
        {
            ProductCounts[product.Type] = new Vector2Int();
        }
        
        var currentCount = ProductCounts[product.Type];
        
        if (product.Defect == DefectType.None)
        {
            currentCount.x++;
        }
        else
        {
            currentCount.y++;
        }
        
        ProductCounts[product.Type] = currentCount;
    }

    private int TotalCount(ProductType type)
    {
        return ProductCounts[type].x + ProductCounts[type].y;
    }
    
    private int DefectPercentage(ProductType type)
    {
        if (TotalCount(type) == 0)
        {
            return 0;
        }
        
        return 100 * ProductCounts[type].y / TotalCount(type);
    }

    private void Update()
    {
        if (!_running)
        {
            return;
        }
        
        _timeLeft -= Time.deltaTime;
        Debug.Log(_timeLeft);
        
        if (_timeLeft <= 0)
        {
            CountScores();
            _running = false;
        }
    }
    
    public void CountScores()
    {
        bool success = true;

        foreach (var quota in CurrentObjective.Quotas)
        {
            if (quota.Quantity > TotalCount(quota.Type))
            {
                success = false;
                Debug.LogError($"Not enough {quota.Type.name}");
                break;
            }

            Debug.Log($"{quota.Type.name}: {TotalCount(quota.Type)}/{quota.Quantity}");

            if (CurrentObjective.MaxDefectivePercentage < DefectPercentage(quota.Type))
            {
                success = false;
                Debug.LogError($"Too many broken {quota.Type.name}");
                break;
            }
            
            Debug.Log($"{quota.Type.name}: Quality level: {DefectPercentage(quota.Type)}%");
        }
        
        Debug.Log(success? CurrentObjective.SuccessMessage : CurrentObjective.FailureMessage);
    }
}
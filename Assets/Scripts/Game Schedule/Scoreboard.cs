using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Scoreboard: MonoBehaviour
{
    public static Scoreboard Instance;
    public LevelObjective CurrentObjective;

    public Dictionary<ProductType, Vector2Int> ProductCounts = new ();

    public float _timeLeft;
    public bool _running;

    public TextMeshPro textMesh;

    public void OnEnable()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }
    
    public void Start()
    {
        SetObjective(CurrentObjective);
        UpdateText();
    }
    
#if UNITY_EDITOR
    [MenuItem("Tools/Restart Level")]
#endif
    public static void RestartLevel()
    {
        foreach (var machine in StagingManager.Instance.GetComponentsInChildren<IResetable>(true))
        {
            machine.ResetMachine();
        }
        
        var allProducts = FindObjectsOfType<Product>();
        for (int i = 0; i < allProducts.Length; i++)
        {
            Destroy(allProducts[i].gameObject);
        }
        
        Instance.SetObjective(Instance.CurrentObjective);
        Instance.UpdateText();
    }
    
    public void SetObjective(LevelObjective objective)
    {
        CurrentObjective = objective;
        
        StagingManager.SetStage(objective.Stage);

        ProductCounts = new();

        _timeLeft = objective.TimeLimit;
        _running = true;
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
        
        UpdateText();
        
        if (_timeLeft <= 0)
        {
            CountScores();
            _running = false;
        }
    }

    public void UpdateText()
    {
        StringBuilder sb = new();
        
        int minutes = (int) _timeLeft / 60;
        int seconds = (int) _timeLeft % 60;
        sb.Append($"Time left: {minutes}:{seconds}\n");

        sb.Append("\n");

        sb.Append(CurrentObjective.LevelMessage);

        sb.Append("\n");
        sb.Append("\n");
        
        foreach (var quota in CurrentObjective.Quotas)
        {
            if (ProductCounts.ContainsKey(quota.Type))
            {
                sb.Append($"{quota.Type.name}: {TotalCount(quota.Type)}/{quota.Quantity}\n");
            }
            else
            {
                sb.Append($"{quota.Type.name}: 0/{quota.Quantity}\n");
            }
        }
        
        textMesh.text = sb.ToString();
    }
    
    public void CountScores()
    {
        bool success = true;

        foreach (var quota in CurrentObjective.Quotas)
        {
            if (!ProductCounts.ContainsKey(quota.Type))
            {
                success = false;
                Debug.LogError($"Not enough {quota.Type.name}");
                break;
            }
            
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
using System.Collections.Generic;
using System.Text;
using KinematicCharacterController.Examples;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Scoreboard: MonoBehaviour
{
    public static Scoreboard Instance;

    public ExampleCharacterController Controller;
    public ExamplePlayer Player;
    public ExampleCharacterCamera Camera;
    public Transform StartPosition;
    public Transform StartLook;

    public StagingManager.StageEnum CurrentStage;
    public LevelObjective CurrentObjective;

    public LevelObjective Stage1Objective;
    public LevelObjective Stage2Objective;
    public LevelObjective Stage3Objective;
    
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
        
        foreach (var stageProp in FindObjectsOfType<StageProp>(true))
        {
            stageProp.gameObject.SetActive(false);
        }
    }
    
    public void StartStage(StagingManager.StageEnum stage)
    {
        CurrentStage = stage;
        
        switch (stage)
        {
            case StagingManager.StageEnum.Level1:
                SetObjective(Stage1Objective);
                break;
            case StagingManager.StageEnum.Level2:
                SetObjective(Stage2Objective);
                break;
            case StagingManager.StageEnum.Level3:
                SetObjective(Stage3Objective);
                break;
        }
        
        RestartCurrentStage();
        
        UpdateText();
    }
    
#if UNITY_EDITOR
    [MenuItem("Tools/Restart Level")]
#endif
    public static void RestartCurrentStage()
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
        
        Instance.Controller.Motor.SetPosition(Instance.StartPosition.position);
        Instance.Controller.Motor.SetRotation(Instance.StartPosition.rotation);
        Instance.Player.enabled = false;
        Instance.Camera.transform.rotation = Instance.StartLook.rotation;
        Instance.Player.enabled = true;
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
        
        int totalSeconds = (int) _timeLeft;
        
        int minutes = (int) _timeLeft / 60;
        int seconds = (int) _timeLeft % 60;

        sb.Append("Time left: ");
        
        if (totalSeconds < 30)
        {
            sb.Append("<color=red>");
        }
        else
        {
            sb.Append("<color=green>");
        }
        
        if (seconds < 10)
        {
            sb.Append($"{minutes}:0{seconds}");
        }
        else
        {
            sb.Append($"{minutes}:{seconds}");
        }
        
        sb.Append("</color>\n");

        sb.Append("\n");

        sb.Append(CurrentObjective.LevelMessage);

        sb.Append("\n");
        sb.Append("\n");
        
        foreach (var quota in CurrentObjective.Quotas)
        {
            if (ProductCounts.ContainsKey(quota.Type))
            {
                if (quota.Quantity <= TotalCount(quota.Type))
                {
                    sb.Append("<color=green>");
                }
                else
                {
                    sb.Append("<color=red>");
                }
                
                sb.Append($"{quota.Type.name}: {TotalCount(quota.Type)}/{quota.Quantity}\n");
                
                sb.Append("</color>");
            }
            else
            {
                sb.Append("<color=red>");
                sb.Append($"{quota.Type.name}: 0/{quota.Quantity}\n");
                sb.Append("</color>");
            }
        }
        
        textMesh.text = sb.ToString();
    }
    
    public void CountScores()
    {
        bool success = true;
        
        StringBuilder sb = new();
        
        foreach (var quota in CurrentObjective.Quotas)
        {
            if (!ProductCounts.ContainsKey(quota.Type))
            {
                success = false;
                sb.Append($"Not a single product of type {quota.Type.name} was produced!\n");
                break;
            }
            
            if (quota.Quantity > TotalCount(quota.Type))
            {
                success = false;
                sb.Append($"Not enough {quota.Type.name} were produced.\n");
                break;
            }

            sb.Append($"{quota.Type.name}: {TotalCount(quota.Type)}/{quota.Quantity}\n");

            sb.Append("\n");
            
            sb.Append($"Overall {quota.Type.name} defect percentage: {DefectPercentage(quota.Type)}%\n");            
            
            if (quota.MaxDefectivePercentage < DefectPercentage(quota.Type))
            {
                success = false;
                sb.Append($"Too many of {quota.Type.name} were defective!\n");
            }
            
            sb.Append("\n");
        }
        
        sb.Append(success ? CurrentObjective.SuccessMessage : CurrentObjective.FailureMessage);

        UIManager.Instance.PushStageComplete(success, sb.ToString());
    }
}
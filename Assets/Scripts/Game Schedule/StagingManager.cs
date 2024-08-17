using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StagingManager: MonoBehaviour
{
    public static StagingManager Instance;
    public static List<StageProp> StageProps;

    public StageEnum CurrentStage;
    
    [Flags]
    public enum StageEnum
    {
        Level1 = 1,
        Level2 = 2,
    }

    public void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        SetStage(StageEnum.Level1);
    }

    public void OnDestroy()
    {
        StageProps.Clear();
    }

    public static void RegisterStageProp(StageProp stageProp)
    {
        if (StageProps == null)
        {
            StageProps = new List<StageProp>();
        }

        if (!StageProps.Contains(stageProp))
        {
            StageProps.Add(stageProp);
        }
    }
    
    public static void RemoveStageProp(StageProp stageProp)
    {
        if (StageProps.Contains(stageProp))
        {
            StageProps.Remove(stageProp);
        }
    }
    
    public static void SetStage(StageEnum stage)
    {
        Instance.CurrentStage = stage;
        
        foreach (var stageProp in StageProps)
        {
            stageProp.gameObject.SetActive((stageProp.ActiveAtStages & stage) != 0);
        }
    }

    [MenuItem("Tools/Staging/Level1")]
    public static void SetStage1()
    {
        SetStage(StageEnum.Level1);
    }
    
    [MenuItem("Tools/Staging/Level2")]
    public static void SetStage2()
    {
        SetStage(StageEnum.Level2);
    }
}
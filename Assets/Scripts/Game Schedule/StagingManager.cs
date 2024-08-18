using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StagingManager: MonoBehaviour
{
    public static StagingManager Instance;
    public static List<StageProp> StageProps;

    public List<LevelObjective> Objectives;

    public StageEnum CurrentStage;
    
    [Flags]
    public enum StageEnum
    {
        Level1 = 1,
        Level2 = 2,
    }

    public void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
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

#if UNITY_EDITOR
    [MenuItem("Tools/Staging/Level1")]
#endif
    public static void SetStage1()
    {
        SetStage(StageEnum.Level1);
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Staging/Level2")]
#endif
    public static void SetStage2()
    {
        SetStage(StageEnum.Level2);
    }
}
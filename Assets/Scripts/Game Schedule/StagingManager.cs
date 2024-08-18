using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StagingManager: MonoBehaviour
{
    public static StagingManager Instance;

    public List<LevelObjective> Objectives;

    public StageEnum CurrentStage;
    
    [Flags]
    public enum StageEnum
    {
        Level1 = 1 << 0,
        Level2 = 1 << 1,
        Level3 = 1 << 2,
    }

    public void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public static void SetStage(StageEnum stage)
    {
        Instance.CurrentStage = stage;
        
        foreach (var stageProp in FindObjectsOfType<StageProp>(true))
        {
            Debug.Log(stageProp.ActiveAtStages.HasFlag(stage));
            stageProp.gameObject.SetActive(stageProp.ActiveAtStages.HasFlag(stage));
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
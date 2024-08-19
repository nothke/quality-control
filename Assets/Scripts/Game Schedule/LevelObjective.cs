using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Level Objective", menuName = "Data/Level Objective", order = 0)]
public class LevelObjective : ScriptableObject
{
    public StagingManager.StageEnum Stage;

    [Serializable]
    public struct ProductQuota
    {
        public ProductType Type;
        public int Quantity;
        [Range(0, 100)] 
        public int MaxDefectivePercentage;
    }
    
    public List<ProductQuota> Quotas;

    public float TimeLimit;
    
    [TextArea]
    public string LevelMessage;
    
    [TextArea]
    public string SuccessMessage;
    
    [TextArea]
    public string FailureMessage;

}
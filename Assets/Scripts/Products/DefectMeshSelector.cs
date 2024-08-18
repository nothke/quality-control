using System;
using System.Collections.Generic;
using UnityEngine;

public class DefectMeshSelector: MonoBehaviour, IDefectVisualizer
{
    public GameObject NormalPrefab;
    
    [Serializable]
    public struct PrefabVariant
    {
        public DefectType DefectType;
        public GameObject Prefab;
    }
    
    public List<PrefabVariant> DefectivePrefabVariants;
    
    public void VisualizeDefect(DefectType defectType)
    {
        if (defectType == DefectType.None)
        {
            NormalPrefab.SetActive(true);
            
            foreach (var variant in DefectivePrefabVariants)
            {
                variant.Prefab.SetActive(false);
            }
        }
        else
        {
            NormalPrefab.SetActive(false);
            
            foreach (var variant in DefectivePrefabVariants)
            {
                variant.Prefab.SetActive(false);
                
                if (defectType.HasFlag(variant.DefectType))
                {
                    variant.Prefab.SetActive(true);
                    return;
                }
            }
        }
    }
}
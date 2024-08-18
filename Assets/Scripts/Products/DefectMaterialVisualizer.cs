using System.Collections.Generic;
using UnityEngine;

public class DefectMaterialVisualizer: MonoBehaviour, IDefectVisualizer
{
    public MeshRenderer MeshRenderer;
    
    private List<DefectType> supportedDefects = new()
    {
        DefectType.Hex_MissingMaterial_1,
        DefectType.Hex_MissingMaterial_2,
        DefectType.Hex_MissingMaterial_3,
        DefectType.Hex_MissingMaterial_4,
        DefectType.Hex_MissingMaterial_5,
    };
    
    public void VisualizeDefect(DefectType defectType)
    {
        for (var i = 0; i < supportedDefects.Count; i++)
        {
            if (!defectType.HasFlag(supportedDefects[i]))
            {
                continue;
            }
            
            MeshRenderer.materials[i] = GameAssets.Instance.MissingMaterial;
        }
    }
}
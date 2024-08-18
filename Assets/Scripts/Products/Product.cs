using UnityEngine;

public class Product : MonoBehaviour
{
    public ProductType Type;
    public DefectType Defect;
    
    public void ApplyDefect(DefectType defectType)
    {
        Defect = defectType;

        foreach (var visualizer in GetComponents<IDefectVisualizer>())
        {
            visualizer.VisualizeDefect(defectType);
        }
    }
}
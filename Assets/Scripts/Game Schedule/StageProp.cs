using UnityEngine;

public class StageProp: MonoBehaviour
{
    public StagingManager.StageEnum ActiveAtStages;

    public void Start()
    {
        StagingManager.RegisterStageProp(this);
        gameObject.SetActive(false);
    }
    
    public void OnDestroy()
    {
        StagingManager.RemoveStageProp(this);
    }
}
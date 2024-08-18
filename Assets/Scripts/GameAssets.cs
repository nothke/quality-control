using UnityEngine;

public class GameAssets: MonoBehaviour
{
    public static GameAssets Instance;

    public Material MissingMaterial;
    
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
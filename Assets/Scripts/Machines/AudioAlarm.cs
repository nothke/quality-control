using UnityEngine;

public class AudioAlarm : MonoBehaviour
{
    public float Duration;
    
    private bool _isPlaying;
    
    public void PlayAlarm()
    {
        Debug.Log("Playing alarm");
    }
}
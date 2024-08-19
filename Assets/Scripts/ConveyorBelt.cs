using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour, IResetable
{
    Rigidbody _rb;
    Rigidbody rb { get { if (!_rb) _rb = GetComponent<Rigidbody>(); return _rb; } }

    public float speed = 1;

    Vector3 startPosition;

    float scrollingTextureProgress = 0;
    public float scrollingTextureSpeedMult = 1;
    public Renderer scrollingTextureRenderer;

    public AudioClip conveyorClip;
    public float volume = 0.25f;
    private AudioSource _audioSource;
    
    public void ResetMachine()
    {
        if (_audioSource)
        {
            _audioSource.Stop();
            Destroy(_audioSource.gameObject);
            _audioSource = null;
        }

        if (isActiveAndEnabled)
        {
            _audioSource = NAudio.Play(conveyorClip, transform.position, volume);
            _audioSource.loop = true;   
        }
    }
    
    private void Start()
    {
        ResetMachine();
        
        startPosition = rb.position;

        //scrollingTextureSpeedMult = -0.1625f * speed;
    }

    private void FixedUpdate()
    {
        rb.position = startPosition;
        Vector3 targetSpeed = speed * transform.forward;
        rb.MovePosition(rb.position + targetSpeed * Time.deltaTime);
    }

    private void Update()
    {
        if(scrollingTextureRenderer)
        {
            scrollingTextureProgress += Time.deltaTime * speed * scrollingTextureSpeedMult;
            scrollingTextureRenderer.material.SetTextureOffset("_MainTex", 
                new Vector2(0, scrollingTextureProgress));
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public float CooldownDuration;
    private float _cooldownTimer;
    
    private float _incinerationTimer;
    public float IncinerationDuration = 1f;

    public float doorVelocity = 5f;
    public Transform DoorTransform;
    public Transform OpenAnchor;
    public Transform ClosedAnchor;
    private Vector3 _targetPosition;

    public AudioClip destroyClip;
    
    List<Product> enteredProducts = new List<Product>();

    public void Start()
    {
        _targetPosition = OpenAnchor.position;
        _incinerationTimer = IncinerationDuration;
    }
    
    public void OnTriggerEnter(Collider otherCollider)
    {
        var product = otherCollider.GetComponentInParent<Product>();

        if (product == null)
        {
            return;
        }
        
        if (!enteredProducts.Contains(product))
        {
            enteredProducts.Add(product);
        }
            
        _incinerationTimer = IncinerationDuration;
        _targetPosition = ClosedAnchor.position;
    }

    public void OnTriggerExit(Collider collider)
    {
        var product = collider.GetComponentInParent<Product>();
        
        if (product)
        {
            if (enteredProducts.Contains(product))
            {
                enteredProducts.Remove(product);
            }
        }
    }
    
    private void Update()
    {
        DoorTransform.position = Vector3.Lerp(
            DoorTransform.position, 
            _targetPosition, 
            Time.deltaTime * doorVelocity);
        
        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
            
            if (_cooldownTimer <= 0)
            {
                _targetPosition = OpenAnchor.position;
            }
            
            return;
        }
        
        if (enteredProducts.Count == 0)
        {
            return;
        }
        
        _incinerationTimer -= Time.deltaTime;
        
        if (_incinerationTimer <= 0)
        {
            foreach (var product in enteredProducts)
            {
                Destroy(product.gameObject);
            }
            
            enteredProducts.Clear();

            NAudio.Play(destroyClip, transform.position, 0.33f);
            enteredProducts.Clear();
            _cooldownTimer = CooldownDuration;
        }
    }
}
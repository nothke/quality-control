using System.Collections.Generic;
using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public float CooldownDuration;
    private float _cooldownTimer;
    
    private float _incinerationTimer = 1f;
    public float IncinerationDuration = 1f;

    public float doorVelocity = 5f;
    public Transform DoorTransform;
    public Transform OpenAnchor;
    public Transform ClosedAnchor;
    private Vector3 _targetPosition;
    
    List<Product> enteredProducts = new List<Product>();

    public void Start()
    {
        _targetPosition = OpenAnchor.position;
    }
    
    public void OnTriggerEnter(Collider otherCollider)
    {
        var rb = otherCollider.GetComponent<Rigidbody>();

        if (!rb)
        {
            return;
        }

        if (otherCollider.TryGetComponent(out Product product))
        {
            if (!enteredProducts.Contains(product))
            {
                enteredProducts.Add(product);
            }
            
            _cooldownTimer = CooldownDuration;
            _targetPosition = ClosedAnchor.position;
        }
        else
        {
            rb.AddForce(5f * Vector3.up);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.TryGetComponent(out Product product))
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
        
        if (_cooldownTimer <= 0)
        {
            return;
        }
        
        _cooldownTimer -= Time.deltaTime;
        _incinerationTimer -= Time.deltaTime;
        
        if (_incinerationTimer <= 0)
        {
            foreach (var product in enteredProducts)
            {
                Destroy(product.gameObject);
            }
            
            enteredProducts.Clear();
            _incinerationTimer = IncinerationDuration;
        }
        
        if (_cooldownTimer <= 0)
        {
            _targetPosition = OpenAnchor.position;
        }
    }
}
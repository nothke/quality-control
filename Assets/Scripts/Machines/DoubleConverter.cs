using System;
using System.Collections.Generic;
using UnityEngine;

public class DoubleConverter: MonoBehaviour, IResetable
{
    public ProductSpawner Spawner;
    public List<Product> inputProducts;

    public ProductType expectedReagent;
    public Transform outputPoint;
    public ProductType conversionProduct;

    public int CurrentHealth;
    public int MaxHealth;
    
    public float conversionDuration = 5f;
    private float _conversionTimer;
    
    public Transform refuseLauncher;
    public float launchPower = 10f;
    
    public void OnTriggerEnter(Collider other)
    {
        var product = other.GetComponentInParent<Product>();
        
        if (product)
        {
            product.gameObject.SetActive(false);
            inputProducts.Add(product);
        }
    }

    public void Start()
    {
        ResetMachine();
    }

    public void ResetMachine()
    {
        _conversionTimer = conversionDuration;
        CurrentHealth = MaxHealth;
        
        inputProducts.Clear();
    }

    public void Update()
    {
        if (CurrentHealth <= 0)
        {
            return;
        }
        
        if (inputProducts.Count == 0)
        {
            return;
        }

        if (_conversionTimer <= 0f)
        {
            var currentProduct = inputProducts[0];
            
            if (inputProducts[0].Type == expectedReagent)
            {
                Spawner.SpawnProduct(conversionProduct);
                inputProducts.RemoveAt(0);
                Destroy(currentProduct);
            }
            else
            {
                Expel(inputProducts[0]);
                inputProducts.RemoveAt(0);
            }
            
            _conversionTimer = conversionDuration;
        }
        
        _conversionTimer -= Time.deltaTime;
    }

    private void Expel(Product product)
    {
        product.transform.position = refuseLauncher.position;
        product.transform.rotation = refuseLauncher.rotation;

        product.gameObject.SetActive(true);
        
        product.GetComponent<Rigidbody>().velocity = launchPower * refuseLauncher.forward;
    }
}
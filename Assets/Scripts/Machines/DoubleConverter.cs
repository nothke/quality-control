using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DoubleConverter: MonoBehaviour, IResetable
{
    public ProductSpawner Spawner;
    public List<Product> inputTypeOne;
    public List<Product> inputTypeTwo;
    public List<Product> refuse;

    public ProductType expectedReagentA;
    public ProductType expectedReagentB;
    public ProductType conversionProduct;

    public int CurrentHealth;
    public int MaxHealth;
    
    public float conversionDuration = 5f;
    private float _conversionTimer;
    
    public Transform refuseLauncher;
    public float launchPower = 5f;
    
    public void OnTriggerEnter(Collider other)
    {
        var product = other.GetComponentInParent<Product>();
        
        product.gameObject.SetActive(false);
        
        if (product.Type == expectedReagentA)
        {
            inputTypeOne.Add(product);
        }
        else if (product.Type == expectedReagentB)
        {
            inputTypeTwo.Add(product);
        }
        else
        {
            refuse.Add(product);
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
        
        inputTypeOne.Clear();
        inputTypeTwo.Clear();
        refuse.Clear();
    }

    public void Update()
    {
        if (CurrentHealth <= 0)
        {
            return;
        }
        
        if (refuse.Count == 0 && inputTypeOne.Count == 0 && inputTypeTwo.Count == 0)
        {
            return;
        }

        if (_conversionTimer <= 0f)
        {
            if (refuse.Count > 0)
            {
                Expel(refuse[0]);
                refuse.RemoveAt(0);
                _conversionTimer = conversionDuration;
                return;
            }
            
            if (inputTypeOne.Count > 0 && inputTypeTwo.Count > 0)
            {
                DefectType defectType = inputTypeOne[0].Defect & inputTypeTwo[0].Defect;
                
                inputTypeOne.RemoveAt(0);
                inputTypeTwo.RemoveAt(0);
                
                Spawner.SpawnProduct(conversionProduct, defectType);
                return;
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
﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Converter: MonoBehaviour, IResetable
{
    public ProductSpawner Spawner;
    public List<Product> inputProducts;

    public ProductType expectedReagent;
    public Transform outputPoint;
    public ProductType conversionProduct;
    public bool introduceDefect;
    
    public int CurrentHealth;
    public int MaxHealth;
    
    public float conversionDuration = 5f;
    private float _conversionTimer;
    
    public AudioClip conversionClip;
    public AudioClip launchClip;
    
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
                if (!introduceDefect)
                {
                    Spawner.SpawnProduct(conversionProduct, currentProduct.Defect);
                }
                else
                {
                    Spawner.SpawnProduct(conversionProduct);
                }
                
                inputProducts.RemoveAt(0);
                Destroy(currentProduct);

                NAudio.Play(conversionClip, transform.position, 0.75f);
            }
            else
            {
                Expel(inputProducts[0]);
                inputProducts.RemoveAt(0);

                NAudio.Play(launchClip, transform.position);
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
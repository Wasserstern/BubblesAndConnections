using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{

    [Header("Plant Settings")]
    public float sproutTime;
    public float growTime;
    public Color baseColor;
    public float minDistanceToOtherPlants;

    // Runtime variables
    float currentGrowTime;
    float currentSproutTime;
    bool isGrowing;
    bool isSprouting;
    void Start()
    {
        isSprouting = true;
    }

    void Update()
    {
        if(isGrowing){
            if(currentGrowTime >= growTime){
                // Plant reaches maturity. This is only called one time. TODO: Some bloom logic + particle effects + animation
                isGrowing = false;
            }
            currentGrowTime += Time.deltaTime;
        }

        if(!isGrowing && currentGrowTime >= growTime){
            // Plant is mature
            currentGrowTime += Time.deltaTime; // This is basically the plants age counter from this point on

        }
        if(isSprouting){
            currentSproutTime += Time.deltaTime;
            if(currentSproutTime >= sproutTime){
                isSprouting = false;
                // Sprout time over. Seed shall grow to plant now.
                Grow();
            }
        }
    }
    /// <summary>
    /// Starts an L-System algorithm that generates branches and leaves.
    /// </summary>
    /// // TODO: Implement L-System
    protected virtual void Grow(){
        isGrowing = true;
    }
    
}

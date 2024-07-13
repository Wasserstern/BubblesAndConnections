using System.Collections.Generic;
using UnityEngine;
public enum FruitType{none, shmapple}
public class Bud : MonoBehaviour 
{
    [Header("General Settings")]

    public List<GameObject> fruitPrefabs;
    FruitType type = FruitType.none;
    public float shmappleGrowTime;

    // Runtime variables
    float growTime;
    float currentGrowTime;
    GameObject fruit;
    private void Start()
    {

    }
    public bool TryHarvest(){
        if(fruit != null){
            currentGrowTime = 0f;
            fruit.transform.SetParent(null);
            fruit = null;
            fruit.GetComponent<Item>().SetInteractable(true);
            return true;
        }
        else{
            return false;
        }
    }

    public void SetType(FruitType type){
        this.type = type;
    }

    private void Update()
    {
        if(type != FruitType.none)
        {
            if(growTime == 0)
            {
                // Set grow time according to fruit type
                switch(type)
                {
                case FruitType.shmapple:
                    {
                        growTime = shmappleGrowTime;
                        break;
                    }
                default:
                    {
                        growTime = 0f;
                        break;
                    }
                }
            }

            if(currentGrowTime >= growTime)
            {
                if(fruit == null){
                    GameObject newFruit = null;
                    switch(type){
                        case FruitType.shmapple:{
                            newFruit = GameObject.Instantiate(fruitPrefabs[0], transform);
                            break;
                        }
                        default:{
                            break;
                        }
                    }
                    fruit = newFruit;
                }
                currentGrowTime = 0f;
            }
            currentGrowTime += Time.deltaTime;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public enum PlantType {tree, bush, flower};

    public PlantType plantType;
    void Start()
    {
        switch (plantType){
            case PlantType.tree:{
                break;
            }
            case PlantType.bush:{
                break;
            }
            case PlantType.flower:{
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBox : MonoBehaviour
{

    // TextBox settings
    [TextAreaAttribute]
    public string[] pages;
    public float width;
    public float height;
    public float spaceBetweenLetters;
    public float spaceBetweenLines;
    public float letterAppearTime;
    public Material standardLetterMaterial;
    public List<GameObject> letterObjects;


    // Runtime variables
    Vector2 nextLetterPosition;
    int currentPageIndex;


    // Start is called before the first frame update
    void Start()
    {
        nextLetterPosition = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T)){
            StartCoroutine(ShowText(0));
        }
    }

    public IEnumerator ShowText(int pageIndex){
        string text = pages[pageIndex];
        for(int i = 0; i < text.Length; i++){
            char c = text[i];
            Debug.Log((int)c - 65);
            if(c !=  32){
                GameObject letterObject = GameObject.Instantiate(letterObjects[(int)c - 65], transform);
                letterObject.GetComponent<SpriteRenderer>().material = standardLetterMaterial;
                letterObject.transform.localScale = Vector3.one;
                letterObject.transform.localPosition = nextLetterPosition;
                
            }
            nextLetterPosition += new Vector2(spaceBetweenLetters, 0f);
            yield return new WaitForSeconds(letterAppearTime);
            if(nextLetterPosition.x > width){
                nextLetterPosition = new Vector2(0f, nextLetterPosition.y - spaceBetweenLetters);
                if(nextLetterPosition.y < -height){
                    // Textbox max height reached. Stop here
                    break;
                }
            }

        }
    }

    
}

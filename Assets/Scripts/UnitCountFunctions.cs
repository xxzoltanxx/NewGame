using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitCountFunctions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetAlphaUnitCount(float alpha)
    {
        Color color = transform.GetChild(0).gameObject.GetComponent<Text>().color;
        transform.GetChild(0).gameObject.GetComponent<Text>().color = new Color(color.r, color.g, color.b, alpha);
        Color color2 = transform.GetChild(1).gameObject.GetComponent<WavySprite>().tint;
        transform.GetChild(1).gameObject.GetComponent<WavySprite>().tint= new Color(color2.r, color2.g, color2.b, alpha);
    }
}

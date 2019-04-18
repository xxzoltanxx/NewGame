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
    }
}

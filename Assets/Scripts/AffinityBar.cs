using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AffinityBar : MonoBehaviour
{
    public void setName(string name)
    {
        transform.GetChild(1).GetChild(1).gameObject.GetComponent<Text>().text = name;
    }
    public void setAffinityPercentage(float percentage)
    {
        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.SetFloat("_Fillpercentage", percentage);
    }

    public void setBoundSoldiers(int soldiers)
    {
        transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = soldiers.ToString();
    }

    // Start is called before the first frame update
    void Awake()
    {
        Messenger.AddListener<float,float>("kChangeAffinityBarScaleOpacity", setScale);
        Material copyMaterial = new Material(transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material);
        copyMaterial.renderQueue = 4300;
        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = copyMaterial;
        transform.GetChild(1).gameObject.GetComponent<Image>().material.renderQueue = 4300;
    }

    private void setScale(float scale, float opacity)
    {
        scale = scale / 3;
        transform.GetChild(1).gameObject.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
        transform.GetChild(0).localScale = new Vector3(scale, scale, scale);
        //transform.GetChild(1).GetChild(0).gameObject.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.SetFloat("_Globalopacity", opacity);
        Color color = transform.GetChild(1).gameObject.GetComponent<Image>().color;
        transform.GetChild(1).gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, opacity);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

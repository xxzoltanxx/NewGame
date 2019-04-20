using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExclamationMarkScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetActiveIfInsideFOV()
    {
        if (transform.parent.GetComponent<FovFadeable>().isInsideFOV)
        {
            gameObject.SetActive(true);
        }
    }
}

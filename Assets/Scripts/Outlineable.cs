using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteOutline))]
[RequireComponent(typeof(Collider2D))]
public class Outlineable : MonoBehaviour
{
    public SpriteRenderer FOVCircle;
    public float outlineSize = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        GetComponent<SpriteOutline>()._outlineSize = outlineSize;
        GetComponent<SpriteOutline>().UpdateOutline(outlineSize);
        if (FOVCircle)
        {
            FOVCircle.color = new Color(FOVCircle.color.r, FOVCircle.color.g, FOVCircle.color.b, 1.0f);
        }
    }

    private void OnMouseExit()
    {
        GetComponent<SpriteOutline>()._outlineSize = 0;
        GetComponent<SpriteOutline>().UpdateOutline(0);
        if (FOVCircle)
        {
            FOVCircle.color = new Color(FOVCircle.color.r, FOVCircle.color.g, FOVCircle.color.b, 0.0f);
        }
    }
}

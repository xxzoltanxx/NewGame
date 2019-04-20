using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Entity))]
public class FovFadeable : MonoBehaviour
{
    private int fadeFlag = 0;
    private bool canShowFOV = false;
    public bool enemyFOVENabled = true;
    private const float fadePerSec = 0.8f;
    public SpriteRenderer FOVCircle;
    public bool isInsideFOV = false;
    bool destroyOnFadeOut = false;
    public Entity enemyEntity;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Dissapear()
    {
        fadeFlag = 2;
        destroyOnFadeOut = true;
    }

    private void Awake()
    {
        if (transform.childCount > 0)
        {
            if (transform.GetChild(0).tag == "enemyFOV")
            {
                FOVCircle = transform.GetChild(0).GetComponent<SpriteRenderer>();
            }
            else
            {
                FOVCircle = null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (FOVCircle && canShowFOV)
        {
            FOVCircle.color = new Color(FOVCircle.color.r, FOVCircle.color.g, FOVCircle.color.b, 1.0f);
        }
        else
        {
            FOVCircle.color = new Color(FOVCircle.color.r, FOVCircle.color.g, FOVCircle.color.b, 0.0f);
        }
        if (!destroyOnFadeOut)
        {
            if (isInsideFOV && (!GetComponent<Entity>().hidden && !enemyEntity.hidden))
            {
                fadeFlag = 1;
                if (enemyFOVENabled)
                {
                    canShowFOV = true;
                }
            }
            else if (isInsideFOV && (!GetComponent<Entity>().hidden && enemyEntity.hidden))
            {
                fadeFlag = 1;
                if (enemyFOVENabled)
                {
                    canShowFOV = true;
                }
            }
            else if (isInsideFOV && (GetComponent<Entity>().hidden && enemyEntity.hidden))
            {
                fadeFlag = 1;
                if (enemyFOVENabled)
                {
                    canShowFOV = true;
                }
            }
            else if (isInsideFOV)
            {
                fadeFlag = 2;
                if (enemyFOVENabled)
                {
                    canShowFOV = false;
                }
            }
        }
        if (fadeFlag == 1)
        {
            Color color = GetComponent<SpriteRenderer>().color;
            if (color.a >= 1)
            {
                fadeFlag = 0;
            }
            else
            {
                color = new Color(color.r, color.g, color.b, Mathf.Min(1.0f, color.a + fadePerSec * Time.deltaTime));
                BroadcastMessage("SetAlphaUnitCount", color.a);
                GetComponent<SpriteRenderer>().color = color;
            }
        }
        else if (fadeFlag == 2)
        {
            Color color = GetComponent<SpriteRenderer>().color;
            if (color.a <= 0)
            {
                fadeFlag = 0;
                if (destroyOnFadeOut)
                {
                    GameObject.Destroy(gameObject);
                }
            }
            else
            {
                color = new Color(color.r, color.g, color.b, Mathf.Max(0.0f, color.a - fadePerSec * Time.deltaTime));
                BroadcastMessage("SetAlphaUnitCount", color.a);
                GetComponent<SpriteRenderer>().color = color;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<FOV>())
        {
            isInsideFOV = true;
            enemyEntity = collision.gameObject.GetComponentInParent<Entity>();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<FOV>())
        {
            isInsideFOV = false;
            fadeFlag = 2;
            canShowFOV = false;
            FOVCircle.color = new Color(FOVCircle.color.r, FOVCircle.color.g, FOVCircle.color.b, 0.0f);
        }
    }

    private void OnMouseEnter()
    {
    }

    private void OnMouseExit()
    {
        GetComponent<SpriteOutline>()._outlineSize = 0;
        GetComponent<SpriteOutline>().UpdateOutline(0);
    }
}

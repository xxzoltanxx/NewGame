using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattleEntryScript : MonoBehaviour
{
    public float menuZLevel = -8;
    public BattleState battleState;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Close()
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        GetComponent<CanvasGroup>().interactable = false;
        GetComponent<Animator>().SetTrigger("dissapear");
    }
    public void Open(Vector2 position, int playerSoldiers, int enemySoldiers, BattleState state)
    {
        this.battleState = state;
        GetComponent<RectTransform>().position = new Vector3(position.x, position.y, menuZLevel);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<Animator>().SetTrigger("appear");

        transform.GetChild(0).GetChild(3).GetComponent<Text>().text = enemySoldiers.ToString();
        transform.GetChild(0).GetChild(2).GetComponent<Text>().text = playerSoldiers.ToString();
    }

    public void ResolveBattle()
    {
        battleState.ResolveBattle();
        Close();
    }
}

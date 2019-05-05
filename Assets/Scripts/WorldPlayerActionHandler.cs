using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPlayerActionHandler : MonoBehaviour
{
    public float menuZLevel = -14;
    public Dictionary<string, Sprite> skillIcons = new Dictionary<string,Sprite>();
    public Entity playerEntity;
    public Abilities playerAbilities;
    private GameObject radialMenu;
    public bool menuShown = false;
    // Start is called before the first frame update
    public void lazyInit(Entity ent)
    {
        playerEntity = ent;
        playerAbilities = playerEntity.abilities;
    }

    public void Awake()
    {
        radialMenu = GameObject.Find("radialMenuSkills");
        skillIcons.Add("Dissapear", Resources.Load<Sprite>("abilitiesIcons/021"));
        skillIcons.Add("None", null);
    }
    void Start()
    {
        
    }

    public void AddAbilityAndTriggerCooldown(Ability ability)
    {
        playerAbilities.activeAbilities.Add(ability);
        playerAbilities.activeAbilities[playerAbilities.activeAbilities.Count - 1].init(playerEntity);
        playerAbilities.cooldown = playerAbilities.cooldownTime;
    }

    public void OpenMenuWorld(Vector2 position)
    {
        if (!menuShown)
        {
            radialMenu.transform.parent.position = new Vector3(playerEntity.gameObject.transform.position.x, playerEntity.gameObject.transform.position.y, menuZLevel);
            var menu = radialMenu.GetComponent<RMF_RadialMenu>();
            radialMenu.GetComponent<RMF_RadialMenu>().Init(playerAbilities.learnedWorldAbilities, this);
            radialMenu.GetComponent<Animator>().SetTrigger("show");
            radialMenu.GetComponent<CanvasGroup>().blocksRaycasts = true;
            menuShown = true;
        }
        else
        {
            menuShown = false;
            radialMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
            radialMenu.GetComponent<Animator>().SetTrigger("dissapear");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

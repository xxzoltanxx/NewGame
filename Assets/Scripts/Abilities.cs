using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    public List<Ability> learnedWorldAbilities = new List<Ability>();
    public List<Ability> activeAbilities = new List<Ability>();
    List<Ability> toRemove = new List<Ability>();
    public float cooldown = 0;
    public float cooldownTime = 10.0f;
    // Start is called before the first frame update
    void Awake()
    {
        learnedWorldAbilities.Add(new DissapearAbility());
    }

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;
        cooldown = Mathf.Max(cooldown - Time.deltaTime, 0);
        toRemove.Clear();
        foreach (Ability ability in activeAbilities)
        {
            if (ability.Exec(Time.deltaTime))
            {
                toRemove.Add(ability);
            }
        }
        foreach (var ability in toRemove)
        {
            activeAbilities.Remove(ability);
        }
    }
}

public class Ability
{
    public float abilityDuration = 5.0f;
    public float timer = 0;
    public string name = "";
    protected Entity playerEntity;
    public virtual bool Exec(float time)
    {
        return true;
    }
    public void init(Entity playerEntity)
    {
        timer = 0;
        this.playerEntity = playerEntity;
    }
}

public class DissapearAbility : Ability
{
    
    public DissapearAbility()
    {
        name = "Dissapear";
    }

    public override bool Exec(float time)
    {
        if (timer == 0)
        {
            playerEntity.Dissapear();
        }
        playerEntity.hiddenInPlainSight = true;
        timer += time;
        if (timer > abilityDuration)
        {
            playerEntity.Reappear();
            playerEntity.hiddenInPlainSight = false;
            timer = 0;
            return true;
        }
        return false;

    }
}
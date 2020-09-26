using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PremadeSpellsManager : NetworkBehaviour
{
    [SerializeField] public Dictionary<int, Spell> mPremadeSpells = new Dictionary<int, Spell>();

    private static PremadeSpellsManager _instance;
    public static PremadeSpellsManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        LoadPremadeSpells();
    }

    private void LoadPremadeSpells()
    {
        //TODO cargar desde el server (db?)

        int lastSpellID = 0;
        // ************ BEGIN APOCALIPSIS ************
        // Effects
        List<Effect> effects = new List<Effect>();
        effects.Add(new ModifyAttribute(DefaultAttributeType.HEALTH, ValueModifierType.FIXED, 85));

        // Targets
        List<EffectTargetType> targets = new List<EffectTargetType>();
        targets.Add(EffectTargetType.OTHER_CHARACTER_ONLY);

        // Create the spell
        Spell newSpell = new Spell(lastSpellID, "Apocalipsis", "Rahma Nañarak O'al", "Causa daño a un usuario o NPC.", 1000, -90, effects, targets);
        PremadeSpellsManager._instance.mPremadeSpells.Add(lastSpellID, newSpell);
        lastSpellID += 1;
        // ************ END APOCALIPSIS ************
    }

    public static Spell GetSpellByID(int id)
    {
        return PremadeSpellsManager._instance.mPremadeSpells[id];
    }
}

using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : NetworkBehaviour
{
    // Spell slots
    public List<SpellSlot> mSpellSlots;

    // Current selected slot
    public SpellSlot mSelectedSlot = null;

    // Character reference
    private PlayableCharacter mCharacter;

    #region unity loop
    private void Awake()
    {
        mCharacter = GetComponent<PlayableCharacter>();

        // Initialize slots
        InitializeSlots();
    }
    #endregion

    public void LaunchSelectedSpell(Vector2 targetPosition)
    {
        if (mSelectedSlot != null && mSelectedSlot.mSpell != null)
        {
            //FIXME validar que tipo de target aplica al momento de lanzar el hechizo

            //FIXME validar si alcanza la mana
            Spell spell = mSelectedSlot.mSpell;
            // Apply all effects to the target
            foreach (Effect effect in spell.GetSpellEffects())
            {
                //FIXME elegir 1 type valido
                effect.ApplyTo(spell._validTargets[0], targetPosition);
            }

            Debug.Log("CASTED SPELL " + spell._name);

            //TODO determinar target character (si es que existe alguno en la posicion)

            // Show FX on all clients
            Connection.Instance.PlaySpellFX(spell, targetPosition, null);
        }
        else
        {
            Debug.Log("No Spell selected");
        }
    }

    public void SelectSlot(int newSlot)
    {
        mSelectedSlot = mSpellSlots[newSlot];
    }

    internal void AddNewSpell(Spell spell)
    {
        if (mSpellSlots == null) { InitializeSlots();  }

        foreach (SpellSlot slot in mSpellSlots)
        {
            if (slot.mSpell == null)
            {
                slot.mSpell = spell;
                break;
            }
        }
    }
    private void InitializeSlots()
    {
        //FIXME cargar tope de slots desde el server
        int max_slots = 3;
        mSpellSlots = new List<SpellSlot>();
        for (int i = 0; i < max_slots; i++)
        {
            mSpellSlots.Add(new SpellSlot());
        }
        //mSelectedSlot = mSpellSlots[0];
    }
}

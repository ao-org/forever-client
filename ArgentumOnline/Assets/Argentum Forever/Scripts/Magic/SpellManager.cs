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

    //Fixed size to 5 to avoid allocating/GC memory
    private Collider2D[] mOverlapBuffer = new Collider2D[5];

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

            int layerMask = 0;

            foreach (EffectTargetType targetType in spell._validTargets)
                layerMask |= GetLayerMaskFromTargetType(targetType);

            Vector2 spellDetectionExtent = Vector2.one / 4;

            int overlapCount = Physics2D.OverlapBoxNonAlloc(targetPosition, spellDetectionExtent, 0, mOverlapBuffer, layerMask);

            ExtDebug.DrawBox(targetPosition, spellDetectionExtent / 2, Quaternion.identity, Color.red);

            if (overlapCount > 0)
            {

                foreach (Effect effect in spell.GetSpellEffects())
                    effect.ApplyTo(ref mOverlapBuffer, overlapCount, targetPosition);

                Debug.Log("Casted Spell: " + spell._name);

                Connection.Instance.PlaySpellFX(spell, targetPosition, mOverlapBuffer[0].transform);
            }
            else
            {
                Debug.Log("No valid target for: " + spell._name);
            }
        }
        else
        {
            Debug.Log("No Spell selected");
        }
    }

    protected int GetLayerMaskFromTargetType(EffectTargetType targetType)
    {
        switch (targetType)
        {
            case EffectTargetType.ANY:
                return LayerMask.GetMask("");
            case EffectTargetType.ANY_CHARACTER:
                return LayerMask.GetMask("Players");
            case EffectTargetType.OTHER_CHARACTER_ONLY:
                return LayerMask.GetMask("Players");
            case EffectTargetType.SELF_CHARACTER_ONLY:
                return LayerMask.GetMask("Players");
            case EffectTargetType.EMPTY_TERRAIN_ONLY:
                return LayerMask.GetMask("");
            default:
                return 0;
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
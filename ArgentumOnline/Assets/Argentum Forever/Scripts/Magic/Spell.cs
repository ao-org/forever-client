using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell
{
    public int mPremadeID;
    public string mName;
    public string mMagicWords;
    public string mDescription;
    public int mRequiredMana;
    public int mRequiredStamina;

    public List<Effect> mEffects = new List<Effect>();

    public List<EffectTargetType> mValidTargets = new List<EffectTargetType>();

    public Spell(int id, string name, string magicWords, string desc, int reqMana, int reqStamina, List<Effect> effects, List<EffectTargetType> validTargets)
    {
        mPremadeID = id;
        mName = name;
        mMagicWords = magicWords;
        mDescription = desc;
        mRequiredMana = reqMana;
        mRequiredStamina = reqStamina;
        mEffects = effects;
        mValidTargets = validTargets;
    }

    public void ApplyEffectsTo(Character caster, Vector2 targetPosition)
    {
        //FIXME validar que tipo de target aplica al momento de lanzar el hechizo

        //FIXME validar si alcanza la mana

        // Apply all effects to the target
        foreach (Effect effect in mEffects)
        {
            //FIXME elegir 1 type valido
            effect.ApplyTo(mValidTargets[0], targetPosition);
        }

        UnityEngine.Debug.Log("CASTED SPELL " + mName);
        
        //TODO determinar target character (si es que existe alguno en la posicion)
        
        // Show FX on all clients
        Connection.Instance.PlaySpellFX(mPremadeID, targetPosition, null);
    }
}

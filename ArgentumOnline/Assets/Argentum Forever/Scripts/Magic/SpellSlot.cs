using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSlot
{
    // Associated spell
    public Spell mSpell;

    public void LaunchAssociatedSpell(PlayableCharacter caster, Vector2 targetPositon)
    {
        if (mSpell != null)
        {
            mSpell.ApplyEffectsTo(caster, targetPositon);
        }
    }

}

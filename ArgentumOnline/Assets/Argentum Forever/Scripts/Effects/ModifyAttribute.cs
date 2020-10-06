using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ModifyAttribute : Effect
{
    [SerializeField]
    private DefaultAttributeType mModifiedAttribute;
    [SerializeField]
    private ValueModifierType mType;
    [SerializeField]
    private int mValue;

    public override void ApplyTo(EffectTargetType targetType, Vector2 position)
    {
        // Check if the target type is valid for this effect
        if (TargetTypeValid(targetType))
        {
            //TODO obtener personaje en la posición indicada
            Character targetCharacter = null; //(Character) target.mTarget;
            Collider2D hitCollider = Physics2D.OverlapPoint(position);
            if (hitCollider != null)
            {
                targetCharacter = hitCollider.GetComponent<PlayableCharacter>();

                if (targetCharacter != null)
                {
                    targetCharacter.ModifyAttribute(mModifiedAttribute, mValue);
                }                
            }
        }
    }

    protected override bool TargetTypeValid(EffectTargetType targetType)
    {
        bool valid = false;
        if (targetType == EffectTargetType.ANY_CHARACTER || targetType == EffectTargetType.SELF_CHARACTER_ONLY || targetType == EffectTargetType.OTHER_CHARACTER_ONLY)
        {
            valid = true;
        }
        return valid;
    }
}

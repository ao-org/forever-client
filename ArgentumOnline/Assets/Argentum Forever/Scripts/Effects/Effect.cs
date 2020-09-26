using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect
{
    public abstract void ApplyTo(EffectTargetType targetType, Vector2 position);

    protected abstract bool TargetTypeValid(EffectTargetType targetType);
}

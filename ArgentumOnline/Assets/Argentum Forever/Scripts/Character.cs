using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : NetworkBehaviour
{
    public Dictionary<DefaultAttributeType, Attribute> mAttributes;
    public PaperdollManager mPaperdollManager;
    public EquipmentManager mEquipmentManager;
    public InventoryManager mInventoryManager;
    public SpellManager mSpellManager;

    private void Awake()
    {
        InitializeDefaultAttributes();
    }

    private void InitializeDefaultAttributes()
    {
        //TODO hacer bien canejo
        mAttributes = new Dictionary<DefaultAttributeType, Attribute>();
        mAttributes.Add(DefaultAttributeType.HEALTH, new Attribute(DefaultAttributeType.HEALTH, "Health", 20));         // 20 de vida inicial
        mAttributes.Add(DefaultAttributeType.MANA, new Attribute(DefaultAttributeType.MANA, "Mana", 20));               // 20 de mana inicial
    }

    internal void ModifyAttribute(DefaultAttributeType mModifiedAttribute, int mValue)
    {
        if (mAttributes == null) { InitializeDefaultAttributes(); }

        if (mAttributes.ContainsKey(mModifiedAttribute))
        {
            mAttributes[mModifiedAttribute].mCurrentValue += mValue;
            UnityEngine.Debug.Log("[Attribute modified " + mAttributes[mModifiedAttribute].mName + " = " + mAttributes[mModifiedAttribute].mCurrentValue);
            //TODO chequeo de post condiciones?
        }
    }

    public void LearnSpell(Spell spell)
    {
        mSpellManager.AddNewSpell(spell);
    }
}

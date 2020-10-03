using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] public int mCurrentMapID;

    [SyncVar]
    [SerializeField] public string mCharactername = "Unnamed Character";

    public Dictionary<DefaultAttributeType, Attribute> mAttributes;
    public PaperdollManager mPaperdollManager;
    public EquipmentManager mEquipmentManager;
    public InventoryManager mInventoryManager;
    public SpellManager mSpellManager;

    protected void Awake()
    {
        InitializeDefaultComponents();
        InitializeDefaultAttributes();
    }

    protected abstract void InitializeDefaultComponents();

    protected void InitializeDefaultAttributes()
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
    public void TeleportToMap(int mapID, Vector2 position, CardinalDirection direction)
    {
        mCurrentMapID = mapID;
        transform.position = new Vector3(position.x, position.y, transform.position.z);
        //TODO forzar direccion
        //TODO conciliar con el server
    }

    public void LearnSpell(Spell spell)
    {
        mSpellManager.AddNewSpell(spell);
    }

    public void EnteredMap(int mapID)
    {
        mCurrentMapID = mapID;
        //TODO conciliar con el server
    }

    public Vector2 GetPositionInCurrentMap()
    {
        // Real position
        Vector2 realPosition = new Vector2(transform.position.x, transform.position.y);

        //TODO realizar traduccion de las coordenadas del mapa actual a coordenadas del mundo

        // Return the real position
        return realPosition;
    }

    public void NotifyMeleeAttackToPaperdoll(bool started)
    {
        mPaperdollManager.NotifyMeleeAttackToPaperdoll(started);
    }

    public void UpdatePaperdoll(bool directionChanged, float horizontalSpeed, float verticalSpeed, float finalSpeed)
    {
        mPaperdollManager.UpdatePaperdoll(directionChanged, horizontalSpeed, verticalSpeed, finalSpeed);
    }

    public void LoadAnimationSet(StringAnimationClipDictionary animations, EquipmentSlotType slot)
    {
        mPaperdollManager.LoadAnimationSet(animations, slot);
    }

    public void CleanAnimationSet(EquipmentSlotType slot)
    {
        mPaperdollManager.CleanAnimationSet(slot);
    }

    public void DealDamage(int amount)
    {
        // Check prerequisites...
        if (!CanTargetPlayer(this)) return;

        // Subtract health from the current value
        mAttributes[DefaultAttributeType.HEALTH].mCurrentValue -= amount;

        // Check if the character is now dead
        if (mAttributes[DefaultAttributeType.HEALTH].mCurrentValue <= 0)
        {
            Kill();
        }
    }

    [Server]
    protected bool CanTargetPlayer(Character player)
    {
        // Check if the character is alive
        if (mAttributes[DefaultAttributeType.HEALTH].mCurrentValue <= 0)
        {
            return false;
        }

        return true;
    }

    protected void Kill()
    {
        Debug.Log($"character {netId} died!");
    }
}

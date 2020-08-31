using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 1)]
public class Item : ScriptableObject
{
    // Item name
    [SerializeField] private string mName;

    // Item description
    [SerializeField] private string mDescription;

    // Base price at NPCs
    [SerializeField] private int nBasePrice;

    // Item weight in "stones" ;)
    [SerializeField] private int mWeight;

    // Item type (consumable/equipable)
    [SerializeField] private ItemType mType;

    // Allowed slots
    [SerializeField] private EquipmentSlotType[] allowedSlots;

    // Animation clips (paperdoll system)
    [SerializeField] public AnimationClip[] mAnimationClips;

    public Item()
    {
        //TODO precargar lista de animaciones necesarias
        //animationClips = new AnimationClip[10];
        //for (int i = 0; i < 10; i++)
        //{
        //    animationClips[i] = null;
        //}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperdollTesting : MonoBehaviour
{
    [SerializeField] private LocalPlayerMovement mPlayer;
    [SerializeField] private Item monkRobes;
    [SerializeField] private Item newbieClothes;

    public void FreeTorsoSlot()
    {
        mPlayer.GetComponent<EquipmentManager>().UnequipItemInSlot(EquipmentSlotType.TORSO);
    }

    public void EquipMonkRobes()
    {
        mPlayer.GetComponent<EquipmentManager>().EquipItem(monkRobes, EquipmentSlotType.TORSO);
    }

    public void EquipNewbieClothes()
    {
        mPlayer.GetComponent<EquipmentManager>().EquipItem(newbieClothes, EquipmentSlotType.TORSO);
    }
}

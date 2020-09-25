using Cinemachine;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacter : NetworkBehaviour
{
    private PaperdollManager mPaperdollManager;
    public EquipmentManager mEquipmentManager;
    public InventoryManager mInventoryManager;

    [SerializeField] private GameObject apocaPrefab;

    #region synchronized variables
    [SyncVar]
    [SerializeField] public int mCurrentMapID;

    [SyncVar]
    [SerializeField] public string mCharactername = "Unnamed Character";

    [SyncVar]
    [SerializeField] public int mMaxHealth;

    [SyncVar]
    [SerializeField] public int mCurrentHealth;

    [SyncVar]
    [SerializeField] public int mMaxMana;

    [SyncVar]
    [SerializeField] public int mCurrentMana;
    #endregion

    #region unity loop
    private void Awake()
    {
        DontDestroyOnLoad(transform);

        mPaperdollManager = transform.GetComponentInChildren<PaperdollManager>();
        mEquipmentManager = transform.GetComponentInChildren<EquipmentManager>();
        mInventoryManager = transform.GetComponentInChildren<InventoryManager>();
    }
    #endregion

    public void TeleportToMap(int mapID, Vector2 position, CardinalDirection direction)
    {
        mCurrentMapID = mapID;
        transform.position = new Vector3(position.x, position.y, transform.position.z);
        //TODO forzar direccion
        //TODO conciliar con el server
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

    public override void OnStartLocalPlayer()
    {
        CinemachineVirtualCamera rVcam = FindObjectOfType<CinemachineVirtualCamera>();
        rVcam.m_Follow = gameObject.transform;

        //TODO KILLME TEST
        //Instantiate(GetComponent<TestingGUI>().mPaperdollTestingGUI, transform);
        mInventoryManager.AddItem(GetComponent<TestingGUI>().mMonkRobes);
        mInventoryManager.AddItem(GetComponent<TestingGUI>().mNewbieClothes);
        mInventoryManager.AddItem(GetComponent<TestingGUI>().mWoodenBucker);
        mInventoryManager.AddItem(GetComponent<TestingGUI>().mWoodenSword);
        mInventoryManager.AddItem(GetComponent<TestingGUI>().mHelmet);
        Instantiate(GetComponent<TestingGUI>().mCharacterInfoGUI, transform);
        Instantiate(GetComponent<TestingGUI>().mItemsGUI, transform);
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

    [ClientRpc]
    public void RpcProcessEquipedItem(int itemID, int slotID)
    {
        // Get animation set from the item ID
        Item item = ItemManager.GetItemByID(itemID);

        // Update equipment slot
        mEquipmentManager.mEquipmentSlots[item.mAllowedSlots[slotID]].EquipItem(item);

        // Update paperdoll
        LoadAnimationSet(item.mAnimationClips, item.mAllowedSlots[slotID]);

        // TODO actualizar stats...
    }

    public void ProcessUnequipedItem(Item item, EquipmentSlotType slot)
    {
        // Update paperdoll
        CleanAnimationSet(slot);

        // TODO actualizar stats... validar..
    }

    public void DealDamage(int amount)
    {
        if (!CanTargetPlayer(this)) return;

        mCurrentHealth -= amount;

        RpcApocaReceived();
        if (mCurrentHealth <= 0)
        {
            Kill();
        }
    }

    [ClientRpc]
    void RpcApocaReceived()
    {
        Instantiate(apocaPrefab, transform);
    }

    [Server]
    private bool CanTargetPlayer(PlayableCharacter player)
    {
        if (player.mCurrentHealth <= 0)
        {
            Debug.Log("player is dead!");
            return false;
        }

        return true;
    }

    private void Kill()
    {
        Debug.Log($"player {netId} died!");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MagicGUI : MonoBehaviour
{
    // Character reference
    PlayableCharacter mCharacter;

    public bool mTargetingEnabled = false;

    void Start()
    {
        mCharacter = GetComponentInParent<PlayableCharacter>();
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //FIXME actualizar solo cuando haga falta

        // Update spell slots
        UpdateSpellList();

        // FIXME no va aca
        CheckSpellCast();
    }

    private void UpdateSpellList()
    {
        int slotN = 0;
        foreach (SpellSlot slot in mCharacter.mSpellManager.mSpellSlots)
        {
            Transform guiSlot = transform.GetChild(0).GetChild(slotN);

            // Update slot text
            if (slot.mSpell != null)
            {
                transform.GetChild(0).GetChild(slotN).GetComponent<Text>().text = slot.mSpell.mName;
            }
            else
            {
                transform.GetChild(0).GetChild(slotN).GetComponent<Text>().text = "(SLOT " + slotN + ")";
            }
            
            slotN++;
        }

        // Clear empty inventory slots (in GUI)
        for (int i = slotN; i < transform.GetChild(0).childCount - 1; i++)
        {
            // Clear slot
            transform.GetChild(0).GetChild(slotN).GetComponent<Text>().text = "(SLOT " + slotN + ")";
        }
    }

    public void SelectSlot(int slotNumber)
    {
        mCharacter.UpdateSelectedSpellSlot(slotNumber);

        foreach (Transform guiSlot in transform.GetChild(0))
        {
            guiSlot.GetComponent<Text>().color = Color.white;
        }

        transform.GetChild(0).GetChild(slotNumber).GetComponent<Text>().color = Color.green;
    }

    public void EnableTargetingMode()
    {
        mTargetingEnabled = true;
    }

    private void CheckSpellCast()
    {
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (mTargetingEnabled)
            {
                // Check if it hits something before sending to the server
                // If I didn't check, it would generate traffic unnecesarily
                Camera mainCamera = FindObjectOfType<Camera>();
                Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mCharacter.LaunchSelectedSpell(mousePos);
            }
            mTargetingEnabled = false;
        }
    }
}

using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : NetworkBehaviour
{
    [SerializeField] private GameObject mConnectionPrefab;
    [SerializeField] private AudioManager mAudioManager;
    [SerializeField] private VisualEffectsManager mVisualEffectsManager;

    private static Connection _instance;
    public static Connection Instance { get { return _instance; } }

    private void InitializeSingleton()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Awake()
    {

        InitializeSingleton();
        mVisualEffectsManager = GameObject.FindObjectOfType<VisualEffectsManager>();
    }

    public void PlaySound(int soundID)
    {
        //TODO delegar al audio manager
    }

    public void PlaySpellFX(int fxID, Vector2 position, Transform attachedTo)
    {
        CmdPlaySpellFX(fxID, position, attachedTo);
    }

    [Command(ignoreAuthority = true)]
    private void CmdPlaySpellFX(int fxID, Vector2 position, Transform attachedTo)
    {
        RpcPlaySpellFX(fxID, position, attachedTo);
    }

    [ClientRpc]
    private void RpcPlaySpellFX(int fxID, Vector2 position, Transform attachedTo)
    {
        mVisualEffectsManager.PlaySpellFX(fxID, position, attachedTo);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffectsManager : MonoBehaviour
{
    [SerializeField] public List<GameObject> mPremadeSpellsFX = new List<GameObject>();

    private static VisualEffectsManager _instance;
    public static VisualEffectsManager Instance { get { return _instance; } }

    private void Awake()
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

    public void PlaySpellFX(int fxID, Vector2 position, Transform attachedTo)
    {
        if (attachedTo != null)
        {
            Instantiate(mPremadeSpellsFX[fxID], attachedTo);
        }
        else
        {
            Instantiate(mPremadeSpellsFX[fxID], position, Quaternion.identity);
        }
    }
}

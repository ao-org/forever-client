using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterDebugGUI : MonoBehaviour
{
    // Player GO reference
    [SerializeField]  private GameObject mPlayer;

    [SerializeField] private TextMeshProUGUI mCharacterName;
    [SerializeField] private TextMeshProUGUI mMap;
    [SerializeField] private TextMeshProUGUI mPosition;
    [SerializeField] private TextMeshProUGUI mFPS;

    private float deltaTime;

    // Start is called before the first frame update
    private void Start()
    {
        mCharacterName.text = mPlayer.GetComponent<CharacterInfo>().mCharactername;
    }

    // Update is called once per frame
    void Update()
    {
        mMap.text = mPlayer.GetComponent<CharacterInfo>().mCurrentMapID.ToString();
        mPosition.text = mPlayer.GetComponent<CharacterInfo>().GetPositionInCurrentMap().ToString();

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        mFPS.text = Mathf.Ceil(fps).ToString();
    }
}

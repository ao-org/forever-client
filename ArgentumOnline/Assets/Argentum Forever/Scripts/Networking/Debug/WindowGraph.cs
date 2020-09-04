using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour
{
    [SerializeField] private Sprite mSprite;
    private RectTransform mGraphContainer;
    private Texture2D mTexture;
    private List<GameObject> mBars;

    private void Awake(){
        mTexture = new Texture2D(1,1);
        mTexture.SetPixel(1,1,new Color(1,1,1,0.5f));
        mTexture.Apply();
        mSprite = Sprite.Create(mTexture, new Rect(0, 0, 1, 1), new Vector2(0, 0));
        mGraphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        mBars = new List<GameObject>();
        List<int> valueList = new List<int>() { 5,78,56,45,30,22,17,15,13,17,25,37,40,36,33};
        ShowGraph(valueList);
    }

    private GameObject CreateBar(Vector2 Pos, float barWidth){
        Debug.Assert(mGraphContainer!=null);
        float graphHeight = mGraphContainer.sizeDelta.y;
        GameObject gameObject = new GameObject("bar",typeof(Image));
        gameObject.transform.SetParent(mGraphContainer,false);
        gameObject.GetComponent<Image>().sprite = mSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Pos;
        rectTransform.sizeDelta = new Vector2(barWidth, graphHeight- (graphHeight-Pos.y));
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        return gameObject;
    }
    private void ShowGraph(List<int> valueList){
        float barWidth = 10f;
        float xSep = 5f;
        float xSize =20f;
        float yMaximum =200f;
        float graphHeight = mGraphContainer.sizeDelta.y;
        Debug.Log("graphHeight " + mGraphContainer.sizeDelta.y);
        GameObject lastCircle = null;
        for(int i=0; i< valueList.Count;++i){
            float xPosition = xSize + i * xSize ;
            float yPosition = (valueList[i]/yMaximum)*graphHeight;
            mBars.Add(CreateBar(new Vector2(xPosition,yPosition),barWidth));
            /*
            if(lastCircle!=null){
                CreateDotConnection(lastCircle.GetComponent<RectTransform>().anchoredPosition,
                    circleGameObj.GetComponent<RectTransform>().anchoredPosition);


            }
            lastCircle = circleGameObj;
            */
        }
    }
    void Update()
    {
        /*
        for (var i = 0; i < mBars.Count; i++) {
                Destroy(mBars[i]);
        }
        List<int> valueList = new List<int>();
        for(int i=0; i< 20; ++i){
            valueList.Add(Random.Range(5, 100));
        }
        ShowGraph(valueList);
        */
    }

    private void CreateDotConnection(Vector2 dotPositionA,Vector2 dotPositionB){
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(mGraphContainer,false);
        gameObject.GetComponent<Image>().color = new Color(1,1,1,0.5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100,3f);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.anchoredPosition = dotPositionA;
    }
}

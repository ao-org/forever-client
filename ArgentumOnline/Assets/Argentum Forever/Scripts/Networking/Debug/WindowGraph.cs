using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour
{
    [SerializeField] private Sprite mCircleSprite;
    private RectTransform mGraphContainer;


    private void Awake(){
        mGraphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        CreateCircle(new Vector2(100,100));
        List<int> valueList = new List<int>() { 5,98,56,45,30,22,17,15,13,17,25,37,40,36,33};
        ShowGraph(valueList);
    }

    private void CreateCircle(Vector2 Pos){
        Debug.Assert(mGraphContainer!=null);
        GameObject gameObject = new GameObject("circle",typeof(Image));
        gameObject.transform.SetParent(mGraphContainer,false);
        gameObject.GetComponent<Image>().sprite = mCircleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Pos;
        rectTransform.sizeDelta = new Vector2(11,11);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;


    }




    private void ShowGraph(List<int> valueList){
        float xSize =50f;
        float yMaximum =200f;
        float graphHeight = mGraphContainer.sizeDelta.y;
        for(int i=0; i< valueList.Count;++i){
            float xPosition = i * xSize;
            float yPosition = (valueList[i]/yMaximum)*graphHeight;
            CreateCircle(new Vector2(xPosition,yPosition));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prueba : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool ok = true;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LateUpdate()
    {
        if (spriteRenderer.isVisible)
        {
            Bounds bounds = spriteRenderer.bounds;
            var pivotX = -bounds.center.x / bounds.extents.x / 2 + 0.5f;
            var pivotY = -bounds.center.y / bounds.extents.y / 2 + 0.5f;
            //var pixelsToUnits = spriteRenderer.textureRect.width / bounds.size.x;
            Vector3 v3 = new Vector3(pivotX, pivotY,0 );
            //spriteRenderer.sortingOrder = (int)Camera.main.WorldToScreenPoint(spriteRenderer.bounds.min).y * -1;
            spriteRenderer.sortingOrder = (int)Camera.main.WorldToScreenPoint(transform.position).y * -1;
            if (ok)
            {
                UnityEngine.Debug.Log("##########################################################");
                UnityEngine.Debug.Log("Name: " + transform.name);
                UnityEngine.Debug.Log("bound min X: " + spriteRenderer.bounds.min.x.ToString() + " Y: " + spriteRenderer.bounds.min.y.ToString());
                UnityEngine.Debug.Log("bound min X: " + spriteRenderer.bounds.max.x.ToString() + " Y: " + spriteRenderer.bounds.max.y.ToString());
                UnityEngine.Debug.Log("bound center X: " + spriteRenderer.bounds.center.x.ToString() + " Y: " + spriteRenderer.bounds.center.y.ToString());
                UnityEngine.Debug.Log("bound Transform X: " + transform.position.x.ToString() + " Y: " + transform.position.y.ToString());
                UnityEngine.Debug.Log("##########################################################");
                ok = false;
            }
        }
        //spriteRenderer.sortingOrder = (int)transform.position.y * -1;
    }
}

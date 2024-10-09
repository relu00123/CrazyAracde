using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSorting : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public GameObject renderBase;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        Transform RenderBaseTranform = transform.parent.Find("RenderBase");

        if (RenderBaseTranform != null)
            renderBase = RenderBaseTranform.gameObject;
           

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<CAPlayerRender>() != null)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-(transform.position.y  + 0.5f)* 100);

        }

        else if (renderBase != null)
        {
            SpriteRenderer baseRenderer = renderBase.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = baseRenderer.sortingOrder;
        }

        else
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        }
    }
}

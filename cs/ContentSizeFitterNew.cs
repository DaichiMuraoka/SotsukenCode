using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentSizeFitterNew : MonoBehaviour
{
    [SerializeField] private RectTransform horizontalBase = null;

    [SerializeField] private float horAdjust = 100;

    [SerializeField] private RectTransform verticalBase = null;

    [SerializeField] private float verAdjust = 100;

    private RectTransform rectTransform = null;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        if(horizontalBase != null) rectTransform.sizeDelta = new Vector2(horizontalBase.sizeDelta.x + horAdjust, rectTransform.sizeDelta.y);
        if(verticalBase != null) rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, verticalBase.sizeDelta.y + verAdjust);
    }
}

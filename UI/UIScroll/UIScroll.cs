using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScroll : MonoBehaviour {
    public GameObject mask = null;
    public GameObject contents = null;
    public GameObject scrollBarParents = null;
    public GameObject scrollBar = null;

    public float wheelWeight = 100;

    float maskSize = 0;
    float contentsSize = 0;
    float barSizeMax = 0;

    float scrollRatio = 0;
    float scrollStartPos = 0;
    float scrollMaxPos = 0;
    float mousePos = 0;

    private void Awake() {
        RectTransform rt = mask.GetComponent<RectTransform>();
        maskSize = rt.sizeDelta.y;
        rt = contents.GetComponent<RectTransform>();
        contentsSize = rt.sizeDelta.y;
        rt = scrollBar.GetComponent<RectTransform>();
        barSizeMax = rt.sizeDelta.y;

        InitBarScale();
    }

    public float GetContentsSize() { return contentsSize; }
    public void SetSize(float x, float y) {
        this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
    }

    public void SetMaskSize(float x, float y) {
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);

        RectTransform contentsRt = contents.GetComponent<RectTransform>();
        contentsRt.sizeDelta = new Vector2(x, contentsRt.sizeDelta.y);
        InitBarScale();
    }

    public void SetContentsSize(float y) {
        contentsSize = y;

        RectTransform contentsRt = contents.GetComponent<RectTransform>();
        contentsRt.sizeDelta = new Vector2(contentsRt.sizeDelta.x, y);

        InitBarScale();
    }

    void InitBarScale() {
        if (maskSize >= contentsSize) {
            scrollRatio = 0;
            scrollBarParents.SetActive(false);

            RectTransform contentsRt = contents.GetComponent<RectTransform>();
            contentsRt.anchoredPosition = new Vector2(contentsRt.anchoredPosition.x, contentsSize - maskSize);
        } else {
            scrollBarParents.SetActive(true);

            RectTransform barRt = scrollBar.GetComponent<RectTransform>();
            float size = barSizeMax * (maskSize / contentsSize);
            scrollMaxPos = size - barSizeMax;
            barRt.sizeDelta = new Vector2(barRt.sizeDelta.x, size);
            SetScrollPos(scrollRatio);
        }
    }

    public void BarOnDown() {
        mousePos = Input.mousePosition.y;
        scrollStartPos = scrollBar.transform.localPosition.y;
    }

    public void BarDrag() {
        float movePos = scrollStartPos + ((Input.mousePosition.y - mousePos) / this.transform.localScale.y);
        SetScrollPos(movePos / scrollMaxPos);
    }

    public void OnWheel(float v) {
        if (maskSize >= contentsSize) return;
        SetScrollPos(scrollRatio + (-v * 10 * (wheelWeight / (contentsSize - maskSize))));
        ClickObject.CheckOver();
    }

    public void SetScrollPos(float v) {
        if (maskSize >= contentsSize) return;
        if (v < 0) v = 0;
        if (v > 1) v = 1;
        scrollBar.transform.localPosition = new Vector3(scrollBar.transform.localPosition.x, scrollMaxPos * v);
        RectTransform contentsRt = contents.GetComponent<RectTransform>();
        contentsRt.anchoredPosition = new Vector2(contentsRt.anchoredPosition.x, (contentsSize - maskSize) * v);
        scrollRatio = v;
    }
}

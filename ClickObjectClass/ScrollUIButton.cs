using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScrollUIButton : ClickObject {
    public float setMaxX = 0;
    public float setViewX = 0;
    public float setMaxY = 0;
    public float setViewY = 0;

    bool initCheck = false;
    float sizeX = 0;
    float sizeY = 0;
    float posX = 0;
    float posY = 0;
    float posXMax = 0;
    float posYMax = 0;

    public void SetScrollSize(float maxX = 0, float viewX = 0, float maxY = 0, float viewY = 0) {
        RectTransform rt = this.gameObject.GetComponent<RectTransform>();

        if (initCheck == false) {
            posX = rt.localPosition.x;
            posY = rt.localPosition.y;
            sizeX = rt.sizeDelta.x;
            sizeY = rt.sizeDelta.y;
            initCheck = true;
        }

        if (maxX == 0) {
            posXMax = 0;
        } else {
            posXMax = (sizeX - (viewX / maxX * sizeX)) / 2;
        }
        if (maxY == 0) {
            posYMax = 0;
        } else {
            posYMax = (sizeY - (viewY / maxY * sizeY)) / 2;
        }

        float rtSizeX = maxX > 0 ? viewX / maxX : 1;
        if (rtSizeX < 0.1) rtSizeX = 0.1f;
        float rtSizeY = maxY > 0 ? viewY / maxY : 1;
        if (rtSizeY < 0.1) rtSizeY = 0.1f;
        rt.localScale = new Vector2(rtSizeX, rtSizeY);

        rt.localPosition = new Vector3(posX - posXMax, posY + posYMax);
    }

    float startMoustPosX = 0;
    float startMoustPosY = 0;
    float startScrollPosX = 0;
    float startScrollPosY = 0;
    bool drag = false;
    override protected void OnUpdateEvent() {
        if (drag) {
            RectTransform rt = this.gameObject.GetComponent<RectTransform>();
            float posTempX = startScrollPosX + Input.mousePosition.x - startMoustPosX;
            if (posTempX < posX - posXMax) posTempX = posX - posXMax;
            if (posTempX > posX + posXMax) posTempX = posX + posXMax;

            float posTempY = startScrollPosY + Input.mousePosition.y - startMoustPosY;
            if (posTempY < posY - posYMax) posTempY = posY - posYMax;
            if (posTempY > posY + posYMax) posTempY = posY + posYMax;

            if (rt.localPosition.x != posTempX || rt.localPosition.y != posTempY) {
                rt.localPosition = new Vector3(posTempX, posTempY);

                float vx = posXMax != 0 ? (posTempX - (posX - posXMax)) / (posXMax * 2) : 0;
                float vy = posYMax != 0 ? ((posY + posYMax) - posTempY) / (posYMax * 2) : 0;

                moveScrollFn(vx, vy);
            }
        }
    }
    override protected void OnDownEvent(_mouseType button) {
        if (button == _mouseType.l) {
            drag = true;
            startMoustPosX = Input.mousePosition.x;
            startMoustPosY = Input.mousePosition.y;

            RectTransform rt = this.gameObject.GetComponent<RectTransform>();
            startScrollPosX = rt.localPosition.x;
            startScrollPosY = rt.localPosition.y;
        }
    }
    override protected void OnUpEvent(_mouseType button) {
        if (button == _mouseType.l) {
            drag = false;
        }
    }

    virtual protected void moveScrollFn(float x, float y) { 
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class moveUIScrollBar : ScrollUIButton {
    public GameObject moveTarget = null;

    public float moveMinX = 0;
    public float moveMaxX = 0;
    public float moveMinY = 0;
    public float moveMaxY = 0;

    override protected void moveScrollFn(float x, float y) {
        if (moveTarget == null) return;
        float posX = (moveMaxX - moveMinX) * x + moveMinX;
        float posY = (moveMaxY - moveMinY) * y + moveMinY;

        moveTarget.GetComponent<RectTransform>().localPosition = new Vector3(posX, posY);
    }
}

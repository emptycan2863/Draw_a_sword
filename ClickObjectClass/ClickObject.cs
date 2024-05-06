/**
 * ClickObject 는 마우스와 오브젝트간의 상호작용을 관리하는 클래스이며, 해당 클래스를 상속시켜 다양한 오브젝트를 만들 수 있다.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum _mouseType { l, r, h, length };

public class ClickObject : MonoBehaviour {
    enum _objType { none, uiRect, uiPolygon, spritePolygon };

    GameObject thisObj = null;
    _objType objType = _objType.none;
    public bool isDefaultClickEvent = false;
    public bool isDefaultClickEventR = true;
    public bool isMaskingOnly = false;
    public bool clickActive = true;
    public GameObject clickMaskingGo = null;
    ClickObject clickMasking = null;
    UIScroll parentScroll = null;

    bool IsActive() {
        if (!thisObj) return false;
        if (!ObjectActive(thisObj)) return false;
        if (!clickActive) return false;
        return true;
    }

    bool ObjectActive(GameObject obj) {
        if (!obj.activeSelf) return false;
        if (obj.transform.parent != null) {
            return ObjectActive(obj.transform.parent.gameObject);
        }
        return true;
    }

    bool IsDestroyed() {
        if (!thisObj) return true;
        if (thisObj.IsDestroyed()) return true;
        return false;
    }

    void Awake() {
        Awake_clickObject();
    }

    protected void Awake_clickObject() {
        thisObj = gameObject;
        SetMaskingObject(clickMaskingGo);
        Init();
        clickObjectList.Add(this);
    }

    void Update() {
    }

    public void SetMaskingObject(GameObject go) {
        if (go != null) {
            clickMasking = go.GetComponent<ClickObject>();
        }
    }

    List<float> polygonPoints = new List<float>();
    void Init() {
        RectTransform thisRectTransform = GetComponent<RectTransform>();
        if (thisRectTransform) {
            PolygonCollider2D polygonUI = GetComponent<PolygonCollider2D>();
            if (polygonUI) {
                objType = _objType.uiPolygon;
                int len = polygonUI.points.Length;
                while (polygonPoints.Count < len * 2) polygonPoints.Add(0);
                return;
            }


            objType = _objType.uiRect;
            while (polygonPoints.Count < 8) polygonPoints.Add(0);
            return;
        }

        PolygonCollider2D polygon = GetComponent<PolygonCollider2D>();
        if (polygon) {
            objType = _objType.spritePolygon;
            int len = polygon.points.Length;
            while (polygonPoints.Count < len*2) polygonPoints.Add(0);
            return;
        }
    }

    //마우스 충돌 체크에서 체크 우선순위를 정하기위한 점수를 수치화 시켜서 반환한다. 월드에 배치된 오브젝트는 Z좌표 순으로, UI에 배치된 오브젝트는 페어런츠 정보를 해석하여 반환한 수치로 계산한다.
    float GetOrderLevel() {
        float level = 0;

        switch (objType) {
            case _objType.uiRect:
            case _objType.uiPolygon:
                level += 1000;
                Transform transformTemp = thisObj.transform;
                Transform parent = transformTemp.parent;
                while (parent != null) {
                    Canvas canvas = parent.gameObject.GetComponent<Canvas>();
                    if (canvas != null) {
                        level += canvas.sortingOrder * 1000;
                        break;
                    }
                    parent = parent.parent;
                }
                float levelTemp = 0;

                while (transformTemp) {
                    levelTemp += (float)transformTemp.GetSiblingIndex() + 1;
                    transformTemp = transformTemp.parent;

                    if (transformTemp) {
                        levelTemp = levelTemp / (transformTemp.childCount + 1);
                    } else {
                        level += levelTemp;
                    }
                }
                break;
            case _objType.spritePolygon:
                level += -thisObj.transform.position.z;
                break;
        }
        return level;
    }

    //해당 오브젝트의 마우스 인식 범위를 폴리곤 형 데이터로 반환받는다.
    List<float> GetPoints() {
        Vector2 vecTemp = Vector2.zero;
        PolygonCollider2D polygon;
        RectTransform thisRectTransform;

        switch (objType) {
            case _objType.uiRect :
                thisRectTransform = GetComponent<RectTransform>();
                float w = thisRectTransform.rect.width * thisRectTransform.lossyScale.x;
                float h = thisRectTransform.rect.height * thisRectTransform.lossyScale.y;
                Vector2 pivot = thisRectTransform.pivot;

                polygonPoints[0] = -w * pivot.x;
                polygonPoints[1] = h * (1 - pivot.y);
                polygonPoints[2] = w * (1 - pivot.x);
                polygonPoints[3] = h * (1 - pivot.y);
                polygonPoints[4] = w * (1 - pivot.x);
                polygonPoints[5] = -h * pivot.y;
                polygonPoints[6] = -w * pivot.x;
                polygonPoints[7] = -h * pivot.y;

                for (int i = 0, len = polygonPoints.Count/2; i < len; ++i) {
                    vecTemp.x = polygonPoints[i * 2];
                    vecTemp.y = polygonPoints[i * 2 + 1];
                    vecTemp = Quaternion.Euler(0, 0, thisRectTransform.transform.eulerAngles.z) * vecTemp;
                    polygonPoints[i * 2] = vecTemp.x + thisRectTransform.position.x;
                    polygonPoints[i * 2 + 1] = vecTemp.y + thisRectTransform.position.y;
                }
                break;
            case _objType.uiPolygon:
                polygon = GetComponent<PolygonCollider2D>();
                thisRectTransform = GetComponent<RectTransform>();
                for (int i = 0, len = polygon.points.Length; i < len; ++i) {
                    vecTemp = Quaternion.Euler(0, 0, thisObj.transform.eulerAngles.z) * (polygon.points[i] * thisObj.transform.lossyScale);
                    polygonPoints[i * 2] = vecTemp.x + thisRectTransform.position.x;
                    polygonPoints[i * 2 + 1] = vecTemp.y + thisRectTransform.position.y;
                }
                break;
            case _objType.spritePolygon:
                polygon = GetComponent<PolygonCollider2D>();
                for (int i = 0, len = polygon.points.Length; i<len; ++i) {
                    vecTemp = Quaternion.Euler(0, 0, thisObj.transform.eulerAngles.z) * (polygon.points[i] * thisObj.transform.lossyScale);
                    vecTemp = GameManager.mainCamera.WorldToScreenPoint(vecTemp + (Vector2)thisObj.transform.position);
                    polygonPoints[i * 2] = vecTemp.x;
                    polygonPoints[i * 2 + 1] = vecTemp.y;
                }
                break;
        }
        return polygonPoints;
    }

    //해당 오브젝트의 마우스 인식 범위를 폴리곤 형 데이터로 반환받는다.
    protected bool MouseOnOverCheck() {
        float x = mousePosX;
        float y = mousePosY;
        List<float> points = GetPoints();
        if (points == null) return false;
        if (points.Count < 6) return false;

        bool isIn = false;
        for (int i = 0, len = points.Count / 2; i < len; ++i) {
            float point1X = points[i * 2];
            float point1Y = points[i * 2 + 1];
            float point2X;
            float point2Y;
            if (i + 1 == len)
            {
                point2X = points[0];
                point2Y = points[1];
            } else {
                point2X = points[i * 2 + 2];
                point2Y = points[i * 2 + 3];
            }

            if (y < point1Y && y < point2Y) continue;
            if (y > point1Y && y > point2Y) continue;

            float xTemp = (point2X - point1X)*(point1Y - y)/(point1Y - point2Y) + point1X;
            if (xTemp < x) isIn = !isIn;
        }

        if (isIn && clickMasking != null && clickMasking.IsActive()) {
            if (clickMasking.maskingCheck == -1) clickMasking.maskingCheck = clickMasking.MouseOnOverCheck() ? 1 : 0;
            if (clickMasking.maskingCheck == 0) isIn = false;
        }

        return isIn;
    }

    int maskingCheck = -1; // -1 체크 필요, 0 체크 아웃, 1 체크 인

    //스태틱 함수 및 변수는 는 매니저 개념으로 관리한다.---------------------------------------------------------------------
    static List<ClickObject> clickObjectList = new List<ClickObject>();
    static float mousePosX = 0;
    static float mousePosY = 0;

    static List<ClickObject> orderList = new List<ClickObject>();
    static List<float> orderLevelList = new List<float>();

    static ClickObject onOverTarget = null;
    static bool defaultClick = false;

    static bool[] mouseButtonList = new bool[3] { false, false, false };
    static bool isDrag = false;
    static _mouseType dragButton = _mouseType.l;

    static ClickObject onDragOverTarget = null;
    static float wheelAxis = 0;

    public static ClickObject GetDragOverTarget() {
        return onDragOverTarget;
    }

    static public void GetWheelAxis(float v) {
        wheelAxis += v;
    }

    static public void UpdateClickObjectManager() {
        if (!UIManager.UIClickCheck()) return;

        bool checkOver = false;

        if (goCheck) {
            goCheck = false;
            checkOver = true;
        }

        if (mousePosX != Input.mousePosition.x) {
            mousePosX = Input.mousePosition.x;
            checkOver = true;
        }
        if (mousePosY != Input.mousePosition.y)
        {
            mousePosY = Input.mousePosition.y;
            checkOver = true;
        }

        for (int i = clickObjectList.Count - 1; i >= 0; --i) {
            ClickObject obj = clickObjectList[i];
            if (!obj.IsActive()) {
                if (obj.IsDestroyed()) clickObjectList.RemoveAt(i);
                continue;
            }
            obj.maskingCheck = -1;
            obj.OnUpdateEvent();
            if(checkOver) obj.OnMouseMoveUpdateEvent();
        }

        if (checkOver) {
            if (isDrag) {
                if (onOverTarget) onOverTarget.OnDragEvent(dragButton);
                checkDragOverFn();
            } else {
                CheckOverFn(); 
            }
        }

        for (_mouseType i = 0, len = _mouseType.length; i < len; ++i) {
            if (mouseButtonList[(int)i] != Input.GetMouseButton((int)i)) {
                if (Input.GetMouseButton((int)i)) {
                    if (isDrag && dragButton != i) {
                        mouseUpFn(dragButton);
                    }

                    mouseDownFn(i);
                } else {
                    if (isDrag && dragButton == i) {
                        mouseUpFn(i);
                    }
                }
            }
            mouseButtonList[(int)i] = Input.GetMouseButton((int)i);
        }

        if (wheelAxis != 0 && onOverTarget) onOverTarget.OnWheelEvent(wheelAxis);
        wheelAxis = 0;
    }

    static bool goCheck = false;
    static public void CheckOver() {
        goCheck = true;
    }

    static void CheckOverFn() {
        orderList.Clear();
        orderLevelList.Clear();

        ClickObject onOverTargetTemp = null;

        for (int i = 0, len = clickObjectList.Count; i < len; ++i) {
            ClickObject obj = clickObjectList[i];
            if (obj.isMaskingOnly) continue;
            if (!obj.IsActive()) continue;
            float orderLevel = obj.GetOrderLevel();

            if (orderLevelList.Count == 0) {
                orderLevelList.Add(orderLevel);
                orderList.Add(obj);
            } else {
                int index = 0;
                for (int len1 = orderLevelList.Count; index < len1; ++index) {
                    if (orderLevelList[index] < orderLevel) break;
                }
                orderLevelList.Insert(index, orderLevel);
                orderList.Insert(index, obj);
            }
        }

        for (int i = 0, len = orderList.Count; i < len; ++i) {
            ClickObject obj = orderList[i];
            if (obj.MouseOnOverCheck()) {
                onOverTargetTemp = obj;
                break;
            }
        }

        if (onOverTarget != onOverTargetTemp) {
            if (onOverTarget) onOverTarget.OnOutEvent();
            if (onOverTargetTemp) onOverTargetTemp.OnOverEvent();
            onOverTarget = onOverTargetTemp;
        }
    }

    static protected void checkDragOverFn() {
        orderList.Clear();
        orderLevelList.Clear();

        ClickObject onOverTargetTemp = null;

        for (int i = 0, len = clickObjectList.Count; i < len; ++i) {
            ClickObject obj = clickObjectList[i];
            if (obj.isMaskingOnly) continue;
            if (!obj.IsActive()) continue;
            float orderLevel = obj.GetOrderLevel();

            if (orderLevelList.Count == 0) {
                orderLevelList.Add(orderLevel);
                orderList.Add(obj);
            } else {
                int index = 0;
                for (int len1 = orderLevelList.Count; index < len1; ++index) {
                    if (orderLevelList[index] < orderLevel) break;
                }
                orderLevelList.Insert(index, orderLevel);
                orderList.Insert(index, obj);
            }
        }

        for (int i = 0, len = orderList.Count; i < len; ++i) {
            ClickObject obj = orderList[i];
            if (obj.MouseOnOverCheck()) {
                onOverTargetTemp = obj;
                break;
            }
        }

        if (onDragOverTarget != onOverTargetTemp && (onOverTarget != onOverTargetTemp || onOverTarget == null)) {
            if (onDragOverTarget) onDragOverTarget.OnDragOutEvent();
            if (onOverTargetTemp) onOverTargetTemp.OnDragOverEvent();
            onDragOverTarget = onOverTargetTemp;
        }
    }

    static void mouseDownFn(_mouseType button) {
        CheckOverFn();
        isDrag = true;
        dragButton = button;
        defaultClick = true;
        if (onOverTarget) {
            onOverTarget.OnDownEvent(button);
            defaultClick = onOverTarget.GetDefaultClick(button);
        }
    }

    bool GetDefaultClick(_mouseType button) {
        if (button == _mouseType.r) {
            return isDefaultClickEventR;
        } else {
            return isDefaultClickEvent;
        }
    }

    static void mouseUpFn(_mouseType button) {
        isDrag = false;
        dragButton = button;
        ClickObject clickTarget = onOverTarget;
        if (onDragOverTarget) {
            onDragOverTarget.OnDragUpEvent(button);
            onDragOverTarget.OnDragOutEvent();
        }
        if (onOverTarget) {
            onOverTarget.OnUpEvent(button);
        }
        onDragOverTarget = null;
        CheckOverFn();
        if (defaultClick && (onOverTarget == null || onOverTarget.GetDefaultClick(button))) {
            GameManager.OnClickEvent(button);
        }
        if (clickTarget != null && clickTarget == onOverTarget) {
            clickTarget.OnClickEvent(button);
        }
    }

    //유니티 자체적인 업데이트가 아닌, 클릭 오브젝트 매니저에서 지정한 타이밍에서 업데이트
    virtual protected void OnUpdateEvent() { 
    }

    virtual protected void OnMouseMoveUpdateEvent() { 
    }

    virtual protected void OnOverEvent() {
    }

    virtual protected void OnOutEvent() {
    }

    virtual protected void OnDownEvent(_mouseType button) {
    }

    virtual protected void OnUpEvent(_mouseType button) {
    }

    virtual protected void OnClickEvent(_mouseType button) {
    }

    virtual protected void OnDragOverEvent() {
    }

    virtual protected void OnDragOutEvent() {
    }

    virtual protected void OnDragUpEvent(_mouseType button) {
    }

    virtual protected void OnDragEvent(_mouseType button) {
    }

    public void SetScroll(UIScroll scrollSc = null) {
        parentScroll = scrollSc;
    }

    public void SetScroll(GameObject scrollObject = null) {
        UIScroll scrollSc = null;
        if (scrollObject != null) {
            scrollSc = scrollObject.GetComponent<UIScroll>();
        }
        parentScroll = scrollSc;
    }

    virtual protected void OnWheelEvent(float v) {
        if (parentScroll != null) parentScroll.OnWheel(v);
    }
}

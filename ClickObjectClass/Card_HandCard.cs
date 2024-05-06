using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Card_HandCard : CardObject {
    GameObject targetingObject = null;
    GameObject nontargetObject = null;
    GameObject nontargetObjectCircle = null;
    GameObject nontargetObjectMax = null;
    GameObject arrowObject = null;
    bool drag = false;

    void Awake() {
        Awake_clickObject();
        Awake_Card_HandCard();
    }

    void Awake_Card_HandCard() {
        targetingObject = this.gameObject.transform.Find("Targeting").gameObject;
        arrowObject = targetingObject.transform.Find("arrow").gameObject;
        nontargetObject = this.gameObject.transform.Find("NontargetUse").gameObject;
        nontargetObjectCircle = nontargetObject.transform.Find("circle").gameObject;
        nontargetObjectMax = nontargetObject.transform.Find("circle2").gameObject;
    }

    int orderCount = 0;
    override protected void OnOverEvent() {
        if (TurnManager.GetPhase() == _phaseType.heroActive) {
            RectTransform rt = this.gameObject.GetComponent<RectTransform>();
            Vector3 v3 = rt.localPosition;
            v3.y = 100;
            rt.localPosition = v3;
            orderCount = 0;
            foreach (Transform tr in this.gameObject.transform.parent) {
                if (tr == this.transform) {
                    break;
                }
                ++orderCount;
            }
            this.transform.SetAsLastSibling();
        }

        UICardViewer.SetCardViewer(linkCardData);
    }
    override protected void OnOutEvent() {
        if (TurnManager.GetPhase() == _phaseType.heroActive) {
            RectTransform rt = this.gameObject.GetComponent<RectTransform>();
            Vector3 v3 = rt.localPosition;
            v3.y = 0;
            rt.localPosition = v3;

            this.transform.SetSiblingIndex(orderCount);
        }
    }

    override protected void OnMouseMoveUpdateEvent() {
        if(drag) DragUpdate();
    }

    override protected void OnDownEvent(_mouseType button) {
        if (TurnManager.GetPhase() != _phaseType.heroActive) return;

        switch (button) {
            case _mouseType.l:
                drag = true;
                switch (linkCardData.GetTargetType()) {
                    case _targetType.self:
                        nontargetObject.SetActive(true);
                        break;
                    default:
                        targetingObject.SetActive(true);
                        TurnManager.TargetFiltering(linkCardData);
                        break;
                }
                DragUpdate();
                break;
        }
    }

    ActorData activatedTarget = null;

    override protected void OnUpEvent(_mouseType button) {
        switch (button) {
            case _mouseType.l:
                drag = false;
                nontargetObject.SetActive(false);
                targetingObject.SetActive(false);
                TurnManager.TargetFiltering();

                if (activatedTarget != null) {
                    linkCardData.UseCard(activatedTarget);
                }
                break;
        }
    }

    void DragUpdate() {
        RectTransform rt;
        Vector3 v3;
        activatedTarget = null;

        switch (linkCardData.GetTargetType()) {
            case _targetType.self:
                rt = nontargetObjectCircle.GetComponent<RectTransform>();
                v3 = Input.mousePosition - rt.position;
                float scale = v3.magnitude / 200;

                Image circle = nontargetObjectCircle.GetComponent<Image>();
                Image circle2 = nontargetObjectMax.GetComponent<Image>();

                if (scale >= 1) {
                    scale = 1;
                    activatedTarget = linkCardData.GetOwner();
                    circle.color = new Color(1, 1, 1, 0.7f);
                    circle2.color = new Color(1, 1, 1, 0.7f);
                } else {
                    circle.color = new Color(0.3f, 0.3f, 0.3f, 0.7f);
                    circle2.color = new Color(0.3f, 0.3f, 0.3f, 0.7f);
                }
                rt.localScale = new Vector3(scale, scale, 1);
                break;
            default:
                v3 = Input.mousePosition;
                ClickObject dragOver = ClickObject.GetDragOverTarget();

                Image arrow = arrowObject.GetComponent<Image>();

                bool targeting = false;

                if (dragOver != null) {
                    ActorClickObject clickObjectTemp = dragOver.gameObject.GetComponent<ActorClickObject>();
                    if (clickObjectTemp != null && clickObjectTemp.thisActorData != null) {
                        if (linkCardData.IsTarget(clickObjectTemp.thisActorData)) {
                            v3 = GameManager.mainCamera.WorldToScreenPoint(clickObjectTemp.thisActorData.GetHeadPos());
                            v3.z = 0;
                            activatedTarget = clickObjectTemp.thisActorData;
                            targeting = true;
                        }
                    }
                }

                if (targeting) {
                    arrow.color = new Color(1, 1, 1, 0.7f);
                } else {
                    arrow.color = new Color(0.3f, 0.3f, 0.3f, 0.7f);
                }

                rt = targetingObject.GetComponent<RectTransform>();
                v3 = v3 - rt.position;
                rt.localScale = new Vector3(1, v3.magnitude / 100, 1);
                rt.rotation = Quaternion.Euler(new Vector3(0, 0, v3.y > 0 ? -math.atan(v3.x / v3.y) / math.PI * 180 : -math.atan(v3.x / v3.y) / math.PI * 180 + 180));
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCard_MyDeck : LobbyCardObject {
    public int slotNum = 0;
    bool isLvLock = false;
    bool isFirst = false;

    public void IsLockCard() {
        this.transform.Find("SPCard").gameObject.SetActive(true);
        isFirst = true;
    }

    public void IsLvLockCard(bool b) {
        this.transform.Find("Lock").gameObject.SetActive(b);
        this.transform.Find("LvText").gameObject.SetActive(b);
        isLvLock = b;
    }

    protected override void OnDownEvent(_mouseType button) {
        if (button == _mouseType.l) {
            if (isFirst || isLvLock) return;
            LobbySceneObject.OnDownMyDeckCard(this);
        }
    }

    protected override void OnUpEvent(_mouseType button) {
        if (button == _mouseType.l) {
            if (isFirst || isLvLock) return;
            LobbySceneObject.OnUpMyDeckCard(this);
        }
    }

    protected override void OnDragUpEvent(_mouseType button) {
        if (button == _mouseType.l) {
            if (isFirst || isLvLock) return;
            LobbySceneObject.OnDragUpMyDeckCard(this);
        }
    }

    protected override void OnOverEvent() {
        LobbySceneObject.SetCardViewer(linkCardData);
        if (isFirst) FirstCardTag(true);
        if (isLvLock) LVLockCardTag(true);
    }
    protected override void OnOutEvent() {
        if (isFirst) FirstCardTag(false);
        if (isLvLock) LVLockCardTag(false);        
    }
    protected override void OnDragOverEvent() {
        if (isFirst) FirstCardTag(true);
        if (isLvLock) LVLockCardTag(true);
    }
    protected override void OnDragOutEvent() {
        if (isFirst) FirstCardTag(false);
        if (isLvLock) LVLockCardTag(false);
    }

    void FirstCardTag(bool b) {
        if (b) {
            UITag.SetTagText(ScriptData.GetScript(300039));
        } else {
            UITag.SetTag();
        }
    }

    void LVLockCardTag(bool b) {
        if (b) {
            UITag.SetTagText(ScriptData.GetScript(300040));
        } else {
            UITag.SetTag();
        }
    }
    protected override void OnClickEvent(_mouseType button) {
        if (button == _mouseType.r) {
            if (isFirst || isLvLock) return;
            LobbySceneObject.OnRClickDeckCard(this);
        }
    }

    public bool GetLock() {
        return isFirst || isLvLock;
    }
}

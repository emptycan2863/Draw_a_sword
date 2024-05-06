using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class UIActionList : MonoBehaviour
{
    static GameObject thisObject = null;

    public GameObject pActionFramePf = null;
    static GameObject actionFramePf = null;

    public GameObject pActionPf = null;
    static GameObject actionPf = null;
    static List<GameObject> actionObjectList = new List<GameObject>();
    static Dictionary<int, GameObject> actionObjectDic = new Dictionary<int, GameObject>();

    public Sprite pActionTargetM = null;
    static Sprite actionTargetM = null;

    public GameObject pScrollObject = null;
    static UIScroll uiScroll = null;
    public GameObject pScrollContents = null;
    static GameObject scrollContents = null;
    public GameObject pMask = null;
    static GameObject mask = null;

    private void Awake() {
        thisObject = this.gameObject;

        actionFramePf = pActionFramePf;
        actionPf = pActionPf;

        actionTargetM = pActionTargetM;
        mask = pMask;

        uiScroll = pScrollObject.GetComponent<UIScroll>();
        scrollContents = pScrollContents;
    }

    static GameObject lastActionFrame = null;
    static float actionCardXpos = 0;

    //이후 제대로 사용자 액터의 정보를 받아서 올린다.
    static public void AddAction(ActorData user, int _id) {
        GameObject frame = Instantiate(actionFramePf, scrollContents.transform);
        frame.transform.localPosition = new Vector3(0, actionObjectList.Count * -80 - 40, 0);
        Transform iconParent = frame.transform.Find("User");
        GameObject iconObject = UIManager.IconInstantiate(user, iconParent);
        ClickObject iconClick = iconObject.GetComponent<ClickObject>();
        iconClick.SetMaskingObject(mask);
        iconClick.SetScroll(uiScroll);

        actionObjectList.Add(frame);
        lastActionFrame = frame;
        actionCardXpos = -475.0f;

        actionObjectDic.Add(_id, frame);

        uiScroll.SetContentsSize(actionObjectList.Count * 80);
        uiScroll.SetScrollPos(1);
    }

    //이후 제대로 카드 정보와 타겟 정보, 액션의 아이디를 받아서 올린다.
    static public void AddActionCard(CardData card, ActorData Target, int parentId, int _id) {
        if (actionObjectDic.ContainsKey(parentId)) {
            GameObject instantObj = Instantiate(actionPf, actionObjectDic[parentId].transform);
            Transform cardObjectTr = instantObj.transform.Find("CardObject");
            cardObjectTr.gameObject.GetComponent<ClickObject>().SetScroll(uiScroll);
            CardObject cardObjectSc = cardObjectTr.gameObject.GetComponent<CardObject>();
            cardObjectSc.linkCardData = card;
            cardObjectSc.SetMaskingObject(mask);
            card.InitCardGraphic(cardObjectTr.Find("Card"));

            float posTemp = 0;
            if (Target != null) {
                GameObject iconObject = UIManager.IconInstantiate(Target, instantObj.transform.Find("Target"));
                ClickObject iconClick = iconObject.GetComponent<ClickObject>();
                iconClick.SetMaskingObject(mask);
                iconClick.SetScroll(uiScroll);
                posTemp = 180.0f;
            } else {
                posTemp = 60.0f;
                instantObj.transform.Find("ActionType").Find("typeImage").gameObject.GetComponent<Image>().sprite = actionTargetM;
            }

            instantObj.transform.localPosition = new Vector3(actionCardXpos, 0, 0);

            actionCardXpos += posTemp;

            actionObjectDic.Add(_id, instantObj);
        }
    }

    static public void ReleaseAction(int _id) {
        if (actionObjectDic.ContainsKey(_id)) {
            int indexof = actionObjectList.IndexOf(actionObjectDic[_id]);
            if (indexof != -1) {
                actionObjectList.RemoveAt(indexof);

                float y = -40;

                for (int i = 0, len = actionObjectList.Count; i < len; ++i) {
                    GameObject obj = actionObjectList[i];
                    obj.transform.localPosition = new Vector3(0, y, 0);
                    y -= 80;
                }
                uiScroll.SetContentsSize(actionObjectList.Count * 80);
                uiScroll.SetScrollPos(1);
            }

            foreach (GameObject actionOvject in actionObjectList) {
                if (actionOvject == actionObjectDic[_id]) {

                    break;
                }
            }
            GameObject.Destroy(actionObjectDic[_id]);
            actionObjectDic.Remove(_id);
        }
    }

    //이제 여기에 딜레이랑 아이디 체크하면서 하나하나 지워주면 됨.
    static public void Clear() {
        for (int i = actionObjectList.Count - 1; i >= 0; --i) {
            GameObject.Destroy(actionObjectList[i]);
        }
        actionObjectList.Clear();
        actionObjectDic.Clear();
        lastActionFrame = null;
        actionCardXpos = 0;
    }
}

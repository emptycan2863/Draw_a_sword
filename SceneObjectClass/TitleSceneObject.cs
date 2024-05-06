using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneObject : SceneObject {
    float startTime = 0;
    bool ready = false;
    public GameObject startText = null;
    CanvasGroup cg = null;

    public GameObject titleWhite = null;
    Image titleWhiteImage = null;

    public GameObject gameSelectObject = null;
    public GameObject gameSelectBack = null;
    public GameObject gameSelectWhite = null;
    Image gameSelectWhiteImage = null;

    public GameObject gameSelectSlotObject = null;
    public GameObject selectGameSlotPrefab = null;

    Vector2[] slotPos = new Vector2[10];

    class TitleStartEvent : TimeEvent {
        public TitleSceneObject sceneObj = null;
        public override void Update(float startTime, float endTime, float progressTimeSec, float progressTimeRatio) {
            sceneObj.cg.alpha = progressTimeRatio;
        }

        public override void End(float progressTimeSec) {
            sceneObj.cg.alpha = 1;
            sceneObj.ready = true;
        }
    }

    void Start() {
        SceneObjectStart();
        GameManager.gameMode = _gamemode.title;
        startTime = GameManager.GetTime();
        cg = startText.GetComponent<CanvasGroup>();
        titleWhiteImage = titleWhite.GetComponent<Image>();
        titleWhiteImage.color = new Color(1, 1, 1, 0);

        gameSelectWhiteImage = gameSelectWhite.GetComponent<Image>();

        SlotPosInit();
        GoMainTitle();
    }

    void GoMainTitle() {
        startText.SetActive(true);
        gameSelectObject.SetActive(false);
        cg.alpha = 0;

        TitleStartEvent startEvent = new TitleStartEvent();
        startEvent.sceneObj = this;
        startEvent.SetTimeEvent(0, 1);
    }

    void SlotPosInit() {
        slotPos[0] = new Vector2(-720, 260);
        slotPos[1] = new Vector2(-360, 260);
        slotPos[2] = new Vector2(0, 260);
        slotPos[3] = new Vector2(360, 260);
        slotPos[4] = new Vector2(720, 260);
        slotPos[5] = new Vector2(-720, -260);
        slotPos[6] = new Vector2(-360, -260);
        slotPos[7] = new Vector2(0, -260);
        slotPos[8] = new Vector2(360, -260);
        slotPos[9] = new Vector2(720, -260);
    }

    protected override void VirtualSceneUpdate() {
    }

    TimeEvent padeEvent = null;
    TimeEvent gameSelectWhiteEvent = null;

    class WhitePadeOut : TimeEvent {
        public Image whiteImage = null;

        public override void Update(float startTime, float endTime, float progressTimeSec, float progressTimeRatio) {
            whiteImage.color = new Color(1, 1, 1, progressTimeRatio);
        }

        public override void End(float progressTimeSec) {
            whiteImage.color = new Color(1, 1, 1, 1);
        }
    }

    class WhitePadeIn : TimeEvent {
        public Image whiteImage = null;

        public override void Update(float startTime, float endTime, float progressTimeSec, float progressTimeRatio) {
            whiteImage.color = new Color(1, 1, 1, 1 - progressTimeRatio);
        }

        public override void End(float progressTimeSec) {
            whiteImage.color = new Color(1, 1, 1, 0);
        }
    }

    public void OnStartButton() {
        if (!ready) return;
        GoSelectGame();
    }

    GameObject[] slotList = new GameObject[10];
    bool slotClickCheck = false;

    public bool GetSlotClickCheck() { return slotClickCheck; }

    class SlotMove1Event : TimeEvent {
        public TitleSceneObject _this = null;

        public override void Update(float startTime, float endTime, float progressTimeSec, float progressTimeRatio) {
            for (int i = 0, len = 10; i < len; ++i) {
                GameObject obj = _this.slotList[i];
                Vector2 pos = new Vector2();
                float ratio = CustomMath.GetEase(_easingType.easeOutQuint, progressTimeRatio);
                pos = _this.slotPos[i] * ratio;
                obj.GetComponent<RectTransform>().localPosition = pos;
            }
        }

        public override void End(float progressTimeSec) {
            _this.slotClickCheck = true;
            ClickObject.CheckOver();
        }
    }

    void GoSelectGame() {
        slotClickCheck = false;
        startText.SetActive(false);

        if (padeEvent != null && !padeEvent.IsDestroy()) padeEvent.Destroy(true);
        WhitePadeOut newPadeEvent = new WhitePadeOut();
        newPadeEvent.whiteImage = titleWhiteImage;
        newPadeEvent.SetTimeEvent(0, 0.3f);
        padeEvent = newPadeEvent;

        gameSelectObject.SetActive(true);

        if (gameSelectWhiteEvent != null && !gameSelectWhiteEvent.IsDestroy()) gameSelectWhiteEvent.Destroy(true);
        WhitePadeIn newPadeEvent2 = new WhitePadeIn();
        newPadeEvent2.whiteImage = gameSelectWhiteImage;
        newPadeEvent2.SetTimeEvent(0, 1.5f);
        gameSelectWhiteEvent = newPadeEvent2;

        for (int i = 0, len = 9; i < len; ++i) {
            GameObject slot = slotList[i] = GameObject.Instantiate(selectGameSlotPrefab, gameSelectSlotObject.transform);
            SelectGameButton buttonClass = slot.GetComponent<SelectGameButton>();
            buttonClass.SetNumber(i);
            buttonClass.titleSceneObject = this.gameObject;
        }
        slotList[9] = gameSelectBack;
        gameSelectBack.GetComponent<RectTransform>().localPosition = new Vector2(0,0);

        SlotMove1Event slotMoveEvent = new SlotMove1Event();
        slotMoveEvent._this = this;
        slotMoveEvent.SetTimeEvent(0.6f, 0.7f);
    }

    class CloseGameSelectEvent : TimeEvent {
        public TitleSceneObject _this = null;
        public override void Update(float startTime, float endTime, float progressTimeSec, float progressTimeRatio) {
            _this.gameSelectObject.GetComponent<CanvasGroup>().alpha = 1 - progressTimeRatio;
        }

        public override void End(float progressTimeSec) {
            foreach (Transform tf in _this.gameSelectSlotObject.transform) {
                GameObject.Destroy(tf.gameObject);
            }

            _this.gameSelectObject.GetComponent<CanvasGroup>().alpha = 1;
            _this.GoMainTitle();
        }
    }

    public void OnClickSelectGame(int code) {
        if (code == -1) {
            if (padeEvent != null && !padeEvent.IsDestroy()) padeEvent.Destroy(true);
            WhitePadeIn newPadeEvent = new WhitePadeIn();
            newPadeEvent.whiteImage = titleWhiteImage;
            newPadeEvent.SetTimeEvent(0, 0.3f);
            padeEvent = newPadeEvent;

            CloseGameSelectEvent closeEvent = new CloseGameSelectEvent();
            closeEvent._this = this;
            closeEvent.SetTimeEvent(0, 0.3f);
        } else {
            PlayerDataManager.LoadData(code);
            GameManager.SceneMove("Scene2Lobby");
        }
    }
}

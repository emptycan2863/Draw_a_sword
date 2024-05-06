/**
 * GameManager 는 게임의 동작을 관리하는 매니저 스크립트이댜.
 * 게임 중 로드 맵에서 단 한번만 생성이 되며 파괴되지 않는다.
 */

using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public enum _gamemode { title, dungeon, battle, lobby };

public class GameManager : MonoBehaviour
{
    static GameManager gm = null;
    static int flame = 0;
    static int deltaFlame = 0;

    public GameObject pMainCamera = null;
    static public Camera mainCamera = null;

    static public bool DEBUG = true;
    static public bool dispDot = true;

    static public _gamemode gameMode = _gamemode.title;

    void Awake()
    {
        if (gm) {
            Destroy(this);
            return;
        }

        Screen.SetResolution(1440, 810, false);

        DontDestroyOnLoad(this);
        gm = this;

        mainCamera = pMainCamera.GetComponent<Camera>();
        DontDestroyOnLoad(pMainCamera);
    }

    void Start() {
        GoTitle();
    }

    public void GoTitle() {
        gameMode = _gamemode.title;
        SceneManager.LoadScene("Scene1Title");
    }

    public static GameManager GetGM() {
        return gm;
    }

    void UpdateFrame(int flame) {
        ClickObject.UpdateClickObjectManager();
        TimeEvent.TimeEventUpdate(flame);

        UIManager.UpdateFrame();
        SoundManager.SoundUpdate();

        TurnManager.TurnUpdate();
        UseCardEffectScript.UpdateFrameStatic();

        SceneObject.SceneUpdate();
        UIReward.UIUpdate();
    }

    //게임 프레임은 일단 60으로 고정하고 생각하자. 과연산은 싫다.
    void Update()
    {
        int time = (int)(Time.time * 60);
        if (flame < time) {
            deltaFlame = time - flame;
            flame = time;
            UpdateFrame(flame);
        }

        float wheelAxis = Input.GetAxis("Mouse ScrollWheel");
        if (wheelAxis != 0) ClickObject.GetWheelAxis(wheelAxis);
    }
    public static float GetDeltaTime() {
        return (float)deltaFlame / 60;
    }
    public static int GetDeltaFlame() {
        return deltaFlame;
    }
    public static float GetTime() {
        return (float)flame / 60;
    }

    public static int GetFlame() {
        return flame;
    }

    public static void OnClickEvent(_mouseType button) {
        switch (gameMode) {
            case _gamemode.battle:
                switch (button) {
                    case _mouseType.l:
                        UICardViewer.SetCardViewer();
                        UIActorInfo.SetActorData();
                        break;
                    case _mouseType.r:
                        if (TurnManager.GetPhase() != _phaseType.heroActive) return;
                        ActorData owner = TurnManager.GetTurnOwner();
                        ActionManager.ResetUseCard(owner);
                        UIHandCard.ChangedActorHand(owner);
                        break;
                }
                break;
        }
    }

    public static GameObject GetActorPosObj() {
        GameObject posObj = GameObject.Find("ActorPositionObj");
        if (posObj == null) {
            posObj = Instantiate(DataManager.actorPositionObjPf);
        }
        return posObj;
    }

    class SceneMoveFn : CallBackClass {
        public string sceneName = null;
        public CallBackClass cbc = null;
        override public void Func() {
            if (cbc != null) cbc.Func();
            SceneManager.LoadScene(sceneName);
        }
    }

    static public void SceneMove(string name, CallBackClass _cbc = null) {
        SceneMoveFn fn = new SceneMoveFn();
        fn.sceneName = name;
        fn.cbc = _cbc;
        UIManager.FadeInOut(fn);
    }

    class BattleEndCbc : CallBackClass {
        public override void Func() {
            TurnManager.EndBattle();
            UIManager.Clear();
            UIReward.CloseUI();
            UIManager.SetBattleUIActive(false);
        }
    }

    static public void BattleEnd() {
        PlayerDataManager.SaveData();
        GameManager.SceneMove("Scene2Lobby", new BattleEndCbc());
    }
}

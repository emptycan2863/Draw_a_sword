/**
 * SceneObject 는 씬을 로드했을 때 필요한 처리를 담당하는 오브젝트이다.
 * 씬을 불러올 때 즉시 실행되며, 현재 열려있는 씬이 어떤 타입이냐에 따라 다른 동작을 할 수 있도록 관리해 준다.
 * 디버그 모드에서는 게임 매니저 없이 테스트가 열렸을 때, 로드맵을 왕복해 오도록 하는 기능이 있다.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneObject : MonoBehaviour
{
    static SceneObject thisScene = null;

    public string BGM = "";
    public float volume = 0.6f;

    // Start is called before the first frame update
    void Awake()
    {
        if (GameManager.GetGM() == null) {
            SceneManager.LoadScene("Scene0Load");
            return;
        }
        thisScene = this;
    }

    private void Start() {
        SceneObjectStart();
    }

    protected void SceneObjectStart() {
        if (BGM != "") SoundManager.PlayBGM(BGM, volume);
    }

    //곧 수정할 함수.
    static public void SceneUpdate() {
        if (thisScene != null) thisScene.VirtualSceneUpdate();
    }
    
    protected virtual void VirtualSceneUpdate() { 
    }
}

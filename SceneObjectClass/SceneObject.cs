/**
 * SceneObject �� ���� �ε����� �� �ʿ��� ó���� ����ϴ� ������Ʈ�̴�.
 * ���� �ҷ��� �� ��� ����Ǹ�, ���� �����ִ� ���� � Ÿ���̳Ŀ� ���� �ٸ� ������ �� �� �ֵ��� ������ �ش�.
 * ����� ��忡���� ���� �Ŵ��� ���� �׽�Ʈ�� ������ ��, �ε���� �պ��� ������ �ϴ� ����� �ִ�.
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

    //�� ������ �Լ�.
    static public void SceneUpdate() {
        if (thisScene != null) thisScene.VirtualSceneUpdate();
    }
    
    protected virtual void VirtualSceneUpdate() { 
    }
}

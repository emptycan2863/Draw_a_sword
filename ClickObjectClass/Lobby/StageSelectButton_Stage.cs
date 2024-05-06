using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectButton_Stage : ClickObject {
    public int stageNum = 0;

    protected override void OnClickEvent(_mouseType button) {
        if (button == _mouseType.l) {
            TurnManager.SetStage(stageNum);
            GameManager.SceneMove(StageData.GetSceneName(stageNum));
        }
    }
}

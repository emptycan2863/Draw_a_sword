using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINextArrow : MonoBehaviour {
    static GameObject buttonObject = null;

    private void Awake() {
        buttonObject = this.gameObject;
        buttonObject.SetActive(false);
    }

    static public void OnClick() {
        if (TurnManager.GetPhase() == _phaseType.heroActive) TurnManager.TurnEnd();
    }

    static public void SetUIActive(bool set) {
        buttonObject.SetActive(set);
    }
}

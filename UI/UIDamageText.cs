using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum _damageTextType {
    damage, heal, guard, shild, eva, mpCost, mpRecov, accel
}

public class UIDamageText : MonoBehaviour
{
    public GameObject prefab = null;
    public Color dColor = new Color(1f, 1f, 1f, 1f);
    public Color dOutline = new Color(1f, 1f, 1f, 1f);

    public Color gColor = new Color(1f, 1f, 1f, 1f);
    public Color gOutline = new Color(1f, 1f, 1f, 1f);

    public Color sColor = new Color(1f, 1f, 1f, 1f);
    public Color sOutline = new Color(1f, 1f, 1f, 1f);

    public Color eColor = new Color(1f, 1f, 1f, 1f);
    public Color eOutline = new Color(1f, 1f, 1f, 1f);

    public Color hColor = new Color(1f, 1f, 1f, 1f);
    public Color hOutline = new Color(1f, 1f, 1f, 1f);

    public Color mcColor = new Color(1f, 1f, 1f, 1f);
    public Color mcOutline = new Color(1f, 1f, 1f, 1f);

    public Color mrColor = new Color(1f, 1f, 1f, 1f);
    public Color mrOutline = new Color(1f, 1f, 1f, 1f);

    public Color accelColor = new Color(1f, 1f, 1f, 1f);
    public Color accelOutline = new Color(1f, 1f, 1f, 1f);

    static UIDamageText thisUi = null;

    static public GameObject sPrefab = null;
    static public Color sDColor = new Color(1f, 1f, 1f, 1f);
    static public Color sDOutline = new Color(1f, 1f, 1f, 1f);

    static public Color sGColor = new Color(1f, 1f, 1f, 1f);
    static public Color sGOutline = new Color(1f, 1f, 1f, 1f);

    static public Color sSColor = new Color(1f, 1f, 1f, 1f);
    static public Color sSOutline = new Color(1f, 1f, 1f, 1f);

    static public Color sEColor = new Color(1f, 1f, 1f, 1f);
    static public Color sEOutline = new Color(1f, 1f, 1f, 1f);

    static public Color sHColor = new Color(1f, 1f, 1f, 1f);
    static public Color sHOutline = new Color(1f, 1f, 1f, 1f);

    static public Color sMcColor = new Color(1f, 1f, 1f, 1f);
    static public Color sMcOutline = new Color(1f, 1f, 1f, 1f);

    static public Color sMrColor = new Color(1f, 1f, 1f, 1f);
    static public Color sMrOutline = new Color(1f, 1f, 1f, 1f);

    static public Color sAccelColor = new Color(1f, 1f, 1f, 1f);
    static public Color sAccelOutline = new Color(1f, 1f, 1f, 1f);

    void Awake() {
        thisUi = this;
        sPrefab = prefab;

        sDColor = dColor;
        sDOutline = dOutline;
        sGColor = gColor;
        sGOutline = gOutline;
        sSColor = sColor;
        sSOutline = sOutline;
        sEColor = eColor;
        sEOutline = eOutline;
        sHColor = hColor;
        sHOutline = hOutline;
        sMcColor = mcColor;
        sMcOutline = mcOutline;
        sMrColor = mrColor;
        sMrOutline = mrOutline;
        sAccelColor = accelColor;
        sAccelOutline = accelOutline;
    }

    static public GameObject AddDamageText(_damageTextType type, int pow, Vector3 pos) {
        GameObject go = GameObject.Instantiate(sPrefab, thisUi.transform);
        go.transform.position = pos;

        string text = "";
        Color c1 = new Color();
        Color c2 = new Color();
        switch (type) {
            case _damageTextType.damage:
                text = ScriptData.GetScript(300009);
                c1 = sDColor;
                c2 = sDOutline;
                break;
            case _damageTextType.heal:
                text = ScriptData.GetScript(300010);
                c1 = sHColor;
                c2 = sHOutline;
                break;
            case _damageTextType.mpCost:
                text = ScriptData.GetScript(300011);
                c1 = sMcColor;
                c2 = sMcOutline;
                break;
            case _damageTextType.mpRecov:
                text = ScriptData.GetScript(300012);
                c1 = sMrColor;
                c2 = sMrOutline;
                break;
            case _damageTextType.guard:
                text = ScriptData.GetScript(300013);
                c1 = sGColor;
                c2 = sGOutline;
                break;
            case _damageTextType.shild:
                text = ScriptData.GetScript(300014);
                c1 = sSColor;
                c2 = sSOutline;
                break;
            case _damageTextType.eva:
                text = ScriptData.GetScript(300015);
                c1 = sEColor;
                c2 = sEOutline;
                break;
            case _damageTextType.accel:
                if (pow > 0) {
                    text = ScriptData.GetScript(300043);
                } else {
                    text = ScriptData.GetScript(300044);
                    pow = -pow;
                }
                c1 = sAccelColor;
                c2 = sAccelOutline;
                break;
        }
        text = text.Replace("{0}", "" + pow + "\n 123");

        Text tCom = go.GetComponent<Text>();
        tCom.text = text;

        tCom.color = c1;
        go.GetComponent<Outline>().effectColor = c2;
        return go;
    }
}

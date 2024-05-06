using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

static public class SpriteData
{
    static Dictionary<string, Sprite> cardSpriteData = new Dictionary<string, Sprite>();

    static public Sprite GetSprite(string path) {
        if (cardSpriteData.ContainsKey(path)) {
            return cardSpriteData[path];
        } else {
            Sprite sp = Resources.Load<Sprite>("Image/" + path);
            if (sp) {
                cardSpriteData.Add(path, sp);
                return sp;
            } else {
                return null;
            }
        }
    }

    static public Sprite GetCardSprite(int num) {
        string path = "CardImage/card_" + num;
        if (cardSpriteData.ContainsKey(path)) {
            return cardSpriteData[path];
        } else {
            Sprite sp = Resources.Load<Sprite>("Image/" + path);
            if (sp) {
                cardSpriteData.Add(path, sp);
                return sp; 
            } else {
                return null;
            }
        }
    }

    static public Sprite GetActorIconSprite(string codeName) {
        string name = "";
        switch (codeName) {
            case "C_Rank":
            case "B_Rank":
            case "A_Rank":
            case "S_Rank":
                name = codeName;
                break;
            default:
                name = HeroActorBaseData.GetActorIconName(codeName);
                break;
        }
        if (name == "none") return null;
        string path = "ActorIcon/" + name;
        if (cardSpriteData.ContainsKey(path)) {
            return cardSpriteData[path];
        } else {
            Sprite sp = Resources.Load<Sprite>("Image/" + path);
            if (sp) {
                cardSpriteData.Add(path, sp);
                return sp;
            } else {
                return null;
            }
        }
    }    
}

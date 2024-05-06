using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardClass;

public class CreateCardDataCallBack {
    virtual public CardData Create() {
        return new CardData();
    }
}

static public class CardDataInitManager {
    static Dictionary<int, CreateCardDataCallBack> CardDataList = new Dictionary<int, CreateCardDataCallBack>();

    static public CardData CreateCardData(int index) {
        if (CardDataList.ContainsKey(index)) {
            return CardDataList[index].Create();
        } else {
            return new CardData();
        }
    }
    static public void Init() {
        CardDataList.Add(1, new CreastCardClass_1());
        CardDataList.Add(2, new CreastCardClass_2());
        CardDataList.Add(3, new CreastCardClass_3());
        CardDataList.Add(4, new CreastCardClass_4());
        CardDataList.Add(5, new CreastCardClass_5());
        CardDataList.Add(6, new CreastCardClass_6());
        CardDataList.Add(7, new CreastCardClass_7());
        CardDataList.Add(101, new CreastCardClass_101());
        CardDataList.Add(102, new CreastCardClass_102());
        CardDataList.Add(103, new CreastCardClass_103());
        CardDataList.Add(104, new CreastCardClass_104());
        CardDataList.Add(105, new CreastCardClass_105());
        CardDataList.Add(201, new CreastCardClass_201());
        CardDataList.Add(202, new CreastCardClass_202());
        CardDataList.Add(203, new CreastCardClass_203());
        CardDataList.Add(204, new CreastCardClass_204());
        CardDataList.Add(205, new CreastCardClass_205());
        CardDataList.Add(1001, new CreastCardClass_1001());
        CardDataList.Add(1002, new CreastCardClass_1002());
        CardDataList.Add(1003, new CreastCardClass_1003());
        CardDataList.Add(1004, new CreastCardClass_1004());
    }
}

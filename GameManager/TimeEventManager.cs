using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeEvent {
    static List<TimeEvent> eventReadyList = new List<TimeEvent>();
    static List<TimeEvent> eventList = new List<TimeEvent>();

    bool destroyed = false;

    static public void TimeEventUpdate(int flame) {
        float thisTime = GameManager.GetTime();
        for (int i = eventReadyList.Count - 1; i >= 0; --i) {
            if (eventReadyList[i].startTime <= thisTime) {
                eventReadyList[i].Start();
                eventList.Insert(0, eventReadyList[i]);
                eventReadyList.RemoveAt(i);
            }
        }

        for (int i = eventList.Count - 1; i >= 0; --i) {
            TimeEvent timeEvent = eventList[i];
            timeEvent.Update(timeEvent.startTime, timeEvent.endTime, thisTime - timeEvent.startTime, (thisTime - timeEvent.startTime) / (timeEvent.endTime - timeEvent.startTime));
            if (timeEvent.endTime <= thisTime) {
                timeEvent.End(timeEvent.endTime - timeEvent.startTime);
                timeEvent.destroyed = true;
                eventList.RemoveAt(i);
            }
        }
    }

    //-----

    float startTime = 0;
    float endTime = 0;

    public void SetTimeEvent(float delay, float duration) {
        startTime = GameManager.GetTime() + delay;
        endTime = startTime + duration;
        eventReadyList.Insert(0, this);
    }

    virtual public void Start() {
        return;
    }

    virtual public void Update(float startTime, float endTime, float progressTimeSec, float progressTimeRatio) {
    }

    virtual public void End(float progressTimeSec) {
        return;
    }

    public void SetEndTime(float t) {
        endTime = t;
    }

    public bool IsDestroy() {
        return destroyed;
    }

    public void Destroy(bool EndFunc) {
        if (destroyed) return;
        float thisTime = GameManager.GetTime();
        if (EndFunc) End(thisTime - startTime);
        eventList.Remove(this);
        destroyed = true;
    }
}
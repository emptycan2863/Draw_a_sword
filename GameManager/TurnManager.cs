using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
/**
 * 턴 규칙
 * 1. 가장 첫 턴에는
 */

public enum _phaseType { 
    draw,
    heroActive,
    enemyActive,
    action,
    end
};

public class TurnManager
{
    static List<TeamData> teamList = new List<TeamData>();
    static List<ActorData> actorList = new List<ActorData>();
    static int turnCount = 0;
    static int cycleCount = 0;
    static bool nextTurnReady = false;
    static float delayTime = 0;
    static ActorData thisTurnActor = null;
    static _phaseType thisPhase = _phaseType.end;
    static int stageIndex = 0;

    static public void TurnUpdate() {
        if (GameManager.gameMode != _gamemode.battle) return;

        foreach (ActorData actor in actorList) {
            actor.ActorUpate();
        }

        ActionManager.ActionManagerUpdate();

        if (nextTurnReady && delayTime < GameManager.GetTime()) {
            nextTurnReady = false;
            NextTurn();
        }
    }

    static public void SetStage(int index) {
        stageIndex = index;
    }

    static void SetTeam(TeamData newTeam) {
        teamList.Add(newTeam);
    }

    static public void StartGame() {
        teamList.Clear();

        TeamData pt = new TeamData();
        pt.SetPlayerTeam();
        SetTeam(pt);

        TeamData enemyTeam = TeamData.GetEnemyTeam(stageIndex);
        SetTeam(enemyTeam);

        actorList.Clear();
        foreach (TeamData td in teamList) {
            List<ActorData> actors = td.GetActors();
            foreach (ActorData ad in actors) {
                actorList.Add(ad);
            }
        }

        turnCount = 0;
        cycleCount = 0;
        thisTurnActor = null;

        NextCycle();
    }

    static void NextTurn() {
        ++turnCount;
        ActorData nextActor = GetNextActor();
        if (nextActor == null) {
            ActionPhaseStart();
            return;
        }

        nextActor.turnCheck = true;

        nextActor.TurnStart();
        thisTurnActor = nextActor;
        delayTime = GameManager.GetTime() + 2;
    }

    static public void NextCycle() {
        int teamCount = 0;
        TeamData winners = null;
        foreach (TeamData team in teamList) {
            List<ActorData> actors = team.GetActors();
            foreach (ActorData actor in actors) {
                if (!actor.isDead) {
                    winners = team;
                    ++teamCount;
                    break;
                }
            }
        }

        if (teamCount == 1) {
            winners.WinEvent();
            return;
        }

        ++cycleCount;
        turnCount = 0;
        foreach (ActorData actor in actorList) {
            actor.turnCheck = false;
            actor.CycleReset();
        }

        NextTurn();
    }

    static void ActionPhaseStart() {
        thisPhase = _phaseType.action;

        ActionManager.ActionStart();
    }

    static ActorData GetNextActor() {
        int speed = -1;
        List<ActorData> target = new List<ActorData>();
        foreach (ActorData actor in actorList) {
            if (actor.isDead) continue;
            if (actor.turnCheck) continue;
            int actorSp = actor.GetSpeed();
            if (speed == -1 || speed > actorSp) {
                speed = actorSp;
                target.Clear();
            }
            if (speed == actorSp) {
                target.Add(actor);
            }
        }

        if (target.Count == 0) return null;
        if (target.Count == 1) return target[0];
        return target[Random.Range(0, target.Count)];
    }

    static public List<ActorData> GetAllActors() {
        List<ActorData> newList = new List<ActorData>();
        foreach (ActorData actor in actorList) {
            newList.Add(actor);
        }
        return newList;
    }

    static public void TargetFiltering(CardData card = null) {
        if (card == null) {
            foreach (ActorData actor in actorList) {
                actor.ObjectDarkly(false);
            }
            return;
        }
        ActorData user = card.GetOwner();
        _targetType type = card.GetTargetType();
        foreach (ActorData actor in actorList) {
            bool darkly = true;
            if (card.IsTarget(actor)) {
                darkly = false;
            }

            actor.ObjectDarkly(darkly);
        }
    }

    static public void TurnEnd() {
        SetPhaseEnd();
        thisTurnActor.TurnEnd();
        NextTurn();
    }
 
    static public void SetPhaseDraw() {
        thisPhase = _phaseType.draw;
    }

    static public void SetPhaseHeroActive() {
        thisPhase = _phaseType.heroActive;
    }

    static void SetPhaseEnd() {
        thisPhase = _phaseType.end;
    }

    static public void SetPhaseEnemyActive() {
        thisPhase = _phaseType.enemyActive;
    }

    static public _phaseType GetPhase() {
        return thisPhase;
    }

    static public ActorData GetTurnOwner() {
        return thisTurnActor;
    }

    static public void WinEvent(_teamType type) {
        switch (type) {
            case _teamType.hero:
                PlayerDataManager.StageReward(stageIndex);
                break;
            default:
                GameManager.BattleEnd();
                break;
        }
    }

    static public void EndBattle() {
        teamList.Clear();
        actorList.Clear();
        turnCount = 0;
        cycleCount = 0;
        nextTurnReady = false;
        delayTime = 0;
        thisTurnActor = null;
        thisPhase = _phaseType.end;
        stageIndex = 0;
    }
}

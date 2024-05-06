using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum _teamType { hero, enemy };
public enum _teamPos { Left, Right };

public class TeamData
{
    string teamName = "";
    _teamType teamType = 0;
    List<ActorData> actors = new List<ActorData>();

    void Init() {
        actors.Clear();
    }

    void SetActor(ActorData actorData) {
        actors.Add(actorData);
        actorData.SetTeam(this);
    }

    public List<ActorData> GetActors() {
        return actors;
    }

    public void SetPlayerTeam() {
        teamName = "PlayerParty";

        Init();

        for (int i = 0, len = PlayerDataManager.GetPartyMemberCount(); i < len; ++i) {
            ActorData hero = PlayerDataManager.GetHeroInParty(i);
            SetActor(hero);
            hero.SetBattleState();
        }

        SetTeamType(_teamType.hero, _teamPos.Left);
    }

    static public TeamData GetEnemyTeam(int index) {
        TeamData newTeam = new TeamData();
        newTeam.teamName = "stage_" + index;
        newTeam.actors.Clear();

        int[] enemyList = StageData.GetEnemys(index);
        for (int i = 0, len = enemyList.Length; i < len; ++i) {
            if (enemyList[i] == 0) continue;
            EnemyActorData enemy = EnemyActorData.CreateEnemyData(enemyList[i]);
            newTeam.SetActor(enemy);
            enemy.SetBattleState();
        }

        newTeam.SetTeamType(_teamType.enemy, _teamPos.Right);
        return newTeam;
    }

    void SetTeamType(_teamType type, _teamPos posType) {
        string posName = "";
        teamType = type;

        switch (posType) {
            case _teamPos.Left:
                posName = "Left";
                break;
            case _teamPos.Right:
                posName = "Right";
                break;
        }
        GameObject posObj = GameManager.GetActorPosObj();
        Transform teamTf = posObj.transform.Find(posName);
        for (int i = 0, len = teamTf.childCount; i < len; ++i) {
            Transform posTf = teamTf.GetChild(i);
            for (int j = 0, jLen = posTf.childCount; j < jLen; ++j) {
                    GameObject.Destroy(posTf.GetChild(j).gameObject);
                }

            if (actors.Count > i) {
                actors[i].SetActorObject(posTf, posType);
            }
        }
    }

    public string GetTeamName() {
        return teamName;
    }

    public void WinEvent() {
        TurnManager.WinEvent(teamType);
    }
}

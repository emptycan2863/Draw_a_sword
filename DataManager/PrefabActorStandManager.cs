using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class PrefabActorStandManager {
    static Dictionary<string, PrefabActorStand> actorStandData = new Dictionary<string, PrefabActorStand>();
    static public PrefabActorStand GatActorStand(string name) {
        if (!actorStandData.ContainsKey(name)) {
            GameObject prefab = Resources.Load<GameObject>("Prefab/ActorStand/" + name);
            actorStandData.Add(name, new PrefabActorStand(prefab));
        }

        return actorStandData[name];
    }
}

public class PrefabActorStand {
    GameObject prefabObject = null;

    public PrefabActorStand(GameObject prefab) {
        prefabObject = prefab;
    }

    public GameObject Instantiate(Transform parents = null) {
        return GameObject.Instantiate(prefabObject, parents);
    }
}
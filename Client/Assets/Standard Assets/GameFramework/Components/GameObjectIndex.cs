using System;
using UnityEngine;

[Serializable]
public class GameObjectIndex
{
    public int id;

    public GameObject gameObject;

    public GameObjectIndex(int id, GameObject go)
    {
        this.id = id;
        this.gameObject = go;
    }
}

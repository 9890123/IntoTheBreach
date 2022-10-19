using System;
using UnityEngine;

public class GameObjectContainer : MonoBehaviour
{
    public GameObjectIndex[] objectArray = null;

    public GameObject Get(int id)
    {
        GameObject result;
        if (this.objectArray == null)
        {
            result = null;
        }
        else
        {
            for (int i = 0; i < this.objectArray.Length; i++)
            {
                if (this.objectArray[i] != null && this.objectArray[i].id == id)
                {
                    result = this.objectArray[i].gameObject;
                    return result;
                }
            }
            result = null;
        }
        return result;
    }

    public void Add(int key, GameObject go, bool destroyOld = false)
    {
        for (int i = 0; i < this.objectArray.Length; i++)
        {
            if (this.objectArray[i] != null && this.objectArray[i].id == key)
            {
                if (destroyOld)
                {
                    UnityEngine.Object.DestroyImmediate(this.objectArray[i].gameObject);
                }
                this.objectArray[i].gameObject = go;
                return;
            }
        }
        GameObjectIndex[] tmpArray = new GameObjectIndex[objectArray.Length + 1];
        Array.Copy(objectArray, tmpArray, objectArray.Length);
        objectArray = tmpArray;
        objectArray[objectArray.Length - 1] = new GameObjectIndex(key, go);
    }
}

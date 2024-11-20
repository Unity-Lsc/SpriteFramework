using System.Collections.Generic;
using UnityEngine;
using SpriteFramework;

public class PoolExample : MonoBehaviour
{

    List<GameObject> m_List;
    Vector3 vec;

    void Start()
    {
        m_List = new List<GameObject>();
        vec = new Vector3(0, 0, 0);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A)) {
           GameObject obj = CreateObj("Charactor/HanBing");
            m_List.Add(obj);

            var handler = obj.GetOrAddCompponent<GameObjectDespawnHandle>();
            handler.SetEnqueueDelayTime(3);
            handler.OnComplete = () => {
                GameEntry.Log("物体自动回池");
            };
        }
        if (Input.GetKeyUp(KeyCode.S)) {
            GameObject obj = CreateObj("Charactor/NvQiang");
            m_List.Add(obj);
        }
        if (Input.GetKeyUp(KeyCode.D)) {
            GameObject obj = CreateObj("Charactor/Ruizi");
            m_List.Add(obj);
        }
        if (Input.GetKeyUp(KeyCode.E)) {
            if(m_List.Count > 0) {
                for (int i = 0; i < m_List.Count; i++) {
                    GameEntry.Pool.GameObjectPool.Enqueue(m_List[i]);
                }
            }
        }
    }

    private GameObject CreateObj(string name) {
        var obj = GameEntry.Pool.GameObjectPool.Dequeue(name);
        obj.transform.localPosition += vec;
        vec += Vector3.one;
        return obj;
    }

}

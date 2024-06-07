﻿using System;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.Framework.Pool
{
    public interface IPooled { }
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject managerObject = new() { name = nameof(PoolManager) };
                    instance = managerObject.AddComponent<PoolManager>();
                }
                return instance;
            }
        }
        private static PoolManager instance;
        private readonly Dictionary<string, GameObjectPool> gameObjectPoolDic = new();
        private readonly Dictionary<string, StoreOnlyObjectPool> storeDic = new();
        private void OnDestroy()
        {
            if (instance == this) instance = null;
            Clear();
        }
        #region GameObject
        public GameObject GetGameObject(string assetName, Transform parent = null)
        {
            GameObject obj = null;
            if (gameObjectPoolDic.TryGetValue(assetName, out GameObjectPool poolData) && poolData.poolQueue.Count > 0)
            {
                obj = poolData.GetObj(parent);
            }
            return obj;
        }
        public void PushGameObject(GameObject obj, string overrideName = null)
        {
            string name = overrideName ?? obj.name;
            if (gameObjectPoolDic.TryGetValue(name, out GameObjectPool poolData))
            {
                poolData.PushObj(obj);
            }
            else
            {
                gameObjectPoolDic.Add(name, new GameObjectPool(obj, gameObject));
            }
        }

        #endregion

        #region C# object
        public T GetObject<T>() where T : class, IPooled, new()
        {
            T obj;
            if (CheckObjectCache<T>())
            {
                string name = typeof(T).FullName;
                obj = (T)storeDic[name].Get();
                return obj;
            }
            else
            {
                return new T();
            }
        }
        public T GetScriptableObject<T>(string objectName) where T : ScriptableObject
        {
            T obj;
            if (CheckObjectCache(objectName))
            {
                obj = (T)storeDic[objectName].Get();
                return obj;
            }
            else
            {
                return null;
            }
        }

        public void ReleaseObject(IPooled obj)
        {
            string name = obj.GetType().FullName;
            if (storeDic.ContainsKey(name))
            {
                storeDic[name].Release(obj);
            }
            else
            {
                storeDic.Add(name, new StoreOnlyObjectPool(obj));
            }
        }
        public void ReleaseObject(IPooled obj, string overrideName)
        {
            if (storeDic.ContainsKey(overrideName))
            {
                storeDic[overrideName].Release(obj);
            }
            else
            {
                storeDic.Add(overrideName, new StoreOnlyObjectPool(obj));
            }
        }


        private bool CheckObjectCache<T>()
        {
            string name = typeof(T).FullName;
            return storeDic.ContainsKey(name) && storeDic[name].poolQueue.Count > 0;
        }
        private bool CheckObjectCache(string objectName)
        {
            return storeDic.ContainsKey(objectName) && storeDic[objectName].poolQueue.Count > 0;
        }

        #endregion


        #region Release
        public void Clear(bool clearGameObject = true, bool clearCObject = true)
        {
            if (clearGameObject)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
                gameObjectPoolDic.Clear();
            }

            if (clearCObject)
            {
                storeDic.Clear();
            }
        }

        public void ClearAllGameObject()
        {
            Clear(true, false);
        }
        public void ClearGameObject(string prefabName)
        {
            GameObject go = transform.Find(prefabName).gameObject;
            if (go)
            {
                Destroy(go);
                gameObjectPoolDic.Remove(prefabName);

            }

        }
        public void ClearGameObject(GameObject prefab)
        {
            ClearGameObject(prefab.name);
        }

        public void ClearAllObject()
        {
            Clear(false, true);
        }
        public void ClearObject<T>()
        {
            storeDic.Remove(typeof(T).FullName);
        }
        public void ClearObject(Type type)
        {
            storeDic.Remove(type.FullName);
        }
        #endregion

    }
}
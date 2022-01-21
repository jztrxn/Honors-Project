// (c) Copyright 2020 HP Development Company, L.P.

using System;
using System.Collections.Generic;
using UnityEngine;


namespace HP.Glia.Examples.Common
{
    public static class ObjectPooler {
        public static List<ObjectPoolItem> registeredObjects = new List<ObjectPoolItem>();
        private static Dictionary<string, UnityObjectPooler> unityObjectPools = new Dictionary<string, UnityObjectPooler>();
        private static GameObject root;

        public static UnityObjectPooler Register(ObjectPoolItem item){
            if(root == null){
                root = new GameObject("Pool");
            }

            if(unityObjectPools.ContainsKey(item.id)){
                return unityObjectPools[item.id];
            }
            else{
                GameObject pool = new GameObject(item.id);
                pool.transform.SetParent(root.transform);
                UnityObjectPooler objectPooler = pool.AddComponent<UnityObjectPooler>();
                objectPooler.Initialize(item);
                unityObjectPools.Add(item.id, objectPooler);
            }

            return unityObjectPools[item.id];
        }

        public static GameObject GetObject(string id){
            if(unityObjectPools.ContainsKey(id)){
                return unityObjectPools[id].GetObject();
            }
            else{
                Debug.LogError($"No pool with id {id}");
                return null;
            }
        }

        public static void Return(GameObject gameObject)
        {
            UnityObjectPoolItem poolItem = gameObject.GetComponent<UnityObjectPoolItem>();

            if(poolItem != null && unityObjectPools.ContainsKey(poolItem.GetID())){
                unityObjectPools[poolItem.GetID()].Return(poolItem);
            }
            else{
                Debug.LogError("Could not return object to the pool");
            }
        }
    }
}
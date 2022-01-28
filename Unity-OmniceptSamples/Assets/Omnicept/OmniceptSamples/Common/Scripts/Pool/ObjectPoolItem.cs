// (c) Copyright 2020 HP Development Company, L.P.

using UnityEngine;

namespace HP.Glia.Examples.Common
{
    [System.Serializable]
    public class ObjectPoolItem {
        public int initialPoolSize = 50;
        public GameObject objectToPool;
        public bool shouldExpand = true;
        public int expandPoolSize = 10;
        public string id;
    }
}
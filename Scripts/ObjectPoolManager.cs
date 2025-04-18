using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Pool;
using static ObjectPoolManager;
using Unity.VisualScripting;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private PoolDatabase poolDatabase;
    [SerializeField] private Canvas canvas = default;

    public static Dictionary<PoolType, PoolObjectInfo> ObjectPools = new Dictionary<PoolType, PoolObjectInfo>();
    private static Transform _poolRoot;

    public enum PoolType
    {
        None,
        Bubble,
        Sugar,
        GFood,
        BFood
    }
    public static PoolType poolType;

    private void Awake()
    {
        SetupPools();
        poolDatabase.Initialize();
    }

    public GameObject GetPrefab(PoolType type)
    {
        return poolDatabase.Get(type);
    }

    private void SetupPools()
    {
        if (_poolRoot == null)
        {
            _poolRoot = new GameObject("ObjectPool").transform;
            if (canvas != default)
            {
                _poolRoot.SetParent(canvas.transform);
                _poolRoot.transform.localScale = Vector3.one;
                _poolRoot.transform.localPosition = Vector3.zero;
            }
        }

        foreach (var poolObject in poolDatabase.AllObjects)
        {
            if (ObjectPools.ContainsKey(poolObject.poolType)) continue;

            var parent = new GameObject("ObjectPoolRoot").transform;
            parent.SetParent(_poolRoot);
            if (canvas != default)
            {
                parent.transform.localScale = Vector3.one;
                parent.transform.localPosition = Vector3.zero;
            }

            var info = new PoolObjectInfo
            {
                PoolType = poolObject.poolType,
                Prefab = poolObject.prefab,
                Expandable = true,
                InactivePoolObjects = new List<GameObject>(),
                Parent = parent
            };

            for (int i = 0; i < poolObject.poolSize; i++)
            {
                GameObject obj = Instantiate(poolObject.prefab, parent);
                obj.name = poolObject.prefab.name;
                obj.SetActive(false);
                info.InactivePoolObjects.Add(obj);
            }

            ObjectPools[poolObject.poolType] = info;
        }
    }

    public static GameObject SpawnObject(PoolType type, Vector3 startPosition, Quaternion startRotation)
    {
        if (!ObjectPools.TryGetValue(type, out var poolInfo))
        {
            Debug.LogWarning($"[ObjectPoolManager] PoolType {type} not found");
            return null;
        }

        GameObject spawnableObject = poolInfo.InactivePoolObjects.FirstOrDefault();

        if (spawnableObject == null && poolInfo.Expandable)
        {
            spawnableObject = Instantiate(poolInfo.Prefab, poolInfo.Parent);
            spawnableObject.name = poolInfo.Prefab.name;
        }

        if (spawnableObject == null)
        {
            Debug.LogWarning($"[ObjectPoolManager] No available objects in pool {type}");
            return null;
        }
        spawnableObject.transform.SetPositionAndRotation(startPosition, startRotation);
        spawnableObject.SetActive(true);
        poolInfo.InactivePoolObjects.Remove(spawnableObject);

        if (spawnableObject.TryGetComponent<IPoolable>(out var poolable))
        {
            poolable.OnSpawn();
        }

        return spawnableObject;
    }

    public static void ReturnObjectToPool(GameObject gameObject)
    {
        foreach (var pool in ObjectPools.Values)
        {
            if (gameObject.name != pool.Prefab.name) continue;
            if (gameObject.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnReturnToPool();
            }
            gameObject.SetActive(false);
            gameObject.transform.SetParent(pool.Parent);
            pool.InactivePoolObjects.Add(gameObject);
            return;
        }
        Debug.LogWarning($"[ObjectPoolManager] Tried to return unregistered object: {gameObject.name}");
    }
}

public class PoolObjectInfo
{
    public PoolType PoolType;
    public GameObject Prefab;
    public bool Expandable;
    public Transform Parent;
    public List<GameObject> InactivePoolObjects = new();
}
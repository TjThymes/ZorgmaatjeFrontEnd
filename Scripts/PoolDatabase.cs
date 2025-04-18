using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static ObjectPoolManager;

[CreateAssetMenu(fileName = "PoolDatabase", menuName = "Pooling/PoolDatabase", order = 2)]
public class PoolDatabase : ScriptableObject
{
    [SerializeField] private List<PoolObject> poolObjects;

    private Dictionary<PoolType, GameObject> _poolLookup;

    public IEnumerable<PoolObject> AllObjects => poolObjects;

    public void Initialize()
    {
        _poolLookup = new Dictionary<PoolType, GameObject>();
        foreach (var entry in poolObjects)
        {
            if (!_poolLookup.ContainsKey(entry.poolType))
            {
                _poolLookup[entry.poolType] = entry.prefab;
            }
        }
    }

    public GameObject Get(PoolType type)
    {
        return _poolLookup != null && _poolLookup.TryGetValue(type, out var prefab) ? prefab : null;
    }
}

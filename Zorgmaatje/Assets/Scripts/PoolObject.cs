using UnityEngine;
using static ObjectPoolManager;

[CreateAssetMenu(fileName = "PoolObject", menuName = "Pooling/PoolObject", order = 1)]
public class PoolObject : ScriptableObject
{
    public PoolType poolType;
    public GameObject prefab;
    public int poolSize = 10;
    public bool expandablePool = true;
}

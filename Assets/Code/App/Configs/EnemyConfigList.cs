using UnityEngine;

namespace Code.App.Configs
{
    [CreateAssetMenu(fileName = "EnemyConfigList", menuName = "Configs / Enemy Config List")]
    public class EnemyConfigList : ScriptableObject
    {
        [field:SerializeField] public GameObject AsteroidPrefab;
        [field:SerializeField] public GameObject SmallAsteroidPrefab;
        [field:SerializeField] public GameObject UfoEnemyPrefab;
    }
}
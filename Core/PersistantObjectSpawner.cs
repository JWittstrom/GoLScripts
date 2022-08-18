using UnityEngine;

namespace GoL.Core
{
    public class PersistantObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject _persistantObjectPrefab;
        static bool _hasSpawned = false;

        private void Awake()
        {
            if (_hasSpawned) return;

            SpawnPersistantObjects();

            _hasSpawned = true;
        }

        private void SpawnPersistantObjects()
        {
            GameObject _persistantObject = Instantiate(_persistantObjectPrefab);
            DontDestroyOnLoad(_persistantObject);
        }
    }
}
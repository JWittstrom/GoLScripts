using UnityEngine;

namespace GoL.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] GameObject _targetToDestroy = null;

        void Update()
        {
            if (!GetComponent<ParticleSystem>().IsAlive())
            {
                if (_targetToDestroy != null)
                {
                    Destroy(_targetToDestroy);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}

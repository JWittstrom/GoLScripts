using UnityEngine;
using GoL.Attributes;
using UnityEngine.Events;

namespace GoL.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float _speed = 1f;
        [SerializeField] bool _isHoming = true;
        [SerializeField] GameObject _hitEffect = null;
        [SerializeField] float _maxLifeTime = 10f;
        [SerializeField] GameObject[] _destroyOnHit = null;
        [SerializeField] float _lifeAfterImpact = 2f;
        [SerializeField] UnityEvent _onHit;

        Health _target = null;
        GameObject _instigator = null;
        float _damage = 0f;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if (_target == null) return;
            if (_isHoming && !_target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this._target = target;
            this._damage = damage;
            this._instigator = instigator;

            Destroy(gameObject, _maxLifeTime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider _targetCapsule = _target.GetComponent<CapsuleCollider>();
            if (_targetCapsule == null)
            {
                return _target.transform.position;
            }
            return _target.transform.position + Vector3.up * _targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != _target) return;
            if (_target.IsDead()) return;
            _target.TakeDamage(_instigator, _damage);

            _speed = 0f;

            _onHit.Invoke();

            if(_hitEffect != null)
            {
                Instantiate(_hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach(GameObject toDestroy in _destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, _lifeAfterImpact);
        }
    }
}

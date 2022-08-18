using UnityEngine;
using GoL.Combat;
using GoL.Core;
using GoL.Movement;
using GoL.Attributes;
using GameDevTV.Utils;
using System;

namespace GoL.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float _chaseDistance = 5f;
        [SerializeField] float _suspicionTime = 3f;
        [SerializeField] float _aggroCooldownTime = 5f;
        [SerializeField] PatrolPath _patrolPath;
        [SerializeField] float _waypointTolerance = 1f;
        [SerializeField] float _waypointDwellTime = 3f;
        [Range(0,1)] [SerializeField] float _patrolSpeedFraction = 0.2f;
        [SerializeField] float _shoutDistance = 5f;



        Fighter _fighter;
        Health _health;
        Mover _mover;
        GameObject _player;

        LazyValue<Vector3> _guardPosition;
        float _timeSinceLastSawPlayer = Mathf.Infinity;
        float _timeSinceArrivedAtWaypoint = Mathf.Infinity;
        float _timeSinceAggrevated = Mathf.Infinity;
        int _currentWaypointIndex = 0;

        private void Awake()
        {
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _player = GameObject.FindWithTag("Player");

            _guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start()
        {
            _guardPosition.ForceInit();
        }

        private void Update()
        {
            if (_health.IsDead()) return;

            if (IsAggrevated() && _fighter.CanAttack(_player))
            {
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer < _suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        public void Aggrevate()
        {
            _timeSinceAggrevated = 0f;
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedAtWaypoint += Time.deltaTime;
            _timeSinceAggrevated += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 _nextPosition = _guardPosition.value;

            if(_patrolPath != null)
            {
                if (AtWapoint())
                {
                    _timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                _nextPosition = GetCurrentWaypoint();
            }

            if (_timeSinceArrivedAtWaypoint > _waypointDwellTime)
            {
                _mover.StartMoveAction(_nextPosition, _patrolSpeedFraction);
            }
        }

        private bool AtWapoint()
        {
            float _distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return _distanceToWaypoint < _waypointTolerance;
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = _patrolPath.GetNextIndex(_currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return _patrolPath.GetWaypoint(_currentWaypointIndex);
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0;
            _fighter.Attack(_player);

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] _hits = Physics.SphereCastAll(transform.position, _shoutDistance, Vector3.up, 0);
            foreach (RaycastHit _hit in _hits)
            {
                AIController _ai = _hit.collider.GetComponent<AIController>();
                if (_ai == null) continue;

                _ai.Aggrevate();
            }
        }

        private bool IsAggrevated()
        {
            float _distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position);
            return _distanceToPlayer < _chaseDistance || _timeSinceAggrevated < _aggroCooldownTime;
        }

        //Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        }
    }
}
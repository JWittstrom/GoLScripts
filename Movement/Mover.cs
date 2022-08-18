using GoL.Core;
using UnityEngine;
using UnityEngine.AI;
using GoL.Saving;
using GoL.Attributes;

namespace GoL.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform _target;
        [SerializeField] float _maxSpeed = 6f;
        [SerializeField] float _maxNavPathLength = 40f;


        NavMeshAgent _navMeshAgent;
        Health _health;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();
        }

        void Update()
        {
            _navMeshAgent.enabled = !_health.IsDead();

            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath _path = new NavMeshPath();
            bool _hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, _path);
            if (!_hasPath) return false;
            if (_path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(_path) > _maxNavPathLength) return false;

            return true;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            _navMeshAgent.destination = destination;
            _navMeshAgent.speed = _maxSpeed * Mathf.Clamp01(speedFraction);
            _navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }
              
        private void UpdateAnimator()
        {
            Vector3 _velocity = _navMeshAgent.velocity;
            Vector3 _localVelocity = transform.InverseTransformDirection(_velocity);
            float _speed = _localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", _speed);
        }

        private float GetPathLength(NavMeshPath path)
        {
            float _total = 0f;
            if (path.corners.Length < 2) return _total;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                _total += Vector3.Distance(path.corners[1], path.corners[1 + i]);
            }

            return 0f;
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 _position = (SerializableVector3)state;
            _navMeshAgent.enabled = false;
            transform.position = _position.ToVector();
            _navMeshAgent.enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}

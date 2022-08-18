using UnityEngine;
using GoL.Movement;
using GoL.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace GoL.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health _health;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType _type;
            public Texture2D _texture;
            public Vector2 _hotspot;
        }

        [SerializeField] CursorMapping[] _cursorMappings = null;
        [SerializeField] float _maxNavMeshProjectionDistance = 1f;
        [SerializeField] float _raycastRadius = 1f;

        private void Awake()
        {
            _health = GetComponent<Health>();
        }


        private void Update()
        {
            if (InteractWithUI()) return;
            if (_health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;

            SetCursor(CursorType.None);
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] _hits = RaycastAllSorted();
            foreach (RaycastHit _hit in _hits)
            {
                IRaycastable[] _raycastables = _hit.transform.GetComponents<IRaycastable>();
                foreach(IRaycastable _raycastable in _raycastables)
                {
                    if (_raycastable.HandleRaycast(this))
                    {
                        SetCursor(_raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] _hits = Physics.SphereCastAll(GetMouseRay(), _raycastRadius);
            float[] _distances = new float[_hits.Length];
            for (int i = 0; i < _hits.Length; i++)
            {
                _distances[i] = _hits[i].distance;
            }
            Array.Sort(_distances, _hits);
            return _hits;
        }

        private bool InteractWithMovement()
        {
            Vector3 _target;
            bool _hasHit = RaycastNavMesh(out _target);
            if (_hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(_target)) return false;

                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(_target, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit _hit;
            bool _hasHit = Physics.Raycast(GetMouseRay(), out _hit);
            if (!_hasHit) return false;

            NavMeshHit _navMeshHit;
            bool _hasCastToNavMesh = NavMesh.SamplePosition(_hit.point, out _navMeshHit, _maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!_hasCastToNavMesh) return false;

            target = _navMeshHit.position;

            return true;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping _mapping = GetCursorMapping(type);
            Cursor.SetCursor(_mapping._texture, _mapping._hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in _cursorMappings)
            {
                if (mapping._type == type)
                {
                    return mapping;
                }               
            }
            return _cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
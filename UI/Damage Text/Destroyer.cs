using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class Destroyer : MonoBehaviour
    {
        [SerializeField] GameObject _targetToDestroy = null;

        public void DestroyTarget()
        {
            Destroy(_targetToDestroy);
        }
    }

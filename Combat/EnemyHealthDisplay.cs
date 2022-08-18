using GoL.Attributes;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GoL.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter _fighter;

        private void Awake()
        {
            _fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            if(_fighter.GetTarget() == null)
            {
                GetComponent<Text>().text = "N/A";
                return;
            }
            Health _health = _fighter.GetTarget();
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", _health.GetHealthPoints(), _health.GetMaxHealthPoints());
        }
    }
}
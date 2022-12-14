using UnityEngine;
using GoL.Saving;
using GoL.Stats;
using GoL.Core;
using System;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace GoL.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float _regenerationPercentage = 70f;
        [SerializeField] TakeDamageEvent _takeDamage;
        [SerializeField] UnityEvent _onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
        }

        LazyValue<float> _healthPoints;

        bool _isDead = false;

        private void Awake()
        {
            _healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start()
        {
            _healthPoints.ForceInit();
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>()._onLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>()._onLevelUp -= RegenerateHealth;
        }

        public bool IsDead()
        {
            return _isDead;
        }
                
        public void TakeDamage(GameObject instigator, float damage)
        {
            _healthPoints.value = Mathf.Max(_healthPoints.value - damage, 0);

            if(_healthPoints.value == 0)
            {
                _onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else
            {
                _takeDamage.Invoke(damage);
            }
        }

        public void Heal(float healthToRestore)
        {
            _healthPoints.value = Mathf.Min(_healthPoints.value + healthToRestore, GetMaxHealthPoints());
        }

        public float GetHealthPoints()
        {
            return _healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return _healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Die()
        {
            if (_isDead) return;

            _isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience _experience = instigator.GetComponent<Experience>();
            if (_experience == null) return;

            _experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        private void RegenerateHealth()
        {
            float _regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (_regenerationPercentage / 100);
            _healthPoints.value = Mathf.Max(_healthPoints.value, _regenHealthPoints);
        }

        public object CaptureState()
        {
            return _healthPoints.value;
        }

        public void RestoreState(object state)
        {
            _healthPoints.value = (float)state;

            if (_healthPoints.value == 0)
            {
                Die();
            }
        }
    }
}

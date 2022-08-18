using GameDevTV.Utils;
using System;
using UnityEngine;


namespace GoL.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)] [SerializeField] int _startingLevel = 1;
        [SerializeField] CharacterClass _characterClass;
        [SerializeField] Progression _progression = null;
        [SerializeField] GameObject _levelUpParticleEffect = null;
        [SerializeField] bool _shouldUseModifiers = false;

        public event Action _onLevelUp;

        LazyValue<int> _currentLevel;

        Experience _experience;

        private void Awake()
        {
            _experience = GetComponent<Experience>();
            _currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            _currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (_experience != null)
            {
                _experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (_experience != null)
            {
                _experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int _newLevel = CalculateLevel();
            if(_newLevel > _currentLevel.value)
            {
                _currentLevel.value = _newLevel;
                LevelUpEffect();
                _onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(_levelUpParticleEffect, transform);
        }

        public float GetStat(Stat stat)
        {
            // Base damage + weapon damage * percentage modifier
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifiers(stat) / 100);
        }

        private float GetBaseStat(Stat stat)
        {
            return _progression.GetStat(stat, _characterClass, GetLevel());
        }

        public int GetLevel()
        {
            return _currentLevel.value;
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!_shouldUseModifiers) return 0f;
            float _total = 0f;
            foreach (IModifierProvider _provider in GetComponents<IModifierProvider>())
            {
                foreach (float _modifier in _provider.GetAdditiveModifiers(stat))
                {
                    _total += _modifier;
                }
            }
            return _total;
        }

        private float GetPercentageModifiers(Stat stat)
        {
            if (!_shouldUseModifiers) return 0f;
            float _total = 0f;
            foreach (IModifierProvider _provider in GetComponents<IModifierProvider>())
            {
                foreach (float _modifier in _provider.GetPercentageModifiers(stat))
                {
                    _total += _modifier;
                }
            }
            return _total;
        }

        private int CalculateLevel()
        {
            Experience _experience = GetComponent<Experience>();
            if (_experience == null) return _startingLevel;

            float _currentXP = _experience.GetPoints();
            int _penultimateLevel = _progression.GetLevels(Stat.ExperienceToLevelUp, _characterClass);
            for (int level = 1; level <= _penultimateLevel; level++)
            {
                float _XPToLevelUp = _progression.GetStat(Stat.ExperienceToLevelUp, _characterClass, level);
                if(_XPToLevelUp > _currentXP)
                {
                    return level;
                }
            }
            return _penultimateLevel + 1;
        }
    }
}
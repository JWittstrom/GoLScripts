using UnityEngine;
using GoL.Saving;
using System;

namespace GoL.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float _experiencePoints = 0f;

        public event Action onExperienceGained;

        public void GainExperience(float experience)
        {
            _experiencePoints += experience;
            onExperienceGained();
        }


        public float GetPoints()
        {
            return _experiencePoints;
        }

        public object CaptureState()
        {
            return _experiencePoints;
        }

        public void RestoreState(object state)
        {
            _experiencePoints = (float)state;
        }
    }
}
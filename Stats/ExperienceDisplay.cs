using System;
using UnityEngine;
using UnityEngine.UI;

namespace GoL.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience _experience;

        private void Awake()
        {
            _experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0.0}", _experience.GetPoints());
        }
    }
}
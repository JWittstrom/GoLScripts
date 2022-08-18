using System;
using UnityEngine;
using UnityEngine.UI;

namespace GoL.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] Text _damageText = null;

        public void SetValue(float amount)
        {
            _damageText.text = String.Format("{0:0}", amount);
        }
    }
}
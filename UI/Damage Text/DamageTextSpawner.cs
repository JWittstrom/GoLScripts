using UnityEngine;

namespace GoL.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText _damageTextPrefab = null;

        public void Spawn(float damageAmount)
        {
            DamageText _inststance = Instantiate<DamageText>(_damageTextPrefab, transform);
            _inststance.SetValue(damageAmount);
        }
    }
}

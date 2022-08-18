using UnityEngine;
using GoL.Attributes;

namespace GoL.Combat
{
    [CreateAssetMenu(fileName ="Weapon", menuName ="Weapons/Make New Weapon", order =0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController _animatorOverride = null;
        [SerializeField] Weapon _equippedPrefab = null;
        [SerializeField] float _weaponDamage = 5f;
        [SerializeField] float _percentageBonus = 0f;
        [SerializeField] float _weaponRange = 2f;
        [SerializeField] bool _isRightHanded = true;
        [SerializeField] Projectile _projectile = null;

        const string _weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            Weapon _weapon = null;
            if (_equippedPrefab != null)
            {
                Transform _handTransform = GetTransform(rightHand, leftHand);
                _weapon = Instantiate(_equippedPrefab, _handTransform);
                _weapon.gameObject.name = _weaponName;
            }

            var _overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (_animatorOverride != null)
            {
                animator.runtimeAnimatorController = _animatorOverride;
            }
            else if (_overrideController != null)
            {
                animator.runtimeAnimatorController = _overrideController.runtimeAnimatorController;
            }

            return _weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform _oldWeapon = rightHand.Find(_weaponName);
            if(_oldWeapon == null)
            {
                _oldWeapon = leftHand.Find(_weaponName);
            }
            if (_oldWeapon == null) return;

            _oldWeapon.name = "DESTROYING";
            Destroy(_oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform _handTransform;
            if (_isRightHanded) _handTransform = rightHand;
            else _handTransform = leftHand;
            return _handTransform;
        }

        public bool HasProjectile()
        {
            return _projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile _projectileInstance = Instantiate(_projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            _projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public float GetDamage()
        {
            return _weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return _percentageBonus;
        }

        public float GetRange()
        {
            return _weaponRange;
        }
    }
}
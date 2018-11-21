using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Weapon"))]

    public class WeaponConfig : ScriptableObject
    {

        public Transform gripTransform;

        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;
        [SerializeField] float timeBetweenAnimationCycles;
        [SerializeField] float maxAttackRange;
        [SerializeField] float additionalDamage = 10f;
        [SerializeField] float damageDelay = .5f;
        [SerializeField] AudioClip[] audioClips = null;

        public float GetTimeBetweenAnimationCycles()
        {
            return timeBetweenAnimationCycles;
        }

        public float GetMaxAttackRange()
        {
            return maxAttackRange;
        }

        public float GetDamageDelay()
        {
            return damageDelay;
        }

        public GameObject GetWeaponPrefab()
        {
            return weaponPrefab;
        }

        public AnimationClip GetAttackAnimClip()
        {
            RemoveAnimationEvents();
            return attackAnimation;
        }

        public float GetAdditionalDamage()
        {
            return additionalDamage;
        }

        public AudioClip GetRandomWeaponSound()
        {
            if (audioClips.Length > 0)
            {
                return audioClips[Random.Range(0, audioClips.Length)];
            } else
            {
                return null;
            }
        }

        // SO that asset packs cannot cause crashes
        private void RemoveAnimationEvents()
        {
            attackAnimation.events = new AnimationEvent[0]; 
        }

        
    }
}

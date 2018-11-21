using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    public class WeaponSystem : MonoBehaviour
    {
        [Range(0f, 1.0f)] [SerializeField] float criticalHitChance = 0.1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] ParticleSystem criticalHitParticleSystem = null;

        [SerializeField] float baseDamage = 10f;
        [SerializeField] WeaponConfig currentWeaponConfig = null;

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";
        

        GameObject target;
        GameObject weaponObject;
        Animator animator = null;
        Character character;
        float lastHitTime;

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
            character = GetComponent<Character>();

            PutWeaponInHand(currentWeaponConfig);
            SetAttackAnimation();
        }

        void Update()
        {
            bool targetIsDead;
            bool targetIsOutOfRange;

            if(target == null)
            {
                targetIsDead = false;
                targetIsOutOfRange = false;
            } else
            {
                targetIsDead = target.GetComponent<HealthSystem>().HealthAsPercentage <= Mathf.Epsilon;
                
                float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
                targetIsOutOfRange = distanceToTarget > currentWeaponConfig.GetMaxAttackRange();

            }

            float characterHealth = GetComponent<HealthSystem>().HealthAsPercentage;
            bool characterIsDead = (characterHealth <= Mathf.Epsilon);

            if(characterIsDead || targetIsOutOfRange || targetIsDead)
            {
                StopAllCoroutines();
            }

        }

        public void AttackTarget(GameObject targetToAttack)
        {
            target = targetToAttack;
            StartCoroutine(AttackTargetRepeatedly());
        }

        public void StopAttacking()
        {
            animator.StopPlayback();
            StopAllCoroutines();
        }

        IEnumerator AttackTargetRepeatedly()
        {
            
            // determine if alive (attacker and defender)
            bool attackerStillAlive = GetComponent<HealthSystem>().HealthAsPercentage >= Mathf.Epsilon; // todo greater or greater equal?
            bool targetStillAlive = target.GetComponent<HealthSystem>().HealthAsPercentage >= Mathf.Epsilon;

            while(attackerStillAlive && targetStillAlive)
            {
                var animationClip = currentWeaponConfig.GetAttackAnimClip();
                float animationClipTime = animationClip.length / character.GetAnimSpeedMultiplier();
                float timeToWait = animationClipTime + currentWeaponConfig.GetTimeBetweenAnimationCycles();

                bool isTimeToHitAgain = Time.time - lastHitTime > timeToWait;

                if (isTimeToHitAgain)
                {
                    AttackTargetOnce();
                    lastHitTime = Time.time;
                }
                yield return new WaitForSeconds(timeToWait);
            }
            
        }

        private void AttackTargetOnce()
        {
            
            float damageDelay = currentWeaponConfig.GetDamageDelay();

            transform.LookAt(target.transform);
            animator.SetTrigger(ATTACK_TRIGGER);
            SetAttackAnimation();
            StartCoroutine(DamageAfterDelay(damageDelay));
        }

        IEnumerator DamageAfterDelay(float damageDelay)
        {
            yield return new WaitForSecondsRealtime(damageDelay);
            target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
            AudioSource audioSource = GetComponent<AudioSource>();
            AudioClip weaponSound = currentWeaponConfig.GetRandomWeaponSound();
            audioSource.PlayOneShot(weaponSound);
        }

        public WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig;
        }

        private void SetAttackAnimation()
        {
            if (!character.GetOverrideController())
            {
                Debug.Break();
                Debug.LogAssertion("Please provide " + gameObject + " with an animator override controller.");
            }

            var animatorOverrideController = character.GetOverrideController();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip();
        }

        public void PutWeaponInHand(WeaponConfig weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            Destroy(weaponObject); // empty hands
            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
        }

        private GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            Assert.IsFalse(numberOfDominantHands <= 0, "No DominantHand found on " + gameObject + ", please add one");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple DominatHand scripts on Player, please remove one");
            return dominantHands[0].gameObject;
        }


        private float CalculateDamage()
        {
            // allow for critical hit
            bool isCriticalHit = UnityEngine.Random.Range(0f, 1f) <= criticalHitChance;
            float damageBeforeCritical = baseDamage + currentWeaponConfig.GetAdditionalDamage();
            if (isCriticalHit)
            {
                criticalHitParticleSystem.Play();
                return damageBeforeCritical * criticalHitMultiplier;
            } else
            {
                return damageBeforeCritical;
            }

        }

       
    }
}
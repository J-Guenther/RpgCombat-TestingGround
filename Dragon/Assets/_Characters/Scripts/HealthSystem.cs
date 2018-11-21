using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace RPG.Characters
{
    public class HealthSystem : MonoBehaviour
    {

        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] Image healthBar;
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;
        [SerializeField] float deathVanishSeconds;

        const string DEATH_TRIGGER = "Death";

        float currentHealthPoints;
        Animator animator;
        AudioSource audioSource;
        Character characterMovement;

        public float HealthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            characterMovement = GetComponent<Character>();

            currentHealthPoints = maxHealthPoints;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            if(healthBar) // Enemies may not have health bars to update
            {
                healthBar.fillAmount = HealthAsPercentage;
            }
        }

        public void TakeDamage(float damage)
        {
            bool characterDies = (currentHealthPoints - damage <= 0);
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            var clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
            audioSource.PlayOneShot(clip);

            if (characterDies)
            {
                StartCoroutine(KillCharacter());
            }
        }

        public void Heal(float points)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + points, 0f, maxHealthPoints);
        }

        IEnumerator KillCharacter()
        {
            characterMovement.Kill();
            animator.SetTrigger(DEATH_TRIGGER);

            audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
            audioSource.Play(); // pverride any existing sounds (not OneShot)
            yield return new WaitForSecondsRealtime(audioSource.clip.length);

            var playerComponent = GetComponent<PlayerControl>();

            // if it is a Player
            if (playerComponent && playerComponent.isActiveAndEnabled) // lazy evaluation
            {
                SceneManager.LoadScene(0);
            } else
            {
                DestroyObject(gameObject, deathVanishSeconds);
            }


        }
    }
}
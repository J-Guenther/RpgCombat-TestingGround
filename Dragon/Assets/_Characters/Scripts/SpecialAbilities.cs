using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class SpecialAbilities : MonoBehaviour
    {
        [SerializeField] AbilityConfig[] abilities;
        [SerializeField] Image energyBar;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenPointsPerSecond = 5f;
        [SerializeField] AudioClip outOfEnergy;

        float currentEnergyPoints;
        AudioSource audioSource;

        public float EnergyAsPercentage { get { return currentEnergyPoints / maxEnergyPoints; } }

        // Use this for initialization
        void Start()
        {
            audioSource = GetComponent<AudioSource>();

            currentEnergyPoints = maxEnergyPoints;
            AttachInitialAbilities();
            UpdateEnergyBar();
        }

        private void Update()
        {
            if(currentEnergyPoints < maxEnergyPoints)
            {
                AddEnergyPoints();
                UpdateEnergyBar();
            }
        }

        private void AttachInitialAbilities()
        {
            for (int abilitiyIndex = 0; abilitiyIndex < abilities.Length; abilitiyIndex++)
            {
                abilities[abilitiyIndex].AttachAbilityTo(gameObject);
            }
        }

        public void AttemptSpecialAbility(int abilityIndex, GameObject target = null)
        {
            var energyComponent = GetComponent<SpecialAbilities>();
            var energyCost = abilities[abilityIndex].GetEnergyCost();

            if (energyCost <= currentEnergyPoints)
            {
                ConsumeEnergy(energyCost);
                abilities[abilityIndex].Use(target);
            } 
            else
            {
                audioSource.PlayOneShot(outOfEnergy);
            }
        }

        public int GetNumberOfAbilities()
        {
            return abilities.Length;
        }

        private void AddEnergyPoints()
        {
            float pointsToAdd = regenPointsPerSecond * Time.deltaTime;
            currentEnergyPoints += pointsToAdd;
        }

        public void ConsumeEnergy(float amount)
        {
            float newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0f, maxEnergyPoints);
            UpdateEnergyBar();
        }

        void UpdateEnergyBar()
        {
            if (energyBar)
            {
                energyBar.fillAmount = EnergyAsPercentage;
            }
            
        }
    }

}

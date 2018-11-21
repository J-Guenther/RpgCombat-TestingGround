using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI; // TODO consider re-wiring

namespace RPG.Characters
{
    [SelectionBase]
    public class Character : MonoBehaviour
    {
        [Header("Animator")]
        [SerializeField] RuntimeAnimatorController animatorController;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] Avatar characterAvatar;
        [SerializeField] [Range(.1f, 1f)] float animatorForwardCap = 1f;

        [Header("Audio")]
        [SerializeField] float audioSourceSpatialBlend = 0.5f;

        [Header("Capsule Collider")]
        [SerializeField] Vector3 colliderCenter = new Vector3(0, 0.9177701f, 0);
        [SerializeField] float colliderRadius = 0.2f;
        [SerializeField] float colliderHeight = 1.83554f;

        [Header("Movement Properties")]
        [SerializeField] float moveSpeedMultiplier = 0.7f;
        [SerializeField] float animationSpeedMultiplier = 1.5f;
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;
        [SerializeField] float moveThreshold = 1f;

        [Header("Nav Mesh Agent")]
        [SerializeField] float navMeshAgentSteeringSpeed = 1f;
        [SerializeField] float navMeshAgentStoppingDistance = 1f;

        private NavMeshAgent navMeshAgent;
        private Animator animator;
        private Rigidbody myRigidbody;
        float turnAmount;
        float forwardAmount;
        private bool isAlive = true;

        private void Awake()
        {
            AddRequiredComponents();
        }

        private void AddRequiredComponents()
        {
            var capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = colliderCenter;
            capsuleCollider.radius = colliderRadius;
            capsuleCollider.height = colliderHeight;

            myRigidbody = gameObject.AddComponent<Rigidbody>();
            myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            animator.avatar = characterAvatar;

            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = audioSourceSpatialBlend;

            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.autoBraking = false;
            navMeshAgent.updatePosition = true;
            navMeshAgent.updateRotation = false;
            navMeshAgent.stoppingDistance = navMeshAgentStoppingDistance;
            navMeshAgent.speed = navMeshAgentSteeringSpeed;
        }

        private void Update()
        {
            if (!navMeshAgent.isOnNavMesh)
            {
                Debug.LogError(gameObject.name + " is not on the navmesh");
            }

            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance && isAlive)
            {
                Move(navMeshAgent.desiredVelocity);
            } else
            {
                Move(Vector3.zero);
            }
        }

        public float GetAnimSpeedMultiplier()
        {
            return animator.speed;
        }

        public void SetDestination(Vector3 worldPos)
        {
            navMeshAgent.destination = worldPos;
        }

        public AnimatorOverrideController GetOverrideController()
        {
            return animatorOverrideController;
        }

        void Move(Vector3 movement)
        {
            SetForwardAndTurn(movement);
            ApplyExtraTurnRotation();
            UpdateAnimator();
        }

        public void Kill()
        {
            isAlive = false;
        }

        private void SetForwardAndTurn(Vector3 movement)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired direction.
            if (movement.magnitude > moveThreshold)
            {
                movement.Normalize();
            }

            var localMove = transform.InverseTransformDirection(movement);
            turnAmount = Mathf.Atan2(localMove.x, localMove.z);
            forwardAmount = localMove.z;
        }


        void UpdateAnimator()
        {
            animator.SetFloat("Forward", forwardAmount * animatorForwardCap, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
            animator.speed = animationSpeedMultiplier;
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }



        private void OnAnimatorMove()
        {
            if (Time.deltaTime > 0)
            {
                Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

                velocity.y = myRigidbody.velocity.y;
                myRigidbody.velocity = velocity;
            }
        }



        //    private void OnDrawGizmos()
        //    {
        //        // Draw movement gizmos
        //        Gizmos.color = Color.black;
        //        Gizmos.DrawLine(transform.position, agent.destination);


        //        // Draw attack sphere
        //        //Gizmos.color = new Color(255f, 0f, 0f, .5f);
        //        //GizmosDrawCircle(transform.position, attackMoveStopRadius);
        //    }

        //    void GizmosDrawCircle(Vector3 center, float radius, float step = 0.1f)
        //    {
        //        float theta = 0.0f;
        //        float x = radius * Mathf.Cos(theta);
        //        float y = radius * Mathf.Sin(theta);
        //        Vector3 position = center + new Vector3(x, 0, y);
        //        Vector3 newPosition = position;
        //        Vector3 lastPosition = position;
        //        for (theta = step; theta < Mathf.PI * 2; theta += step)
        //        {
        //            x = radius * Mathf.Cos(theta);
        //            y = radius * Mathf.Sin(theta);
        //            newPosition = center + new Vector3(x, 0, y);
        //            Gizmos.DrawLine(position, newPosition);
        //            position = newPosition;
        //        }
        //        Gizmos.DrawLine(position, lastPosition);
        //    }
    }


}
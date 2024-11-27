using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;





public class Enemy : MonoBehaviour
{
    public Animator animator;
    public int HP; // Enemy health points
    public PlayerController pc; // Reference to the player controller
    public NavMeshAgent agent; // NavMeshAgent for pathfinding
    public float attackCD; // Attack cooldown time
    private float attackTimer; // Tracks last attack time
    public int attackValue; // Attack damage value
    public AudioSource audioSource; // Audio source for sound effects
    public AudioClip attackAudio; // Attack sound effect
    private bool isDead; // Tracks if the enemy is dead
    public AudioClip dieAudio; // Death sound effect
    private bool hasTarget; // Tracks if the enemy has a target

    void Start()
    {

    }

    void Update()
    {
        if (isDead)
        {
            return;
        }
        if (Vector3.Distance(transform.position, pc.transform.position) >= 5)
        {
            // If the player is more than 5 meters away, the enemy does nothing
            return;
        }
        else
        {
            // If the player is within 5 meters, the enemy identifies the player as a target
            hasTarget = true;
        }
        if (hasTarget) // Once a target is acquired
        {
            if (Vector3.Distance(transform.position, pc.transform.position) <= 1)
            {
                // Attack if the player is within 1 meter
                Attack();
            }
            else
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                agent.isStopped = false;
                agent.SetDestination(pc.transform.position);
                animator.SetFloat("MoveState", 1); // Set animation to moving state
            }
        }
    }

    /// <summary>
    /// Take damage from the player
    /// </summary>
    public void TakeDamage(int attackValue)
    {
        animator.SetTrigger("Hit"); // Trigger hit animation
        HP -= attackValue; // Reduce health points
        if (HP <= 0)
        {
            animator.SetBool("Die", true); // Trigger death animation
            agent.isStopped = true; // Stop the enemy's movement
            isDead = true; // Mark the enemy as dead
            audioSource.Stop();
            audioSource.PlayOneShot(dieAudio); // Play death sound effect
        }
    }

    /// <summary>
    /// Enemy attack functionality
    /// </summary>
    private void Attack()
    {
        agent.isStopped = true; // Stop moving while attacking
        animator.SetFloat("MoveState", 0); // Stop movement animation
        if (Time.time - attackTimer >= attackCD)
        {
            animator.SetTrigger("Attack"); // Trigger attack animation
            attackTimer = Time.time; // Update the attack timer
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            Invoke("DelayPlayAttackSound", 1); // Delay attack sound by 1 second
        }
    }

    /// <summary>
    /// Play attack sound after a delay
    /// </summary>
    private void DelayPlayAttackSound()
    {
        pc.TakeDamage(attackValue); // Deal damage to the player
        audioSource.PlayOneShot(attackAudio); // Play attack sound effect
    }
}

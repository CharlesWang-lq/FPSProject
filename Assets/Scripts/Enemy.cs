using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public int HP; // Enemy health
    public PlayerController pc; // Reference to the player controller
    public NavMeshAgent agent; // Navigation agent
    public float attackCD; // Attack cooldown
    private float attackTimer; // Timer to track last attack
    public int attackValue; // Damage dealt by enemy
    public AudioSource audioSource; // Audio source for sound effects
    public AudioClip attackAudio; // Attack sound effect
    private bool isDead; // Is the enemy dead
    public AudioClip dieAudio; // Death sound effect
    private bool hasTarget; // Does the enemy have a target

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
            // If farther than 5 meters, the enemy does nothing
            return;
        }
        else
        {
            // Within 5 meters, the enemy sees the player and gains a target
            hasTarget = true;
        }
        if (hasTarget) // When the enemy has a target
        {
            if (Vector3.Distance(transform.position, pc.transform.position) <= 1)
            {
                // Attack the player
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
                animator.SetFloat("MoveState", 1);
            }
        }
    }

    /// <summary>
    /// Enemy takes damage
    /// </summary>
    public void TakeDamage(int attackValue)
    {
        animator.SetTrigger("Hit");
        HP -= attackValue;
        if (HP <= 0)
        {
            animator.SetBool("Die", true);
            agent.isStopped = true;
            isDead = true;
            audioSource.Stop();
            audioSource.PlayOneShot(dieAudio);
        }
    }
    /// <summary>
    /// Enemy attacks the player
    /// </summary>
    private void Attack()
    {
        agent.isStopped = true;
        animator.SetFloat("MoveState", 0);
        if (Time.time - attackTimer >= attackCD)
        {
            animator.SetTrigger("Attack");
            attackTimer = Time.time;
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            Invoke("DelayPlayAttackSound", 1);
        }
    }
    /// <summary>
    /// Play attack sound effect after a delay
    /// </summary>
    private void DelayPlayAttackSound()
    {
        pc.TakeDamage(attackValue);
        audioSource.PlayOneShot(attackAudio);
    }
}

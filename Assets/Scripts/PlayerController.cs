// using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour //ai help generator some of the code
{
    public float moveSpeed; // Movement speed
    public float rotateSpeed; // Rotation angle
    private float angleY; // Horizontal view angle, rotates around Y-axis
    private float angleX; // Vertical view angle, rotates around X-axis
    public Animator animator; // Animation controller
    public Rigidbody rigid; // Rigidbody
    public float jumpForce; // Jump force
    public Transform gunPointTrans; // Gun muzzle position
    public GameObject bloodEffectGo; // Blood effect prefab
    public GameObject grassEffectGo; // Ground hit effect prefab
    public GameObject pinkFlowerEffectGo; // Pink flower effect
    public GameObject woodEffectGo; // Tree trunk effect
    public GameObject riverEffectGo; // Water effect
    public GameObject otherEffectGo; // Other effects

    public float attackCD; // Attack cooldown
    public float attackTimer; // Records last attack time
    public Transform attackEffectTrans; // Position for spawning attack effects
    public GameObject singleAttackEffectGo; // Single-shot gun attack effect

    public GUNTYPE gunType; // Current gun type
    public GameObject autoAttackEffectGo; // Automatic gun attack effect
    public GameObject snipingAttackEffectGo; // Sniper gun attack effect
    // Bullets in the clip
    private Dictionary<GUNTYPE, int> bulletsClip = new Dictionary<GUNTYPE, int>();

    public int maxSingleShotBullets;
    public int maxAutoShotBullets;
    public int maxSnipingShotBullets;

    public bool isReloading; // Reloading status

    public GameObject[] gunGo;

    public GameObject scope; // Scope

    public int HP;

    // Bullet power for each weapon type
    private Dictionary<GUNTYPE, int> gunWeaponDamage = new Dictionary<GUNTYPE, int>();
    
    public AudioSource audioSource;
    public AudioClip singleShootAudio;
    public AudioClip autoShootAudio;
    public AudioClip snipingShootAudio;
    public AudioClip reloadAudio;
    public AudioClip hitGroundAudio;
    public AudioClip jumpAudio;
    public AudioClip fallOffAudio;
    public AudioSource moveAudioSource;

    public Text playerHPText;

    public GameObject[] gunUIGos;

    public Text bulletText;
    
    public Text ammoBagText;
    public Text countdownText;

    public GameObject bloodUIGo;
    public GameObject fallOffScreenEffect;

    public GameObject scopeUIGo;

    public GameObject gameOverPanel;

    private bool isFiring = false; // Tracks whether the player is currently firing
    private bool isFallingOff = false; // Tracks if the player is in fall-off process

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        bulletsClip.Add(GUNTYPE.SINGLESHOT, maxSingleShotBullets);
        bulletsClip.Add(GUNTYPE.AUTO, maxAutoShotBullets);
        bulletsClip.Add(GUNTYPE.SNIPING, maxSnipingShotBullets);
        gunWeaponDamage.Add(GUNTYPE.SINGLESHOT, 2);
        gunWeaponDamage.Add(GUNTYPE.AUTO, 1);
        gunWeaponDamage.Add(GUNTYPE.SNIPING, 5);
        ammoBagText.text = "=∞";
        fallOffScreenEffect.SetActive(false);
        countdownText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isFallingOff)
        {
            PlayerMove();
            LookAround();
            Attack();
            Jump();
            ChangeGun();
            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }
            OpenOrCloseScope();
            CheckFallOffMap();
        }
    }

    /// <summary>
    /// Player movement
    /// </summary>
    private void PlayerMove()
    {
        // Vertical axis input
        float verticalInput = Input.GetAxis("Vertical");
        // Horizontal axis input
        float horizontalInput = Input.GetAxis("Horizontal");
        // Vertical displacement
        Vector3 movementV = transform.forward * verticalInput * moveSpeed
            * Time.deltaTime;
        // Horizontal displacement
        Vector3 movementH = transform.right * horizontalInput * moveSpeed
            * Time.deltaTime;
        // Add displacement to position
        transform.position += movementV + movementH;
        animator.SetFloat("MoveX", horizontalInput);
        animator.SetFloat("MoveY", verticalInput);
        if (verticalInput > 0 || horizontalInput > 0)
        {
            if (!moveAudioSource.isPlaying)
            {
                moveAudioSource.Play();
            }
        }
        else
        {
            if (moveAudioSource.isPlaying)
            {
                moveAudioSource.Stop();
            }
        }
    }

    /// <summary>
    /// Look around
    /// </summary>
    private void LookAround()
    {
        // Horizontal view
        float mouseX = Input.GetAxis("Mouse X"); // Mouse horizontal axis input
        float lookHAngleY = mouseX * rotateSpeed; // Adjust horizontal view angle
        angleY = angleY + lookHAngleY;

        // Vertical view
        float mouseY = -Input.GetAxis("Mouse Y"); // Mouse vertical axis input
        float lookVAngleX = mouseY * rotateSpeed; // Adjust vertical view angle
        angleX = Mathf.Clamp(angleX + lookVAngleX, -60, 60);

        transform.eulerAngles = new Vector3(angleX, angleY, transform.eulerAngles.z);
    }

    /// <summary>
    /// Jump functionality
    /// </summary>
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigid.AddForce(jumpForce * Vector3.up);
            audioSource.PlayOneShot(jumpAudio);
        }
    }
    
    /// <summary>
    /// Attack functionality
    /// </summary>
    private void Attack()
    {
        if (!isReloading)
        {
            switch (gunType)
            {
                case GUNTYPE.SINGLESHOT:
                    SingleShotAttack();
                    break;
                case GUNTYPE.AUTO:
                    AutoAttack();
                    break;
                case GUNTYPE.SNIPING:
                    SnipingAttack();
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Switch weapons
    /// </summary>
    private void ChangeGun()
    {
        if (Input.GetKeyDown(KeyCode.C) && !isReloading && !isFiring)
        {
            gunType++;
            if (gunType > GUNTYPE.SNIPING)
            {
                gunType = 0;
            }
            switch (gunType)
            {
                case GUNTYPE.SINGLESHOT:
                    attackCD = 0.2f;
                    ChangeGunGameObject(0);
                    break;
                case GUNTYPE.AUTO:
                    attackCD = 0.1f;
                    ChangeGunGameObject(1);
                    break;
                case GUNTYPE.SNIPING:
                    attackCD = 1;
                    ChangeGunGameObject(2);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Display the corresponding game object
    /// </summary>
    private void ChangeGunGameObject(int gunLevel)
    {
        for (int i = 0; i < gunGo.Length; i++)
        {
            gunGo[i].SetActive(false);
            gunUIGos[i].SetActive(false);
        }
        gunGo[gunLevel].SetActive(true);
        gunUIGos[gunLevel].SetActive(true);
        bulletText.text = "X" + bulletsClip[gunType].ToString();
        ammoBagText.text = "=∞";
    }

    /// <summary>
    /// Single-shot gun attack
    /// </summary>
    private void HandleAttack(AudioClip shootAudio, GameObject attackEffectPrefab, string animationTrigger, bool isAuto = false)
    {
        if ((isAuto ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0)) && Time.time - attackTimer >= attackCD)
        {
            isFiring = true; // Set firing state

            if (bulletsClip[gunType] > 0) // If the clip has bullets, attack is possible
            {
                PlaySound(shootAudio);
                bulletsClip[gunType]--;
                UpdateBulletText();
                
                if (isAuto)
                    animator.SetBool(animationTrigger, true);
                else
                    animator.SetTrigger(animationTrigger);

                CreateAttackEffect(attackEffectPrefab);
                
                if (!isAuto)
                    Invoke("GunAttack", GetInvokeDelay());
                else
                    GunAttack();
            }
            else // If the clip is empty, reload bullets
            {
                Reload();
            }
        }
        else if (isAuto && Input.GetMouseButtonUp(0))
        {
            // Stop automatic attack when button is released
            isFiring = false; // Reset firing state
            animator.SetBool(animationTrigger, false);
        }
    }

    private void CreateAttackEffect(GameObject attackEffectPrefab)
    {
        GameObject go = Instantiate(attackEffectPrefab, attackEffectTrans);
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
    }

    private void UpdateBulletText()
    {
        bulletText.text = "X" + bulletsClip[gunType].ToString();
    }

    private float GetInvokeDelay()
    {
        return gunType == GUNTYPE.SNIPING ? 0.25f : 0.1f; // Add other delays if necessary
    }

    // Single-shot gun attack
    private void SingleShotAttack()
    {
        HandleAttack(singleShootAudio, singleAttackEffectGo, "SingleAttack");
        isFiring = false; // Reset firing state
    }

    // Automatic gun attack
    private void AutoAttack()
    {
        HandleAttack(autoShootAudio, autoAttackEffectGo, "AutoAttack", true);
    }

    // Sniper gun attack
    private void SnipingAttack()
    {
        HandleAttack(snipingShootAudio, snipingAttackEffectGo, "SnipingAttack");
        isFiring = false; // Reset firing state
    }

    /// <summary>
    /// Core shooting functionality
    /// </summary>
    private void GunAttack()
    {
        RaycastHit hit;
        attackTimer = Time.time;
        if (Physics.Raycast(gunPointTrans.position, gunPointTrans.forward, out hit, 5))
        {
            // Generate effects based on the hit object
            switch (hit.collider.tag)
            {
                case "Enemy":
                    hit.transform.GetComponent<Enemy>().TakeDamage(gunWeaponDamage[gunType]);
                    Instantiate(bloodEffectGo, hit.point, Quaternion.identity);
                    break;
                case "Flowers":
                    PlaySound(hitGroundAudio);
                    Instantiate(pinkFlowerEffectGo, hit.point, Quaternion.identity);
                    break;
                case "Hourse":
                    PlaySound(hitGroundAudio);
                    Instantiate(bloodEffectGo, hit.point, Quaternion.identity);
                    break;
                case "Grass":
                    PlaySound(hitGroundAudio);
                    Instantiate(grassEffectGo, hit.point, Quaternion.identity);
                    break;
                case "Wood":
                    PlaySound(hitGroundAudio);
                    Instantiate(woodEffectGo, hit.point, Quaternion.identity);
                    break;
                case "Others":
                    PlaySound(hitGroundAudio);
                    Instantiate(otherEffectGo, hit.point, Quaternion.identity);
                    break;
                case "River":
                    PlaySound(hitGroundAudio);
                    Instantiate(riverEffectGo, hit.point, Quaternion.identity);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Reload bullets
    /// </summary>
    private void Reload()
    {
        int maxBullets = GetMaxBulletsForGun(gunType);
        if (bulletsClip[gunType] >= maxBullets) return;

        PlaySound(reloadAudio);
        isReloading = true;
        isFiring = false;
        Invoke("RecoverAttackState", 2.667f);
        animator.SetTrigger("Reload");

        bulletsClip[gunType] = maxBullets;
        UpdateBulletText();
    }

    // Helper function to get the maximum bullets for the current gun type
    private int GetMaxBulletsForGun(GUNTYPE gunType)
    {
        return gunType switch
        {
            GUNTYPE.SINGLESHOT => maxSingleShotBullets,
            GUNTYPE.AUTO => maxAutoShotBullets,
            GUNTYPE.SNIPING => maxSnipingShotBullets,
            _ => 0
        };
    }

    /// <summary>
    /// Finished reloading, ready to attack
    /// </summary>
    private void RecoverAttackState()
    {
        isReloading = false;
    }

    /// <summary>
    /// Toggle the scope
    /// </summary>
    private void OpenOrCloseScope()
    {
        if (Input.GetMouseButton(1)||Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            scope.SetActive(true);
            scopeUIGo.SetActive(true);
        }
        else
        {
            scope.SetActive(false);
            scopeUIGo.SetActive(false);
        }
    }

    /// <summary>
    /// Check if player falls off the map
    /// </summary>
    private void CheckFallOffMap()
    {
        if (transform.position.y < -10 && !isFallingOff) // Adjust this value as needed for your map
        {
            StartCoroutine(FallOffRoutine());
        }
    }

    /// <summary>
    /// Handles the fall-off routine, including sound, screen effect, health reduction, and respawn
    /// </summary>
    private IEnumerator FallOffRoutine()
    {
        isFallingOff = true;

        // Play fall-off sound
        PlaySound(fallOffAudio);

        // Show red screen effect
        fallOffScreenEffect.SetActive(true);

        // Reduce player health
        HP -= 50;
        if (HP <= 0)
        {
            HP = 0;
            gameOverPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        playerHPText.text = HP.ToString();

        // Wait for a moment to show the effect
        yield return new WaitForSeconds(1f);

        // Hide screen effect
        fallOffScreenEffect.SetActive(false);

        // Respawn the player
        Respawn();

        isFallingOff = false;
    }

    /// <summary>
    /// Respawn the player to a safe position
    /// </summary>
    private void Respawn()
    {
        // Set the player position to a predefined spawn point or safe location
        transform.position = new Vector3(0, 10, 0); // Example spawn point, adjust as needed
        rigid.velocity = Vector3.zero; // Reset velocity to avoid unexpected movement
    }

    /// <summary>
    /// Take damage
    /// </summary>
    /// <param name="value">Amount of damage</param>
    public void TakeDamage(int value)
    {
        bloodUIGo.SetActive(true);
        Invoke("HideBloodUIGo", 0.5f);
        HP -= value;
        if (HP <= 0)
        {
            HP = 0;
            gameOverPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        playerHPText.text = HP.ToString();
    }

    /// <summary>
    /// Play sound effects
    /// </summary>
    public void PlaySound(AudioClip ac)
    {
        audioSource.PlayOneShot(ac);
    }

    private void HideBloodUIGo()
    {
        bloodUIGo.SetActive(false);
    }

    public void Replay()
    {
        SceneManager.LoadScene(0);
    }
}

[System.Serializable]
public enum GUNTYPE
{ 
    SINGLESHOT,
    AUTO,
    SNIPING
}

// using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
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
    public float attackTimer; // Tracks last attack time
    public Transform attackEffectTrans; // Position for spawning attack effects
    public GameObject singleAttackEffectGo; // Single-shot gun attack effect

    public GUNTYPE gunType; // Current gun type
    public GameObject autoAttackEffectGo; // Automatic gun attack effect
    public GameObject snipingAttackEffectGo; // Sniper gun attack effect
    // Total bullets in inventory
    private Dictionary<GUNTYPE, int> bulletsBag = new Dictionary<GUNTYPE, int>();
    // Bullets in the clip
    private Dictionary<GUNTYPE, int> bulletsClip = new Dictionary<GUNTYPE, int>();

    public int maxSingleShotBullets;
    public int maxAutoShotBullets;
    public int maxSnipingShotBullets;

    public bool isReloading; // Reloading status

    public GameObject[] gunGo;

    public GameObject scope; // Scope object

    public int HP; // Player's health points

    // Bullet power for each weapon type
    private Dictionary<GUNTYPE, int> gunWeaponDamage = new Dictionary<GUNTYPE, int>();

    public AudioSource audioSource;
    public AudioClip singleShootAudio; // Single-shot sound effect
    public AudioClip autoShootAudio; // Automatic gun sound effect
    public AudioClip snipingShootAudio; // Sniper gun sound effect
    public AudioClip reloadAudio; // Reload sound effect
    public AudioClip hitGroundAudio; // Ground hit sound effect
    public AudioClip jumpAudio; // Jump sound effect
    public AudioSource moveAudioSource;

    public Text playerHPText;

    public GameObject[] gunUIGos;

    public Text bulletText;

    public GameObject bloodUIGo;

    public GameObject scopeUIGo;

    public GameObject gameOverPanel;


    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        bulletsBag.Add(GUNTYPE.SINGLESHOT,30);
        bulletsBag.Add(GUNTYPE.AUTO, 50);
        bulletsBag.Add(GUNTYPE.SNIPING, 5);
        bulletsClip.Add(GUNTYPE.SINGLESHOT, maxSingleShotBullets);
        bulletsClip.Add(GUNTYPE.AUTO, maxAutoShotBullets);
        bulletsClip.Add(GUNTYPE.SNIPING, maxSnipingShotBullets);
        gunWeaponDamage.Add(GUNTYPE.SINGLESHOT, 2);
        gunWeaponDamage.Add(GUNTYPE.AUTO, 1);
        gunWeaponDamage.Add(GUNTYPE.SNIPING, 5);
    }

    void Update()
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
    }
    /// <summary>
    /// Player Movement
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
        // Mouse horizontal axis input
        float mouseX = Input.GetAxis("Mouse X");
        // Adjust horizontal view angle (player input value)
        float lookHAngleY = mouseX * rotateSpeed;
        angleY = angleY + lookHAngleY;

        // Vertical view
        // Mouse vertical axis input (player input value)
        float mouseY = -Input.GetAxis("Mouse Y");
        // Adjust vertical view angle
        float lookVAngleX = mouseY * rotateSpeed;
        angleX = Mathf.Clamp(angleX + lookVAngleX, -60, 60);

        transform.eulerAngles = new Vector3(
            angleX, angleY, transform.eulerAngles.z);
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
    /// Switch weapons
    /// </summary>
    private void ChangeGun()
    {
        if (Input.GetKeyDown(KeyCode.C) && !isReloading)
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
    /// Display corresponding game object
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
    }

     /// <summary>
    /// Single-shot gun attack
    /// </summary>
    private void SingleShotAttack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - attackTimer >= attackCD)
        {
            // Get the number of bullets in the clip for the currently used gun
            if (bulletsClip[gunType] > 0) // If the clip still has bullets, the player can attack
            {
                PlaySound(singleShootAudio);
                bulletsClip[gunType]--;
                bulletText.text = "X" + bulletsClip[gunType].ToString();
                animator.SetTrigger("SingleAttack");
                GameObject go = Instantiate(singleAttackEffectGo, attackEffectTrans);
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                Invoke("GunAttack", 0.1f);
            }
            else // If the clip is empty, bullets need to be reloaded from the inventory
            {
                Reload();
            }
        }
    }

    /// <summary>
    /// Automatic gun attack
    /// </summary>
    private void AutoAttack()
    {
        if (Input.GetMouseButton(0) && Time.time - attackTimer >= attackCD)
        {
            // Get the number of bullets in the clip for the currently used gun
            if (bulletsClip[gunType] > 0) // If the clip still has bullets, the player can attack
            {
                PlaySound(autoShootAudio);
                bulletsClip[gunType]--;
                bulletText.text = "X" + bulletsClip[gunType].ToString();
                animator.SetBool("AutoAttack", true);
                GameObject go = Instantiate(autoAttackEffectGo, attackEffectTrans);
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                GunAttack();
            }
            else // If the clip is empty, bullets need to be reloaded from the inventory
            {
                Reload();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool("AutoAttack", false);
        }
    }

    /// <summary>
    /// Sniper gun attack
    /// </summary>
    private void SnipingAttack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - attackTimer >= attackCD)
        {
            // Get the number of bullets in the clip for the currently used gun
            if (bulletsClip[gunType] > 0) // If the clip still has bullets, the player can attack
            {
                PlaySound(snipingShootAudio);
                bulletsClip[gunType]--;
                bulletText.text = "X" + bulletsClip[gunType].ToString();
                animator.SetTrigger("SnipingAttack");
                GameObject go = Instantiate(snipingAttackEffectGo, attackEffectTrans);
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                Invoke("GunAttack", 0.25f);
            }
            else // If the clip is empty, bullets need to be reloaded from the inventory
            {
                Reload();
            }
        }
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
            // Generate effects based on what the bullet hits
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
        bool canReload = false; // Determines if bullets can be reloaded
        switch (gunType)
        {
            case GUNTYPE.SINGLESHOT:
                if (bulletsClip[gunType] < maxSingleShotBullets)
                {
                    canReload = true;
                }                
                break;
            case GUNTYPE.AUTO:
                if (bulletsClip[gunType] < maxAutoShotBullets)
                {
                    canReload = true;
                }
                break;
            case GUNTYPE.SNIPING:
                if (bulletsClip[gunType] < maxSnipingShotBullets)
                {
                    canReload = true;
                }
                break;
            default:
                break;
        }
        if (canReload) // If the clip isn't full, reload it
        {
            PlaySound(reloadAudio);
            if (bulletsBag[gunType] > 0) // If the inventory still has bullets
            {
                isReloading = true;
                Invoke("RecoverAttackState", 2.667f);
                animator.SetTrigger("Reload");
                switch (gunType)
                {
                    case GUNTYPE.SINGLESHOT:
                        if (bulletsBag[gunType] >= maxSingleShotBullets)
                        {
                            if (bulletsClip[gunType] > 0) // If the clip has some remaining bullets, fill it up
                            {
                                int bulletNum = maxSingleShotBullets - bulletsClip[gunType];
                                bulletsBag[gunType] -= bulletNum;
                                bulletsClip[gunType] += bulletNum;
                            }
                            else // If the clip is empty, fill it to max capacity
                            {
                                bulletsBag[gunType] -= maxSingleShotBullets;
                                bulletsClip[gunType] += maxSingleShotBullets;
                            }
                        }
                        else
                        {
                            bulletsClip[gunType] += bulletsBag[gunType];
                            bulletsBag[gunType] = 0;
                        }
                        break;
                    case GUNTYPE.AUTO:
                        // Similar logic for automatic guns
                        break;
                    case GUNTYPE.SNIPING:
                        // Similar logic for sniper guns
                        break;
                    default:
                        break;
                }
            }
            bulletText.text = "X" + bulletsClip[gunType].ToString();
        }
        animator.SetBool("AutoAttack", false);
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
        if (Input.GetMouseButton(1) && gunType == GUNTYPE.SNIPING)
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
    /// Take damage
    /// </summary>
    /// <param name="value"></param>
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
    /// Play audio
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

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    #region Variablen
    public int scores = 5;

    [Header("Player Ground Movement")]
    [SerializeField] float maxMovementSpeed = 5f;
    [SerializeField] float aimMovementSpeed = 2.5f;
    [SerializeField] float movementAcceleration = 10f;
    [SerializeField] float rotationAcceleration = 10f;
    [SerializeField] float maxSprintMultiplicator = 1.47f;
    [SerializeField] float maxCrouchMultiplicator = 0.8f;
    [Space]
    [Header("Player Air")]
    [SerializeField] float gravity = 30f;
    [SerializeField] float gravityAcceleration = 10f;
    [SerializeField] float mouseSensitivity = 1f;
    [SerializeField] Transform cam;
    [SerializeField] LayerMask aimColliderMask = new LayerMask();
    [SerializeField] Transform debugTransform;
    [Space]
    [Header("Shooting")]
    [SerializeField] GameObject holdingWeapon;
    [SerializeField] GameObject equippedWeapon;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject muzzlePrefab;
    [SerializeField] Transform bulletSpawnPos;
    [SerializeField] float timerBetweenBullets = .2f;
    [SerializeField] float bulletSpeed = 150f;
    [SerializeField] float timerBtwMuzzle = .8f;
    [Space]
    [Header("Cinemachine")]
    [SerializeField] GameObject CinemachineCameraTarget;
    [SerializeField] GameObject followCam;
    [SerializeField] GameObject aimCam;
    [SerializeField] GameObject deathCam;
    [SerializeField] CinemachineBrain cineBrain;
    [SerializeField] float TopClamp = 70.0f;
    [SerializeField] float BottomClamp = -30.0f;
    [SerializeField] float CameraAngleOverride = 0.0f;
    public bool LockCameraPosition = false;
    [SerializeField] float shakeStrenght = .5f;
    [SerializeField] float shakeTimer = .2f;
    [Space]
    [Header("UI")]
    [SerializeField] Image crosshair;
    [SerializeField] Canvas ui;
    [SerializeField] Transform tabParent;
    [SerializeField] Image timeRedValue;
    [SerializeField] Text timeValueText;
    [SerializeField] Text scoreValueText;
    [SerializeField] Text maxScoreValueText;
    [SerializeField] Slider sensSlider;
    [SerializeField] GameObject escapeUI;
    [Space]
    [Header("Audio")]
    [SerializeField] AudioTriggerFX footSteps = default;
    [Space]
    [Header("DefaultWeapon")]
    [SerializeField] WeaponScriptObject defaultWeapon = default;
    [SerializeField] Transform handTransform = default;
    [SerializeField] WeaponScriptObject[] weaponList = default;

    CharacterController controller;
    Animator anim;
    Vector3 velocity;
    Vector3 mouseWorldPos;
    Vector3 target;
    Ray ray;

    WeaponScriptObject currentWeapon;
    PlayerHealth health;
    HandleWeapon weaponInfo;

    WeaponScriptObject newWeapon;
    //GameObject crosshairImage;

    // Multiplayer
    PhotonView m_View;
    M_GameManager manager;

    bool isRunning;
    bool isAiming;
    bool isRootMotion;
    bool isSprinting;
    bool isRunningArmed;
    bool crouchActive;

    // cinemachine
    float cinemachineTargetYaw;
    float cinemachineTargetPitch;

    float muzzleTimer = 0;
    float shootTimer = Mathf.Infinity;

    float velocityChange;
    float turnSmoothVel;
    float movementSpeed = 0;
    float setWeight = 0;
    float sprintMultiplicator;

    public bool escapePressed;

    string newName = "not Used";
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        health = GetComponent<PlayerHealth>();
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        m_View = GetComponent<PhotonView>();
        weaponInfo = GetComponent<HandleWeapon>();
        newWeapon = defaultWeapon;
        manager = FindObjectOfType<M_GameManager>();
        name = PhotonNetwork.NickName;

        tabParent = manager.ReturnTab();
    }

    private void Start()
    {
        if (!m_View.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(followCam.gameObject);
            Destroy(aimCam.gameObject);
            Destroy(deathCam.gameObject);
            Destroy(GetComponent<Rigidbody>());
            Destroy(ui.gameObject);
        }
        escapeUI.SetActive(false);
        sensSlider.value = 30;
        mouseSensitivity = sensSlider.value;
        LockCameraPosition = false;
        mouseWorldPos = Vector3.zero;
        Cursor.lockState = CursorLockMode.Locked;
        EquipWeapon();
    }

    void Update()
    {
        // if (!m_View.IsMine) return;
        if (manager.gameEnd)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            LockCameraPosition = true;
            anim.SetBool("gunIdle", true);
            return;
        }


        if (m_View.IsMine)
        {
            if (health.GetDeath())
            {
                cineBrain.m_DefaultBlend.m_Time = 1.6f;
                deathCam.SetActive(true);
                return;
            }

            if(escapePressed)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                LockCameraPosition = true;
            }
            else
            {
                ResumeGame();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                LockCameraPosition = false;
            }

            string minutes = (manager.GameTime / 60).ToString("00");
            string seconds = (manager.GameTime % 60).ToString("00");


            timeValueText.text = $"{minutes}:{seconds}";
            timeRedValue.fillAmount = manager.GameTime / manager.MaxGameTime;
            maxScoreValueText.text = manager.maxPoints.ToString("F0");

            isAiming = Input.GetMouseButton(1);

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //m_View.RPC("EquipWeapon", RpcTarget.All, weaponList[Random.Range(0, weaponList.Length)], m_View.ViewID);
                // newWeapon = weaponList[Random.Range(0, weaponList.Length)];
                EquipWeapon();
            }

            UpdateAnim();
            RPC_HandleRaycast();
            HandleAiming();

            if (Input.GetKeyDown(KeyCode.R) && !weaponInfo.GetIfReloading())
            {
                weaponInfo.Reload();
                //currentWeapon.PlayReloadAudio(transform.position, player);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (escapePressed)
                {
                    ResumeGame();
                }
                else
                {
                    escapePressed = true;
                    escapeUI.SetActive(true);
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftControl) && !crouchActive) crouchActive = true;
            else if (Input.GetKeyDown(KeyCode.LeftControl) && crouchActive) crouchActive = false;

            if (isSprinting && !crouchActive)
            {
                footSteps.ChangeMaxDistance(15f);
                footSteps.ChangeVolume(0.15f);
            }
            else if (crouchActive)
            {
                footSteps.ChangeMaxDistance(5f);
                footSteps.ChangeVolume(0.05f);
            }
            else
            {
                footSteps.ChangeMaxDistance(10f);
                footSteps.ChangeVolume(0.1f);
            }

            if (crouchActive) setWeight = Mathf.Lerp(setWeight, 1, Time.deltaTime * 5f);
            else setWeight = Mathf.Lerp(setWeight, 0, Time.deltaTime * 5f);

            if (isRootMotion) return;
            HandleMovement();

            scoreValueText.text = manager.GetCurrentScore().ToString("F0");
        }

    }

    public void ResumeGame()
    {
        escapePressed = false;
        escapeUI.SetActive(false);
    }

    public void RestartGame()
    {
        manager.RestartGame();
    }

    public void LeaveGame()
    {
        manager.LeaveGame();
    }

    private void LateUpdate()
    {
        CameraRotation();
        isRootMotion = anim.GetBool("isRootMotion");
        anim.applyRootMotion = isRootMotion;
    }
    #endregion


    void EquipWeapon()
    {
        currentWeapon = defaultWeapon;
        currentWeapon.AttachWeapon(anim, handTransform);
    }

    public void ChangeValue()
    {
        mouseSensitivity = sensSlider.value;
    }


    #region Movement
    void HandleMovement()
    {
        Vector3 moveDir;
        Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        if (dir.magnitude >= 0.1f)
        {
            isRunning = true;

            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, rotationAcceleration);

            transform.rotation = Quaternion.Euler(0, angle, 0);
            moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            controller.Move(moveDir * movementSpeed * Time.deltaTime);
        }
        else isRunning = false;

        velocity.y = Mathf.Max(velocity.y - Time.deltaTime * gravityAcceleration, controller.isGrounded ? -1f : -gravity);

        if (isSprinting) sprintMultiplicator = maxSprintMultiplicator;
        else sprintMultiplicator = 1f;

        controller.Move(velocity * Time.deltaTime);
    }
    #endregion

    #region Aim
    void HandleAiming()
    {
        shootTimer += Time.deltaTime;
        muzzleTimer += Time.deltaTime;

        isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (!isAiming)
        {
            aimCam.SetActive(false);
            crosshair.GetComponent<Image>().enabled = false;

            if (crouchActive)
            {
                if (movementSpeed >= maxMovementSpeed - 3.75f) movementSpeed = maxMovementSpeed - 3.75f;

                movementSpeed = isRunning ? Mathf.Lerp(movementSpeed, crouchActive ? maxMovementSpeed - 2.75f : maxMovementSpeed, movementAcceleration * Time.deltaTime)
                                  : Mathf.Lerp(movementSpeed, 0, movementAcceleration * Time.deltaTime * 10);
            }
            else
            {
                movementSpeed = isRunning ? Mathf.Lerp(movementSpeed, isSprinting && !crouchActive ? maxMovementSpeed + 2f : maxMovementSpeed, movementAcceleration * Time.deltaTime)
                                  : Mathf.Lerp(movementSpeed, 0, movementAcceleration * Time.deltaTime * 10);
            }
        }
        else
        {
            aimCam.SetActive(true);
            crosshair.GetComponent<Image>().enabled = true;

            if (crouchActive)
            {
                if (movementSpeed > aimMovementSpeed - 1f) movementSpeed = aimMovementSpeed - 1f;

                movementSpeed = isRunning && isAiming ? Mathf.Lerp(movementSpeed, aimMovementSpeed - 1f, movementAcceleration * Time.deltaTime)
                                      : Mathf.Lerp(movementSpeed, 0, movementAcceleration * Time.deltaTime * 10);
            }
            else
            {
                if (movementSpeed > aimMovementSpeed) movementSpeed = aimMovementSpeed;

                movementSpeed = isRunning && isAiming ? Mathf.Lerp(movementSpeed, aimMovementSpeed, movementAcceleration * Time.deltaTime)
                                      : Mathf.Lerp(movementSpeed, 0, movementAcceleration * Time.deltaTime * 10);
            }

            Vector3 worldAimTarget = mouseWorldPos;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDir = (worldAimTarget - transform.position).normalized;

            bool fireMode = currentWeapon.GetBurstMode() ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

            if(Input.GetMouseButtonDown(0) && shootTimer >= (currentWeapon.GetTimerBtwBullets() / 10) && weaponInfo.GetCurrentAmmo() > 0)
            {
                m_View.RPC("NoAmmoSFX", RpcTarget.All, m_View.ViewID);
            }

            transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 200f);

            if (!weaponInfo.GetIfReloading() && fireMode && isAiming && shootTimer >= (currentWeapon.GetTimerBtwBullets() / 10))
            {
                // TODO: Shot Projectile
                if (Physics.Raycast(ray, out RaycastHit hit, 999f, aimColliderMask))
                {
                    GameObject bullet = PhotonNetwork.Instantiate(Path.Combine("prefabs", "Bullet"), currentWeapon.GetBulletSpawnPos().position, Quaternion.identity, 0);
                    bullet.TryGetComponent(out BulletProjectileRaycast b);
                    b.ChangeSpeed(bulletSpeed);
                    b.SetTarget(hit.point);
                    weaponInfo.Shoot();

                    if (weaponInfo.GetCurrentAmmo() <= 0)
                    {
                        m_View.RPC("NoAmmoSFX", RpcTarget.All, m_View.ViewID);
                        return;
                    }

                    //m_View.RPC("RPC_SpawnMuzzleFlash", RpcTarget.All, m_View.ViewID);
                    m_View.RPC("ShootSFX", RpcTarget.All, m_View.ViewID);

                    hit.collider.gameObject.TryGetComponent<IDamagable>(out IDamagable damage);
                    if (damage != null)
                    {
                        damage.Damage(currentWeapon.GetWeaponDamage());
                        manager.AddNewScore();
                        manager.AddNewKill();
                    }

                    shootTimer = 0;

                    aimCam.GetComponent<CamShake>().ShakeCam(shakeStrenght, shakeTimer);
                }
            }
        }
    }
    #endregion



    #region Camera
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (Input.mousePosition.sqrMagnitude >= 0.1f && !LockCameraPosition)
        {
            cinemachineTargetYaw += Input.GetAxisRaw("Mouse X") * (mouseSensitivity * 10) * Time.deltaTime;
            cinemachineTargetPitch += -Input.GetAxisRaw("Mouse Y") * (mouseSensitivity * 10) * Time.deltaTime;
        }

        // clamp our rotations so our values are limited 360 degrees
        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride, cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    #endregion

    #region AudioRPC
    [PunRPC]
    public void ShootSFX(int id)
    {
        if (m_View.ViewID == id)
        {
            currentWeapon.PlayShootAudio(transform.position);
        }
    }

    [PunRPC]
    public void NoAmmoSFX(int id)
    {
        if (m_View.ViewID == id)
        {
            currentWeapon.PlayNoAmmoAudio(transform.position);
        }
    }

    [PunRPC]
    void FootSFX(int id)
    {
        if (m_View.ViewID == id) footSteps.Play(transform.position);
    }

    public void FootSteps()
    {
        m_View.RPC("FootSFX", RpcTarget.All, m_View.ViewID);
    }
    #endregion

    [PunRPC]
    GameObject RPC_SpawnBulletProjectile(int id)
    {
        if (m_View.ViewID == id)
        {
            GameObject bullet = Instantiate(bulletPrefab, currentWeapon.GetBulletSpawnPos().position, Quaternion.identity) as GameObject;
            return bullet;
        }
        return null;
    }

    [PunRPC]
    void RPC_SpawnMuzzleFlash(int id)
    {
        if (m_View.ViewID == id)
        {
            //GameObject muzzleFlash = PhotonNetwork.Instantiate(Path.Combine("prefabs", "Muzzle"), currentWeapon.GetBulletSpawnPos().position, currentWeapon.GetBulletSpawnPos().rotation) as GameObject;
            GameObject muzzleFlash = Instantiate(currentWeapon.GetMuzzlePrefab(), currentWeapon.GetBulletSpawnPos().position, currentWeapon.GetBulletSpawnPos().rotation) as GameObject;
            Destroy(muzzleFlash, .3f);
        }
    }


    [PunRPC]
    private void RPC_HandleRaycast()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderMask))
        {
            //debugTransform.position = raycastHit.point;
            mouseWorldPos = raycastHit.point;
        }
    }

    void UpdateAnim()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        anim.SetFloat("movementX", h);
        anim.SetFloat("movementY", v);
        anim.SetFloat("Movement", velocityChange);
        anim.SetFloat("speedValue", sprintMultiplicator);
        anim.SetFloat("crouchSpeed", maxCrouchMultiplicator);

        anim.SetBool("isRunningUnarmed", isRunning);
        anim.SetBool("inSprint", isSprinting);
        anim.SetBool("gunIdle", isAiming);
        anim.SetBool("crouchIdle", crouchActive);
    }

    public WeaponScriptObject GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public string GetUserName()
    {
        newName = PhotonNetwork.NickName;
        return newName;
    }
}
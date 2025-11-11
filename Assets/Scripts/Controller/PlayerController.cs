using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 스피드 조정 변수
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float runSpeed;

    [SerializeField]
    private float crouchSpeed;

    private float baseSpeed;
    private float currentSpeed; // 실제 사용되는 속도
    private float applySpeed
    {
        get { return currentSpeed; }
        set { 
            baseSpeed = value;
            UpdateCurrentSpeed();
        }
    }

    private float calSpeed;

    [SerializeField]
    private float jumpForce;


    // 상태 변수
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;


    // 앉았을 때 얼마나 앉을지 결정하는 변수.
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    // 땅 착지 여부
    private BoxCollider boxCollider;


    // 민감도
    [SerializeField]
    private float lookSensitivity = 2f;


    // 카메라 한계
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0;
    private const string PREF_KEY_SENS = "LookSensitivity";


    //필요한 컴포넌트
    [SerializeField]
    private Camera theCamera;

    private Rigidbody myRigid;

    // 🔹 인터랙션용 필드 추가
    [Header("Interaction Settings")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactMask = ~0; // 모든 레이어 기본
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    // 초기화
    void Start()
    {
        // Player 태그 자동 설정
        if (!gameObject.CompareTag("Player"))
        {
            gameObject.tag = "Player";
        }
        
        boxCollider = GetComponent<BoxCollider>();
        myRigid = GetComponent<Rigidbody>();
        baseSpeed = walkSpeed;
        UpdateCurrentSpeed();

        // 초기화.
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;

        if (PlayerPrefs.HasKey(PREF_KEY_SENS))
            lookSensitivity = PlayerPrefs.GetFloat(PREF_KEY_SENS);
    }
    
    /// <summary>
    /// 현재 속도를 업데이트합니다 (speedLevel과 버프 적용)
    /// </summary>
    private void UpdateCurrentSpeed()
    {
        if (isRun)
        {
            // 달리기 중: 버프 적용된 runSpeed 사용
            float speedMultiplier = PlayerManager.Instance.GetRunSpeedMultiplier();
            currentSpeed = runSpeed * speedMultiplier * PlayerManager.Instance.speedLevel.current;
        }
        else
        {
            // 걷기/앉기: 기본 속도만 사용
            currentSpeed = baseSpeed * PlayerManager.Instance.speedLevel.current;
        }
    }


    void Update()
    {
        IsGround();
        TryJump();
        TryRun();
        TryCrouch();
        Move();

        if (Cursor.lockState == CursorLockMode.None) return;

        CameraRotation();
        CharacterRotation();

        // 🔹 여기 추가
        TryInteract();
    }


    public float LookSensitivity
    {
        get => lookSensitivity;
        set
        {
            lookSensitivity = Mathf.Clamp(value, 0.1f, 20f);
            PlayerPrefs.SetFloat(PREF_KEY_SENS, lookSensitivity);
        }
    }


    // 🔸 문, 인터랙션 처리 함수
    private void TryInteract()
    {
        // 카메라 중앙에서 레이 쏘기
        Ray ray = theCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask, QueryTriggerInteraction.Collide))
        {
            // BusSystem (버스) - 런타임에 타입으로 찾기 (컴파일 오류 방지)
            Component busComponent = null;
            // 먼저 직접 컴포넌트 찾기 시도
            var allComponents = hit.collider.GetComponents<Component>();
            foreach (var comp in allComponents)
            {
                if (comp != null && comp.GetType().Name == "BusSystem")
                {
                    busComponent = comp;
                    break;
                }
            }
            // 찾지 못했으면 부모에서 찾기
            if (busComponent == null)
            {
                var parentComponents = hit.collider.GetComponentsInParent<Component>();
                foreach (var comp in parentComponents)
                {
                    if (comp != null && comp.GetType().Name == "BusSystem")
                    {
                        busComponent = comp;
                        break;
                    }
                }
            }
            
            // BusSystem을 찾았고 E키를 눌렀다면 Interact 호출
            if (busComponent != null && Input.GetKeyDown(interactKey))
            {
                var interactMethod = busComponent.GetType().GetMethod("Interact");
                if (interactMethod != null)
                {
                    interactMethod.Invoke(busComponent, null);
                    return;
                }
            }

            // DoorInteract
            var door = hit.collider.GetComponentInParent<DoorInteract>();
            if (door != null && Input.GetKeyDown(interactKey))
            {
                door.Interact();
                return;
            }

            // SingleDoorInteract
            var singleDoor = hit.collider.GetComponentInParent<SingleDoorInteract>();
            if (singleDoor != null && Input.GetKeyDown(interactKey))
            {
                singleDoor.Interact();
                return;
            }
        }
    }


    // 앉기 시도
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }


    // 앉기 동작
    private void Crouch()
    {
        isCrouch = !isCrouch;

        if (isCrouch)
        {
            baseSpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            baseSpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }
        
        UpdateCurrentSpeed();

        StartCoroutine(CrouchCoroutine());
    }

    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while (_posY != applyCrouchPosY)
        {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if (count > 15)
                break;
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
    }


    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, boxCollider.bounds.extents.y + 0.1f);
    }


    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (isCrouch)
            Crouch();

        myRigid.linearVelocity = transform.up * jumpForce;
    }


    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && (PlayerManager.Instance.staminaLevel.current > 0))
        {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }
    }

    private void Running()
    {
        if (isRun) return;
        if (isCrouch)
            Crouch();

        isRun = true;
        UpdateCurrentSpeed();
        PlayerManager.Instance.StartStaminaLoss();
    }
    
    /// <summary>
    /// 현재 달리는 중인지 확인합니다
    /// </summary>
    public bool IsRunning()
    {
        return isRun;
    }

    public void RunningCancel()
    {
        if (!isRun) return;
        isRun = false;
        baseSpeed = walkSpeed;
        UpdateCurrentSpeed();
        PlayerManager.Instance.StopStaminaLoss();
    }


    private void Move()
    {
        // 달리는 중이면 버프 적용된 속도로 업데이트
        UpdateCurrentSpeed();
        
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }


    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }


    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    private void OnSensitivityChanged(float value)
    {
        LookSensitivityValue = value; // 아래 프로퍼티에서 클램프 + PlayerPrefs 반영
    }

    public float LookSensitivityValue
    {
        get => lookSensitivity;
        set
        {
            // 즉시 적용 + 범위 보호
            lookSensitivity = Mathf.Clamp(value, 0.1f, 20f);
            PlayerPrefs.SetFloat(PREF_KEY_SENS, lookSensitivity); // 씬 간 공유 저장
        }
    }
    private void OnEnable()
    {
        // 시작 시 한 번 더 안전하게 적용(씬 이동 등으로 복귀했을 때 대비)
        LookSensitivityValue = PlayerPrefs.GetFloat(PREF_KEY_SENS, LookSensitivity);

        SettingsEvents.OnLookSensitivityChanged += OnSensitivityChanged;
    }

    private void OnDisable()
    {
        SettingsEvents.OnLookSensitivityChanged -= OnSensitivityChanged;
    }
}

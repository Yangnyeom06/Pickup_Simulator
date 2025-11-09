using System.Collections;
using System.Collections.Generic;
<<<<<<< HEAD
=======
using System;
using System.Reflection;
>>>>>>> 74b4bcf0 (update)
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
<<<<<<< HEAD
    private float applySpeed
    {
        get { return baseSpeed * PlayerManager.Instance.speedLevel.current; }
        set { baseSpeed = value; }
=======
    private float currentSpeed; // 실제 사용되는 속도
    private float applySpeed
    {
        get { return currentSpeed; }
        set { 
            baseSpeed = value;
            UpdateCurrentSpeed();
        }
>>>>>>> 74b4bcf0 (update)
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

<<<<<<< HEAD

    // Use this for initialization
=======
    // 🔹 인터랙션용 필드 추가
    [Header("Interaction Settings")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactMask = ~0; // 모든 레이어 기본
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    // 초기화
>>>>>>> 74b4bcf0 (update)
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        myRigid = GetComponent<Rigidbody>();
<<<<<<< HEAD
        applySpeed = walkSpeed;
=======
        baseSpeed = walkSpeed;
        UpdateCurrentSpeed();
>>>>>>> 74b4bcf0 (update)

        // 초기화.
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;

        if (PlayerPrefs.HasKey(PREF_KEY_SENS))
            lookSensitivity = PlayerPrefs.GetFloat(PREF_KEY_SENS);
<<<<<<< HEAD

    }




    // Update is called once per frame
    void Update()
    {

=======
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
>>>>>>> 74b4bcf0 (update)
        IsGround();
        TryJump();
        TryRun();
        TryCrouch();
        Move();

        if (Cursor.lockState == CursorLockMode.None) return;

        CameraRotation();
        CharacterRotation();

<<<<<<< HEAD


    }

=======
        // 🔹 여기 추가
        TryInteract();
    }


>>>>>>> 74b4bcf0 (update)
    public float LookSensitivity
    {
        get => lookSensitivity;
        set
        {
            lookSensitivity = Mathf.Clamp(value, 0.1f, 20f);
<<<<<<< HEAD
            PlayerPrefs.SetFloat(PREF_KEY_SENS, lookSensitivity); // 씬 간 공유
=======
            PlayerPrefs.SetFloat(PREF_KEY_SENS, lookSensitivity);
>>>>>>> 74b4bcf0 (update)
        }
    }


<<<<<<< HEAD
=======
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

>>>>>>> 74b4bcf0 (update)

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
<<<<<<< HEAD
            applySpeed = crouchSpeed;
=======
            baseSpeed = crouchSpeed;
>>>>>>> 74b4bcf0 (update)
            applyCrouchPosY = crouchPosY;
        }
        else
        {
<<<<<<< HEAD
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        StartCoroutine(CrouchCoroutine());

    }

    // 부드러운 동작 실행.
    IEnumerator CrouchCoroutine()
    {

=======
            baseSpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }
        
        UpdateCurrentSpeed();

        StartCoroutine(CrouchCoroutine());
    }

    IEnumerator CrouchCoroutine()
    {
>>>>>>> 74b4bcf0 (update)
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


<<<<<<< HEAD
    // 지면 체크.
=======
>>>>>>> 74b4bcf0 (update)
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, boxCollider.bounds.extents.y + 0.1f);
    }


<<<<<<< HEAD
    // 점프 시도
=======
>>>>>>> 74b4bcf0 (update)
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

<<<<<<< HEAD

    // 점프
    private void Jump()
    {

        // 앉은 상태에서 점프시 앉은 상태 해제.
=======
    private void Jump()
    {
>>>>>>> 74b4bcf0 (update)
        if (isCrouch)
            Crouch();

        myRigid.linearVelocity = transform.up * jumpForce;
    }


<<<<<<< HEAD
    // 달리기 시도
=======
>>>>>>> 74b4bcf0 (update)
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

<<<<<<< HEAD
    // 달리기 실행
    private void Running()
    {
        if (isRun) return; // 달리고 있으면 리턴
=======
    private void Running()
    {
        if (isRun) return;
>>>>>>> 74b4bcf0 (update)
        if (isCrouch)
            Crouch();

        isRun = true;
<<<<<<< HEAD
        applySpeed = runSpeed;
        PlayerManager.Instance.StartStaminaLoss();
        Debug.Log("달리는 중");
    }


    // 달리기 취소
    public void RunningCancel()
    {
        if (!isRun) return; // 안달리고 있으면 리턴

        isRun = false;
        applySpeed = walkSpeed;
        PlayerManager.Instance.StopStaminaLoss();
        Debug.Log("안달리는 중");
    }


    // 움직임 실행
    private void Move()
    {

=======
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
        
>>>>>>> 74b4bcf0 (update)
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

<<<<<<< HEAD
    // 좌우 캐릭터 회전
    private void CharacterRotation()
    {

=======

    private void CharacterRotation()
    {
>>>>>>> 74b4bcf0 (update)
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }


<<<<<<< HEAD

    // 상하 카메라 회전
=======
>>>>>>> 74b4bcf0 (update)
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

<<<<<<< HEAD
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
     private void OnSensitivityChanged(float value)
=======
    private void OnSensitivityChanged(float value)
>>>>>>> 74b4bcf0 (update)
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
<<<<<<< HEAD


=======
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
>>>>>>> 74b4bcf0 (update)
}

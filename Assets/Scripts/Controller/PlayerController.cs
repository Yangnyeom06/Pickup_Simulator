using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public PlayerManager player;
    // 스피드 조정 변수
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float runSpeed;

    [SerializeField]
    private float crouchSpeed;

    private float baseSpeed;
    private float applySpeed
    {
        get { return baseSpeed * player.speedLevel.current; }
        set { baseSpeed = value; }
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


    // Use this for initialization
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        myRigid = GetComponent<Rigidbody>();
        applySpeed = walkSpeed;

        // 초기화.
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
        
         if (PlayerPrefs.HasKey(PREF_KEY_SENS))
            lookSensitivity = PlayerPrefs.GetFloat(PREF_KEY_SENS);

    }




    // Update is called once per frame
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
        
    

	}

    public float LookSensitivity
    {
        get => lookSensitivity;
        set
        {
            lookSensitivity = Mathf.Clamp(value, 0.1f, 20f);
            PlayerPrefs.SetFloat(PREF_KEY_SENS, lookSensitivity); // 씬 간 공유
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
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        StartCoroutine(CrouchCoroutine());

    }

    // 부드러운 동작 실행.
    IEnumerator CrouchCoroutine()
    {

        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while(_posY != applyCrouchPosY)
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


    // 지면 체크.
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, boxCollider.bounds.extents.y + 0.1f);
    }


    // 점프 시도
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }


    // 점프
    private void Jump()
    {

        // 앉은 상태에서 점프시 앉은 상태 해제.
        if (isCrouch)
            Crouch();

        myRigid.linearVelocity = transform.up * jumpForce;
    }


    // 달리기 시도
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && (player.staminaLevel.current > 0))
        {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }
    }

    // 달리기 실행
    private void Running()
    {
        if (isRun) return; // 달리고 있으면 리턴
        if (isCrouch)
            Crouch();

        isRun = true;
        applySpeed = runSpeed;
        player.StartStaminaLoss();
        Debug.Log("달리는 중");
    }


    // 달리기 취소
    public void RunningCancel()
    {
        if (!isRun) return; // 안달리고 있으면 리턴

        isRun = false;
        applySpeed = walkSpeed;
        player.StopStaminaLoss();
        Debug.Log("안달리는 중");
    }


    // 움직임 실행
    private void Move()
    {

        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    // 좌우 캐릭터 회전
    private void CharacterRotation()
    {
       
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }



    // 상하 카메라 회전
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

}

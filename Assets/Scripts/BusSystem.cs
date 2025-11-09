<<<<<<< HEAD
using System.Collections;
=======
>>>>>>> origin/dev/newUI
using UnityEngine;
using UnityEngine.UI;
using TMPro;

<<<<<<< HEAD
public class BusSystem : MonoBehaviour
{
    [Header("버스 설정")]
    [SerializeField] private string busCardItemID = "BUS_CARD";

    [Header("UI 참조")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text messageText;

    [Header("시스템 참조")]
    [SerializeField] private FadeInOut fadeController;

    [Header("이동 설정")]
    [SerializeField] private Transform targetLocation;
    [SerializeField] private float travelDelay = 3f;
    [SerializeField] private Transform playerTransform;

    [Header("버스 연출")]
    [SerializeField] private Transform busObject;
    [SerializeField] private Transform busStopPoint;
    [SerializeField] private float busMoveSpeed = 5f;

    private bool isInteractable = true;

    private void Start()
    {
=======
/// <summary>
/// 버스 시스템: 버스 카드를 사용하여 시내로 이동할 수 있게 해주는 스크립트
/// 버스 오브젝트에 이 컴포넌트를 추가하고, Inspector에서 설정을 해주세요.
/// </summary>
public class BusSystem : MonoBehaviour
{
    [Header("버스 설정")]
    [SerializeField] private string busCardItemID = "BUS_CARD"; // 버스 카드 아이템 ID (StoreItemData의 itemID와 일치해야 함)
    [SerializeField] private string targetSceneName = "City"; // 이동할 씬 이름 (시내 씬)
    [SerializeField] private string targetSpawnPointName = "BusStationSpawnPoint"; // 도착 지점 스폰 포인트 이름
    
    [Header("UI 참조")]
    [SerializeField] private Button confirmButton; // 확인 버튼
    [SerializeField] private Button cancelButton; // 취소 버튼
    [SerializeField] private TMP_Text messageText; // 메시지 텍스트 (선택사항)
    
    [Header("시스템 참조")]
    [SerializeField] private FadeInOut fadeController; // 페이드 인/아웃 컨트롤러
    
    private bool isInteractable = true;
    
    private void OnDisable()
    {
        // Invoke 취소
        CancelInvoke();
    }
    
    private void Start()
    {
        // UI 초기화 - 버튼 숨김
>>>>>>> origin/dev/newUI
        if (confirmButton != null)
        {
            confirmButton.gameObject.SetActive(false);
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(OnConfirmTravel);
        }
<<<<<<< HEAD

=======
        
>>>>>>> origin/dev/newUI
        if (cancelButton != null)
        {
            cancelButton.gameObject.SetActive(false);
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(OnCancelTravel);
        }
<<<<<<< HEAD

=======
        
        // 메시지 텍스트 숨김 및 기본 설정
>>>>>>> origin/dev/newUI
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
            if (string.IsNullOrEmpty(messageText.text))
            {
<<<<<<< HEAD
                messageText.text = "Do you want to move?";
            }
        }

=======
                messageText.text = "이동하시겠습니까?";
            }
        }
        
        // FadeController 자동 찾기 (Inspector에서 할당되지 않은 경우)
>>>>>>> origin/dev/newUI
        if (fadeController == null)
        {
            fadeController = FindFirstObjectByType<FadeInOut>();
        }
<<<<<<< HEAD

        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                playerTransform = playerObj.transform;
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Interact();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideButtons();
        }
    }

    public void Interact()
    {
        if (!isInteractable) return;

        if (confirmButton == null || cancelButton == null)
        {
            Debug.LogError("확인/취소 버튼이 설정되지 않았습니다.");
            return;
        }

        ShowButtons();
    }

=======
    }
    
    /// <summary>
    /// 플레이어가 버스 앞에서 E키를 눌렀을 때 호출되는 메서드
    /// </summary>
    public void Interact()
    {
        if (!isInteractable) return;
        
        // 버튼이 설정되어 있는지 확인
        if (confirmButton == null || cancelButton == null)
        {
            Debug.LogError("BusSystem: 확인 버튼 또는 취소 버튼이 설정되지 않았습니다! Inspector에서 버튼을 할당해주세요.");
            return;
        }
        
        // 버튼 표시
        ShowButtons();
    }
    
    /// <summary>
    /// 인벤토리에 버스 카드가 있는지 확인
    /// </summary>
>>>>>>> origin/dev/newUI
    public bool HasBusCard()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager를 찾을 수 없습니다!");
            return false;
        }
<<<<<<< HEAD

        foreach (var slot in InventoryManager.Instance.buyItemSlotList)
        {
            if (slot == null) continue;

            if (slot.currentShopItem != null &&
=======
        
        // 상점 아이템 슬롯에서 버스 카드 찾기
        foreach (var slot in InventoryManager.Instance.buyItemSlotList)
        {
            if (slot == null) continue;
            
            // 상점 아이템 확인
            if (slot.currentShopItem != null && 
>>>>>>> origin/dev/newUI
                slot.currentShopItem.itemID == busCardItemID)
            {
                return true;
            }
        }
<<<<<<< HEAD

        return false;
    }

=======
        
        return false;
    }
    
    /// <summary>
    /// 버스 카드를 인벤토리에서 제거
    /// </summary>
>>>>>>> origin/dev/newUI
    public void RemoveBusCard()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager를 찾을 수 없습니다!");
            return;
        }
<<<<<<< HEAD

        foreach (var slot in InventoryManager.Instance.buyItemSlotList)
        {
            if (slot == null) continue;

            if (slot.currentShopItem != null &&
                slot.currentShopItem.itemID == busCardItemID)
            {
                slot.ClearSlot();

=======
        
        // 상점 아이템 슬롯에서 버스 카드 찾아서 제거
        foreach (var slot in InventoryManager.Instance.buyItemSlotList)
        {
            if (slot == null) continue;
            
            // 상점 아이템 확인
            if (slot.currentShopItem != null && 
                slot.currentShopItem.itemID == busCardItemID)
            {
                // 슬롯 내용 비우기
                slot.ClearSlot();
                
                // savedStoreItems에서도 제거 (itemID 기준)
>>>>>>> origin/dev/newUI
                if (InventoryManager.Instance.savedStoreItems != null)
                {
                    InventoryManager.Instance.savedStoreItems.RemoveAll(
                        s => s != null && s.itemID == busCardItemID
                    );
                }
<<<<<<< HEAD

=======
                
>>>>>>> origin/dev/newUI
                Debug.Log("버스 카드를 사용했습니다.");
                return;
            }
        }
    }
<<<<<<< HEAD

    public void ShowButtons()
    {
        if (confirmButton != null)
            confirmButton.gameObject.SetActive(true);

        if (cancelButton != null)
            cancelButton.gameObject.SetActive(true);

        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
            messageText.text = HasBusCard()
                ? "Would you like to call a bus?"
                : "No busCard";
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideButtons()
    {
        CancelInvoke(nameof(HideButtons));

        if (confirmButton != null)
            confirmButton.gameObject.SetActive(false);

        if (cancelButton != null)
            cancelButton.gameObject.SetActive(false);

        if (messageText != null)
            messageText.gameObject.SetActive(false);

=======
    
    /// <summary>
    /// 버튼 표시
    /// </summary>
    public void ShowButtons()
    {
        // 확인 버튼 표시
        if (confirmButton != null)
        {
            confirmButton.gameObject.SetActive(true);
        }
        
        // 취소 버튼 표시
        if (cancelButton != null)
        {
            cancelButton.gameObject.SetActive(true);
        }
        
        // 메시지 텍스트 표시 및 초기화 (버스카드 보유 여부에 따라 메시지 변경)
        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
            if (HasBusCard())
            {
                messageText.text = "이동하시겠습니까?";
            }
            else
            {
                messageText.text = "버스 카드가 필요합니다!";
            }
        }
        
        // 커서 잠금 해제 (UI 조작을 위해)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    /// <summary>
    /// 버튼 숨기기
    /// </summary>
    public void HideButtons()
    {
        // Invoke로 예약된 자동 닫기가 있다면 취소
        CancelInvoke(nameof(HideButtons));
        
        // 확인 버튼 숨김
        if (confirmButton != null)
        {
            confirmButton.gameObject.SetActive(false);
        }
        
        // 취소 버튼 숨김
        if (cancelButton != null)
        {
            cancelButton.gameObject.SetActive(false);
        }
        
        // 메시지 텍스트 숨김
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
        
        // 취소나 이동하지 않는 경우에만 커서 다시 잠금
        // (이동하는 경우 씬이 바뀌므로 커서 상태는 신경 쓸 필요 없음)
>>>>>>> origin/dev/newUI
        if (isInteractable)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
<<<<<<< HEAD

    public void OnConfirmTravel()
    {
        if (!HasBusCard())
        {
            if (messageText != null)
                messageText.text = "You need bus card.";
            Invoke(nameof(HideButtons), 2f);
            return;
        }

        isInteractable = false;
        HideButtons();

        RemoveBusCard();

        StartCoroutine(MoveBusThenTeleport());
    }

=======
    
    /// <summary>
    /// 확인 버튼 클릭 시 호출
    /// </summary>
    public void OnConfirmTravel()
    {
        // 버스 카드 보유 여부 확인
        if (!HasBusCard())
        {
            Debug.LogWarning("버스 카드를 찾을 수 없습니다!");
            // 버스카드가 없으면 메시지 표시하고 버튼은 유지
            if (messageText != null)
            {
                messageText.text = "버스 카드가 필요합니다!";
            }
            // 2초 후 자동으로 버튼 닫기
            Invoke(nameof(HideButtons), 2f);
            return;
        }
        
        // 버스 카드가 있는 경우에만 순간이동 진행
        isInteractable = false;
        HideButtons();
        
        // 버스 카드 사용 (제거)
        RemoveBusCard();
        
        // 씬 이동
        TravelToCity();
    }
    
    /// <summary>
    /// 취소 버튼 클릭 시 호출
    /// </summary>
>>>>>>> origin/dev/newUI
    public void OnCancelTravel()
    {
        HideButtons();
    }
<<<<<<< HEAD

    private IEnumerator MoveBusThenTeleport()
    {
        if (busObject == null || busStopPoint == null)
        {
            Debug.LogWarning("버스 오브젝트나 도착 지점이 설정되지 않았습니다.");
            yield break;
        }

        // ❗ 버스 오브젝트를 처음에 활성화
        busObject.gameObject.SetActive(true);

        // 버스 위치를 초기 위치로 설정하고 싶다면 여기에 위치 리셋도 가능
        // busObject.position = startPoint.position;

        // 버스가 도착할 때까지 이동
        while (Vector3.Distance(busObject.position, busStopPoint.position) > 0.1f)
        {
            busObject.position = Vector3.MoveTowards(
                busObject.position,
                busStopPoint.position,
                busMoveSpeed * Time.deltaTime
            );

            Quaternion targetRot = Quaternion.LookRotation(busStopPoint.position - busObject.position);
            busObject.rotation = Quaternion.Slerp(busObject.rotation, targetRot, Time.deltaTime * 2f);

            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        TravelToCity();
    }


    public void TravelToCity()
    {
        if (targetLocation == null || playerTransform == null)
        {
            Debug.LogError("이동 대상 또는 플레이어 참조가 비어 있습니다.");
            isInteractable = true;
            return;
        }

=======
    
    /// <summary>
    /// 시내로 이동
    /// </summary>
    public void TravelToCity()
    {
        if (SceneChangeManager.Instance == null)
        {
            Debug.LogError("SceneChangeManager를 찾을 수 없습니다!");
            isInteractable = true; // 복구
            return;
        }
        
        // FadeController가 있으면 페이드 효과 사용
>>>>>>> origin/dev/newUI
        if (fadeController != null)
        {
            fadeController.StartFadeInAndOut(() =>
            {
<<<<<<< HEAD
                Invoke(nameof(TeleportPlayer), travelDelay);
=======
                SceneChangeManager.Instance.LoadPlayScene(
                    targetSceneName, 
                    SceneChangeManager.Instance.selectSlotId, 
                    targetSpawnPointName
                );
>>>>>>> origin/dev/newUI
            });
        }
        else
        {
<<<<<<< HEAD
            Invoke(nameof(TeleportPlayer), travelDelay);
        }
    }

    private void TeleportPlayer()
    {
        playerTransform.position = targetLocation.position;

        // y축만 회전 적용
        Vector3 currentEuler = playerTransform.eulerAngles;
        Vector3 targetEuler = targetLocation.eulerAngles;
        playerTransform.rotation = Quaternion.Euler(currentEuler.x, targetEuler.y, currentEuler.z);

        isInteractable = true;
=======
            // 페이드 효과 없이 바로 이동
            SceneChangeManager.Instance.LoadPlayScene(
                targetSceneName, 
                SceneChangeManager.Instance.selectSlotId, 
                targetSpawnPointName
            );
        }
    }
    
    /// <summary>
    /// Inspector에서 씬 이름과 스폰 포인트를 설정할 수 있도록 헬퍼 메서드
    /// </summary>
    public void SetTravelDestination(string sceneName, string spawnPointName)
    {
        targetSceneName = sceneName;
        targetSpawnPointName = spawnPointName;
>>>>>>> origin/dev/newUI
    }
}

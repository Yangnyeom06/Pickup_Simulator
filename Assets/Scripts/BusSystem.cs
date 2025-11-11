using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        if (confirmButton != null)
        {
            confirmButton.gameObject.SetActive(false);
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(OnConfirmTravel);
        }

        if (cancelButton != null)
        {
            cancelButton.gameObject.SetActive(false);
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(OnCancelTravel);
        }

        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
            if (string.IsNullOrEmpty(messageText.text))
            {
                messageText.text = "Do you want to move?";
            }
        }

        if (fadeController == null)
        {
            fadeController = FindFirstObjectByType<FadeInOut>();
        }

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

    public bool HasBusCard()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager를 찾을 수 없습니다!");
            return false;
        }

        foreach (var slot in InventoryManager.Instance.buyItemSlotList)
        {
            if (slot == null) continue;

            if (slot.currentShopItem != null &&
                slot.currentShopItem.itemID == busCardItemID)
            {
                return true;
            }
        }

        return false;
    }

    public void RemoveBusCard()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager를 찾을 수 없습니다!");
            return;
        }

        foreach (var slot in InventoryManager.Instance.buyItemSlotList)
        {
            if (slot == null) continue;

            if (slot.currentShopItem != null &&
                slot.currentShopItem.itemID == busCardItemID)
            {
                slot.ClearSlot();

                if (InventoryManager.Instance.savedStoreItems != null)
                {
                    InventoryManager.Instance.savedStoreItems.RemoveAll(
                        s => s != null && s.itemID == busCardItemID
                    );
                }

                Debug.Log("버스 카드를 사용했습니다.");
                return;
            }
        }
    }

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

        if (isInteractable)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

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

        // FadeInOut 싱글톤에서 코루틴 실행 (항상 활성화되어 있음)
        FadeInOut fadeInstance = fadeController ?? FadeInOut.Instance;
        if (fadeInstance != null && fadeInstance.gameObject.activeInHierarchy)
        {
            fadeInstance.StartCoroutine(MoveBusThenTeleport());
            return;
        }

        // 플레이어 GameObject에서 코루틴 실행
        if (playerTransform != null && playerTransform.gameObject.activeInHierarchy)
        {
            MonoBehaviour playerMono = playerTransform.GetComponent<MonoBehaviour>();
            if (playerMono != null)
            {
                playerMono.StartCoroutine(MoveBusThenTeleport());
                return;
            }
        }

        // 이 GameObject가 활성화되어 있으면 여기서 실행
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(MoveBusThenTeleport());
        }
        else
        {
            Debug.LogError("코루틴을 실행할 수 있는 활성화된 GameObject를 찾을 수 없습니다!");
            isInteractable = true;
        }
    }

    public void OnCancelTravel()
    {
        HideButtons();
    }

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

        if (fadeController != null)
        {
            fadeController.StartFadeInAndOut(() =>
            {
                Invoke(nameof(TeleportPlayer), travelDelay);
            });
        }
        else
        {
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
    }
}

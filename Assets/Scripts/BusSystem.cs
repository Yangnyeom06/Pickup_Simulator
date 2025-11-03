using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BusSystem : MonoBehaviour
{
    [Header("버스 설정")]
    [SerializeField] private string busCardItemID = "BUS_CARD";

    [Header("UI 참조")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text messageText;

    [Header("버스 연출")]
    [SerializeField] private Transform busObject;
    [SerializeField] private Transform busArrivalPoint;

    [Header("시스템 참조")]
    [SerializeField] private FadeInOut fadeController;
    [SerializeField] private AreaPortalWithFade portal;

    private bool isInteractable = true;

    private void OnDisable() => CancelInvoke();

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
                messageText.text = "이동하시겠습니까?";
            }
        }

        if (fadeController == null)
        {
            fadeController = FindFirstObjectByType<FadeInOut>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Interact();
    }

    public void Interact()
    {
        if (!isInteractable) return;

        if (confirmButton == null || cancelButton == null)
        {
            Debug.LogError("버튼이 설정되지 않았습니다!");
            return;
        }

        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);

            if (HasBusCard())
            {
                messageText.text = "버스를 호출하시겠습니까?";
                confirmButton.interactable = true;
            }
            else
            {
                messageText.text = "버스 카드가 필요합니다!";
                confirmButton.interactable = false;
            }
        }

        confirmButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public bool HasBusCard()
    {
        if (InventoryManager.Instance == null) return false;

        foreach (var slot in InventoryManager.Instance.buyItemSlotList)
        {
            if (slot == null) continue;
            if (slot.currentShopItem != null && slot.currentShopItem.itemID == busCardItemID)
                return true;
        }

        return false;
    }

    public void RemoveBusCard()
    {
        if (InventoryManager.Instance == null) return;

        foreach (var slot in InventoryManager.Instance.buyItemSlotList)
        {
            if (slot == null) continue;

            if (slot.currentShopItem != null && slot.currentShopItem.itemID == busCardItemID)
            {
                slot.ClearSlot();
                InventoryManager.Instance.savedStoreItems?.RemoveAll(
                    s => s != null && s.itemID == busCardItemID);
                return;
            }
        }
    }

    public void HideButtons()
    {
        CancelInvoke(nameof(HideButtons));

        confirmButton?.gameObject.SetActive(false);
        cancelButton?.gameObject.SetActive(false);
        messageText?.gameObject.SetActive(false);

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
            messageText.text = "버스 카드가 필요합니다!";
            Invoke(nameof(HideButtons), 2f);
            return;
        }

        isInteractable = false;
        HideButtons();
        RemoveBusCard();
        StartCoroutine(MoveBusAndTeleport());
    }

    private IEnumerator MoveBusAndTeleport()
    {
        if (busObject == null || busArrivalPoint == null || portal == null) yield break;

        float t = 0f;
        Vector3 start = busObject.position;
        Vector3 end = busArrivalPoint.position;

        while (t < 1f)
        {
            t += Time.deltaTime;
            busObject.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        portal.DoTeleport(player);
    }

    public void OnCancelTravel() => HideButtons();
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class StoreNPC : MonoBehaviour
{
    [Header("레이캐스트용 카메라와 최대 거리")]
    public Camera mainCamera;
    public float rayDistance = 100f;

    [Header("Inspector 에서 드래그해서 지정할 클릭 대상들")]
    public List<Transform> clickableTargets;

    [Header("씬에 미리 배치된 버튼들")]
    public Button buyButton;
    public Button sellButton;
    public Button cancelButton;

    [Header("연동할 시스템들")]
    public BuySystem buySystem;
    public SaleSystem saleSystem;

    [Header("상점에서 판매할 아이템들")]
    public ShopItemData[] storeItems; // Inspector에서 할당할 상점 아이템들

    private ShopItemData shopItem;
    private SnackData snackItem;
    private ItemData itemData;
    private Item currentItem;

    void Start()
    {
        buyButton.gameObject.SetActive(false);
        sellButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        dialogueTrigger = GetComponent<DialogueTrigger>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward ,out hit, rayDistance))
            {
                if (hit.transform.gameObject == this.gameObject)
                {
                    ShowButtons();
                }
            }
        }
    }

    void ShowButtons()
    {
        buyButton.gameObject.SetActive(true);
        sellButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
    }

    public void HideButtons()
    {
        buyButton.gameObject.SetActive(false);
        sellButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
    }

    public void OnBuyClicked()
    {
        // 상점 아이템들과 인벤토리 스낵들을 모두 장바구니에 추가하여 UI 열기
        buySystem.OpenCartWithStoreItems(storeItems);
        HideButtons();
    }

    public void OnSellClicked()
    {
        if (saleSystem != null)
        {
            saleSystem.ShowSellUI();
        }
        HideButtons();
    }

    public void OnCancelClicked()
    {
        HideButtons();
    }

    
}


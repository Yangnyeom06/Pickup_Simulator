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

    [Header("연동할 시스템들")]
    public BuySystem buySystem;
    public SaleSystem saleSystem;

    private ShopItemData shopItem;
    private SnackData snackItem;
    private ItemData itemData;
    private Item currentItem;

    void Start()
    {
        buyButton.gameObject.SetActive(false);
        sellButton.gameObject.SetActive(false);
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

        CancelBtn();
    }

    void ShowButtons()
    {
        buyButton.gameObject.SetActive(true);
        sellButton.gameObject.SetActive(true);
    }

    public void HideButtons()
    {
        buyButton.gameObject.SetActive(false);
        sellButton.gameObject.SetActive(false);
    }

    public void OnBuyClicked()
    {
        buySystem.OpenCartWithInventorySnacks(); // 인벤토리 기반 장바구니 UI 열기
        HideButtons();
    }

    public void OnSellClicked()
    {
        if (currentItem != null)
        {
            saleSystem.sellUI.SetActive(true);
        }
        HideButtons();
    }

    public void CancelBtn()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            buyButton.gameObject.SetActive(false);
            sellButton.gameObject.SetActive(false); 
        }
    }
}


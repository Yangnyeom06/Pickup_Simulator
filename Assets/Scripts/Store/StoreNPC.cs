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

    [Header("상점에서 판매할 아이템들")]
    public StoreItemData[] storeItems; // Inspector에서 할당할 상점 아이템들

    private DialogueTrigger dialogueTrigger; // 대화창 용
    private ItemData itemData;
    private Item currentItem;

    void Start()
    {
        buyButton.gameObject.SetActive(false);
        sellButton.gameObject.SetActive(false);
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
                    //dialogueTrigger.TriggerSerialDialogue();
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
        // cartSnacks에 저장된 주운 아이템으로 장바구니 UI 열기
        buySystem.OpenCartWithStoreItems(storeItems);
        HideButtons();
    }

    public void OnSellClicked()
    {
        if (saleSystem != null)
        {
            saleSystem.ShowSellUI();
        }
        else
        {
            Debug.LogError("SaleSystem이 null입니다!");
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


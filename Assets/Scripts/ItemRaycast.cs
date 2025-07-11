using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.AI;

/// <summary>
/// 씬 내의 아이템(또는 정적 물체)에 다가가면 해당 아이템을 줍거나, 상호작용 할 수 있도록 해주는 스크립트
/// 플레이어의 오브젝트에 자식으로 넣은 EmptyObject에 Trigger Collider을 추가하여 사용
/// </summary>
public class ItemRaycast : MonoBehaviour
{
    
    public TextMeshProUGUI explainText;
    public GameObject followMouseImage;

    /// <summary>
    /// 레이캐스트 된 아이템
    /// </summary>
    private RaycastHit mHit;

    /// <summary>
    /// 레이캐스트 거리
    /// </summary>
    [SerializeField] private float mRayDistance;
    
    private bool mIsPickupActive = false;  //아이템 습득이 가능한가?

    private Item mCurrentItem; //활성화시 현재 등록된 아이템

    [Header("레이캐스트를 쏠 카메라")]
    [SerializeField] private Camera mRayCamera; //레이를 쏠 카메라 (메인카메라)

    [SerializeField] public InventoryManager mInventory;
    [SerializeField] public SaleSystem saleSystem;
    // [SerializeField] private ItemActionManager mItemActionCustomFunc; //아이템 상호작용 커스텀 함수 매니저 (이 글에서는 설명 X)
    // [SerializeField] private ItemRaycastInfoText mItemRaycastInfoText; //아이템 상호작용 가능시 보여질 텍스트 매니저 (이 글에서는 설명 X)

    private void Update()
    {
        CheckItem();

        // 이제 마우스 클릭이 아닌 'E' 키로 아이템 획득
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickUp();
        }
    }

    /// <summary>
    /// 레이캐스트를 이용하여 아이템을 확인한다.
    /// </summary>
    /// 
    /// 
    // 
    private void CheckItem()

    {
        if (Physics.Raycast(mRayCamera.transform.position, mRayCamera.transform.forward, out mHit, mRayDistance))
        {
            //Debug.Log("raycast 확인");
            //레이캐스트 결과의 태그가 아이템이라면?
            if (mHit.transform.tag == "Item")
            {

                //현재 레이캐스트된 아이템
                Item rayCastedItem = mHit.transform.GetComponent<Item>();

            if(mCurrentItem == rayCastedItem) { return; } 
                //아이템 얻어오기 및 정보 호출
                mCurrentItem = mHit.transform.GetComponent<Item>();
                // mItemRaycastInfoText.EnableText(mHit.transform.position + Vector3.up * rayCastedItem.IndicatorHeight, mCurrentItem.Item); (이 글에서는 설명 X)
            

               followMouseImage.SetActive(true); // 아이템 설명 이미지+텍스트 보이도록..
            if (mCurrentItem != null && mCurrentItem.GetComponent<Item>() != null )
            {
                explainText.text = mCurrentItem.itemData.itemName;
                explainText.text += mCurrentItem.itemData.itemRarity.ToString();
                explainText.text += mCurrentItem.itemData.itemType.ToString();
                explainText.text += mCurrentItem.itemData.value.ToString();
                explainText.text += mCurrentItem.itemData.dirty.ToString();
                explainText.text += mCurrentItem.itemData.description;
            }
            else
            {
                followMouseImage.SetActive(false);
                //explainText.text = ""; // 또는 다른 기본 텍스트를 넣어줄 수 있습니다.
            }

            if (mHit.transform.tag != "Item" ) // 슬롯에서 벗어났으면 설명창 닫기
            {
                followMouseImage.SetActive(false);
            }

                Debug.LogFormat("아이템: {0} 획득 가능", mCurrentItem.itemData.itemName);

                mIsPickupActive = true;

                return;
            }
            //레이캐스트 닿았을 때, 아이템이 아닌경우에는 비활성화
            else
            {
                ItemInfoDisappear();
            }
        }
        //레이캐스트 결과가 없으면 비활성화
        else
        {
            ItemInfoDisappear();
        }


    }

    /// <summary>
    /// 아이템 정보 보여주기를 비활성화 한다.
    /// </summary>
    private void ItemInfoDisappear()
    {
        //픽업 비활성화
        mIsPickupActive = false;

        //텍스트 비활성화
        // mItemRaycastInfoText.DisableText(); (이 글에서는 설명 X)

        //현재 아이템은 null
        mCurrentItem = null;
    }

    /// <summary>
    /// 아이템을 습득한다.
    /// </summary>
    public void TryPickUp()
    {
        if (mIsPickupActive)
        {
            Debug.Log("[Pickup] TryPickUp 실행됨, 아이템: " + mCurrentItem?.itemData?.itemName);
            mInventory.AddItem(mCurrentItem.itemData);
            Destroy(mCurrentItem.gameObject);
            
            if (saleSystem != null && saleSystem.sellUI.activeSelf)
            {
                saleSystem.RefreshSellSlots();
                Debug.Log("[Pickup] RefreshSellSlots 호출됨");
            }
            ItemInfoDisappear(); 
        }
    }
}


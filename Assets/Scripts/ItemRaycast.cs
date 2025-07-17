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

        // // 이제 마우스 클릭이 아닌 'E' 키로 아이템 획득
        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     TryPickUp();
        // }

        if (mIsPickupActive) 
        { 
            TryPickItem(); 
        }

    }

    /// <summary>
    /// 아이템을 주울 수 있는지 확인한다.
    /// </summary>
    private void TryPickItem()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            {
                //현재 인벤토리 아이템 가져오기
                int count = 0;

                for (; count < mInventory.slotList.Count; ++count)
                {                    //현재 아이템 칸이 null이라면 주울 수 있는 상태
                    if (mInventory.slotList[count].currentItem == null) { break; }
                }
                //모든 칸이 null이 아니고, 중첩이 불가능하면 주울 수 없음
                if (count == mInventory.slotList.Count) { return; }
                //아이템 줍는 효과음 재생
                
            }

            TryPickUp();
            ItemInfoDisappear();
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
        Ray ray = mRayCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out mHit, mRayDistance))
        {

            if (mHit.transform.CompareTag("Item") || mHit.transform.root.CompareTag("Item"))
            {

                // 자식 오브젝트에 닿은 경우에도 부모에서 Item을 찾아줌
                Item rayCastedItem = mHit.transform.GetComponent<Item>();
                if (rayCastedItem == null)
                {
                    rayCastedItem = mHit.transform.GetComponentInParent<Item>();
                }

                if (rayCastedItem == null)
                {
                    ItemInfoDisappear();
                    return;
                }

                if (mCurrentItem == rayCastedItem)
                {
                    return; // 같은 아이템이면 갱신 안 함
                }

                mCurrentItem = rayCastedItem;
                mIsPickupActive = true;
                followMouseImage.SetActive(true);

                // 아이템 설명 텍스트 표시
                var itemData = mCurrentItem.itemData;
                explainText.text = itemData.itemName +
                               itemData.itemRarity.ToString() +
                               itemData.itemType.ToString() +
                               itemData.value.ToString() +
                               itemData.dirty.ToString() +
                               itemData.description;

                Debug.LogFormat("아이템: {0} 획득 가능", itemData.itemName);
            }
            else
            {
                ItemInfoDisappear();
            }
        }
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
            // mItemActionCustomFunc.InteractionItem(mCurrentItem.Item, mCurrentItem.gameObject); (이 글에서는 설명 X)

            
                mInventory.AddItem(mCurrentItem.itemData);
                Destroy(mCurrentItem.gameObject);
            

            ItemInfoDisappear(); 
        }
    }
}


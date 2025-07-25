using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.AI;
using UnityEngine.UIElements;

/// <summary>
/// 씬 내의 아이템(또는 정적 물체)에 다가가면 해당 아이템을 줍거나, 상호작용 할 수 있도록 해주는 스크립트
/// 플레이어의 오브젝트에 자식으로 넣은 EmptyObject에 Trigger Collider을 추가하여 사용
/// </summary>
public class ItemRaycast : MonoBehaviour
{
<<<<<<< HEAD
    
=======
>>>>>>> b1474ee3016d9fd679b9a8a2c25df41a812864ad
    public TextMeshProUGUI explainText;
    public GameObject followMouseImage;

    /// <summary>
    /// 레이캐스트 된 아이템
    /// </summary>
    private RaycastHit mHit;

    /// <summary>
    /// 레이캐스트 거리
    /// </summary>
    [SerializeField] public float mRayDistance;

    private bool mIsPickupActive = false;  //아이템 습득이 가능한가?

    private Item mCurrentItem; //활성화시 현재 등록된 아이템

    private Transform mPlayerTransform;
    private GameObject mLargeItemObject;
    [SerializeField] private Transform mHoldPoint;


    private bool hand;

    [Header("레이캐스트를 쏠 카메라")]
    [SerializeField] public Camera mRayCamera; //레이를 쏠 카메라 (메인카메라)

    [SerializeField] public InventoryManager mInventory;
    [SerializeField] public SaleSystem saleSystem;
    // [SerializeField] private ItemActionManager mItemActionCustomFunc; //아이템 상호작용 커스텀 함수 매니저 (이 글에서는 설명 X)
    // [SerializeField] private ItemRaycastInfoText mItemRaycastInfoText; //아이템 상호작용 가능시 보여질 텍스트 매니저 (이 글에서는 설명 X)
    
    private void Start()
    {
        mPlayerTransform = this.transform;
    }
    

    private void Update()
    {
        CheckItem();

<<<<<<< HEAD
        // // 이제 마우스 클릭이 아닌 'E' 키로 아이템 획득
        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     TryPickUp();
        // }

        if (mIsPickupActive) 
        { 
            TryPickItem(); 
        }

=======
        if (mIsPickupActive) { TryPickItem(); }
        if (hand == true && Input.GetKeyDown(KeyCode.G)) { DropDownItem(); }
>>>>>>> b1474ee3016d9fd679b9a8a2c25df41a812864ad
    }

    /// <summary>
    /// 아이템을 주울 수 있는지 확인한다.
    /// </summary>
    private void TryPickItem()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
<<<<<<< HEAD
=======
            if (mCurrentItem.itemData.itemType == ItemType.Large && hand == false)
            {
                TryPickUpLarge();
                ItemInfoDisappear();
            }
            else
>>>>>>> b1474ee3016d9fd679b9a8a2c25df41a812864ad
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
                TryPickUp();
                ItemInfoDisappear();
            }

        }
    }

    /// <summary>
    /// 레이캐스트를 이용하여 아이템을 확인한다.
    /// </summary>
<<<<<<< HEAD
    /// 
    /// 
    // 
=======
    ///

>>>>>>> b1474ee3016d9fd679b9a8a2c25df41a812864ad
    private void CheckItem()
    {
<<<<<<< HEAD
        Ray ray = mRayCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out mHit, mRayDistance))
        {

            if (mHit.transform.CompareTag("Item") || mHit.transform.root.CompareTag("Item"))
=======

        {
            if (Physics.Raycast(mRayCamera.transform.position, mRayCamera.transform.forward, out mHit, mRayDistance))
>>>>>>> b1474ee3016d9fd679b9a8a2c25df41a812864ad
            {
                //Debug.Log("raycast 확인");
                //레이캐스트 결과의 태그가 아이템이라면?
                if (mHit.transform.tag == "Item")
                {

<<<<<<< HEAD
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
=======
                    //현재 레이캐스트된 아이템
                    Item rayCastedItem = mHit.transform.GetComponent<Item>();

                    if (mCurrentItem == rayCastedItem)
                    {
                        return;
                    }
                    //아이템 얻어오기 및 정보 호출
                    mCurrentItem = mHit.transform.GetComponent<Item>();
                    // mItemRaycastInfoText.EnableText(mHit.transform.position + Vector3.up * rayCastedItem.IndicatorHeight, mCurrentItem.Item); (이 글에서는 설명 X)

                    followMouseImage.SetActive(true); // 아이템 설명 이미지+텍스트 보이도록..


                    if (mCurrentItem != null && mCurrentItem.GetComponent<Item>() != null)
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

                    if (mHit.transform.tag != "Item") // 슬롯에서 벗어났으면 설명창 닫기
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
>>>>>>> b1474ee3016d9fd679b9a8a2c25df41a812864ad
            else
            {
                ItemInfoDisappear();
            }
        }
<<<<<<< HEAD
        else
        {
            ItemInfoDisappear();
        }
=======
>>>>>>> b1474ee3016d9fd679b9a8a2c25df41a812864ad
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


            mInventory.AddItem(mCurrentItem.itemData, mCurrentItem);
            Destroy(mCurrentItem.gameObject);


            ItemInfoDisappear();
        }
    }

    private void TryPickUpLarge()
    {
        if (mIsPickupActive && hand == false)
        {
            mLargeItemObject = mCurrentItem.gameObject;

            Collider itemCollider = mLargeItemObject.GetComponent<Collider>();
            
            if (itemCollider != null)
                itemCollider.enabled = false;

            Rigidbody rb = mLargeItemObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            mLargeItemObject.transform.SetParent(mHoldPoint);
            mLargeItemObject.transform.localPosition = Vector3.zero;
            mLargeItemObject.transform.localRotation = Quaternion.identity;

            hand = true;
        }
    }

    private void DropDownItem()
    {
        if (hand == true)
        {
            mLargeItemObject.transform.SetParent(null);

            Vector3 dropPosition = mPlayerTransform.position + mPlayerTransform.forward * 1.5f;
            mLargeItemObject.transform.position = dropPosition;
            mLargeItemObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            
            Collider itemCollider = mLargeItemObject.GetComponent<Collider>();

            if (itemCollider != null)
                itemCollider.enabled = true;

            Rigidbody rb = mLargeItemObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            mLargeItemObject = null;
            hand = false;
        }
    }
}
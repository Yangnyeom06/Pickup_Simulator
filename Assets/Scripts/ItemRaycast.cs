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
    private StoreItem mCurrentStoreItem; //활성화시 현재 등록된 상점아이템 (스낵/상점 통합)

    private Transform mPlayerTransform;
    private GameObject mLargeItemObject;
    [SerializeField] private Transform mHoldPoint;


    [SerializeField] private bool hand;

    [Header("레이캐스트를 쏠 카메라")]
    [SerializeField] public Camera mRayCamera; //레이를 쏠 카메라 (메인카메라)


    // Large 아이템 상태를 외부에서 접근할 수 있도록 하는 프로퍼티들
    public bool IsHoldingLargeItem => hand;
    public GameObject CurrentLargeItem => mLargeItemObject;
    public Item CurrentLargeItemData => mLargeItemObject?.GetComponent<Item>();

    /// <summary>
    /// 현재 들고 있는 Large 아이템을 판매로 인해 제거합니다
    /// </summary>
    public void SellCurrentLargeItem()
    {
        if (hand && mLargeItemObject != null)
        {
            string itemName = mLargeItemObject.name;

            Destroy(mLargeItemObject);
            mLargeItemObject = null;
            hand = false;
        }
        else
        {
            Debug.LogWarning($"[SellCurrentLargeItem] 제거할 수 없음 - hand: {hand}, mLargeItemObject: {mLargeItemObject != null}");
        }
    }
    // [SerializeField] private ItemActionManager mItemActionCustomFunc; //아이템 상호작용 커스텀 함수 매니저 (이 글에서는 설명 X)
    // [SerializeField] private ItemRaycastInfoText mItemRaycastInfoText; //아이템 상호작용 가능시 보여질 텍스트 매니저 (이 글에서는 설명 X)

    private void Start()
    {
        mPlayerTransform = this.transform;

        // 필수 컴포넌트들이 할당되었는지 확인
        if (mRayCamera == null)
        {
            Debug.LogError("ItemRaycast: mRayCamera가 할당되지 않았습니다!");
        }

    }


    private void Update()
    {
        CheckItem();

        if (mIsPickupActive) { TryPickItem(); }
        if (hand == true && Input.GetKeyDown(KeyCode.G)) { DropDownItem(); }
    }

    /// <summary>
    /// 아이템을 주울 수 있는지 확인한다.
    /// </summary>
    private void TryPickItem()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Physics.Raycast(mRayCamera.transform.position, mRayCamera.transform.forward, out mHit, mRayDistance);

            if (mHit.transform.tag == "note")
            {
                DayManager.Instance.Open();
            }

            // 상점 아이템 처리 (스낵/상점 통합)
            if (mCurrentStoreItem != null)
            {
                mCurrentStoreItem.PickupItem();
                ItemInfoDisappear();
                return;
            }

            // 일반 아이템 처리
            if (mCurrentItem != null)
            {
                if (mCurrentItem.itemData.itemType == ItemType.Large && hand == false)
                {
                    TryPickUpLarge();
                    ItemInfoDisappear();
                }
                else if (mCurrentItem.itemData.itemType != ItemType.Large)
                {
                    //현재 인벤토리 아이템 가져오기
                    int count = 0;

                    for (; count < InventoryManager.Instance.slotList.Count; ++count)
                    {                    //현재 아이템 칸이 null이라면 주울 수 있는 상태
                        bool isNull = (InventoryManager.Instance.slotList[count].currentItem == null);
                        if (isNull) { break; }
                    }
                    //모든 칸이 null이 아니고, 중첩이 불가능하면 주울 수 없음
                    if (count == InventoryManager.Instance.slotList.Count)
                    {
                        Debug.Log("인벤토리가 가득함");
                        return;
                    }
                    else
                    {
                        Debug.Log($"빈 슬롯 발견");
                    }
                    //아이템 줍는 효과음 재생
                    TryPickUp();
                    ItemInfoDisappear();
                }
            }
        }
    }

    /// <summary>
    /// 레이캐스트를 이용하여 아이템을 확인한다.
    /// </summary>
    ///

    private void CheckItem()
    {
        {
            if (Physics.Raycast(mRayCamera.transform.position, mRayCamera.transform.forward, out mHit, mRayDistance))
            {
                // 레이캐스트가 뭔가에 닿았을 때 디버깅
                //Debug.Log($"레이캐스트 적중: {mHit.transform.name} (태그: {mHit.transform.tag})");
                // 상점 아이템 체크 (스낵/상점 통합)
                StoreItem storeItem = mHit.transform.GetComponent<StoreItem>();
                if (storeItem != null)
                {
                    if (mCurrentStoreItem == storeItem)
                    {
                        return;
                    }

                    // 기존 아이템들 초기화
                    mCurrentItem = null;
                    mCurrentStoreItem = storeItem;

                    followMouseImage.SetActive(true);

                    // 아이템 타입에 따라 정보 표시
                    if (storeItem.storeItemData != null)
                    {
                        if (storeItem.storeItemData.dataType == StoreItemDataType.Snack)
                        {
                            explainText.text = storeItem.storeItemData.snackName;
                            explainText.text += $"\n가격: {storeItem.storeItemData.price}";
                            explainText.text += $"\n설명: {storeItem.storeItemData.description}";
                        }
                        else if (storeItem.storeItemData.dataType == StoreItemDataType.ShopItem)
                        {
                            explainText.text = storeItem.storeItemData.itemName;
                            explainText.text += $"\n가격: {storeItem.storeItemData.price}";
                            explainText.text += $"\n설명: {storeItem.storeItemData.description}";
                        }
                    }

                    mIsPickupActive = true;
                    return;
                }

                //레이캐스트 결과의 태그가 아이템이라면?
                if (mHit.transform.tag == "Item")
                {
                    //현재 레이캐스트된 아이템
                    Item item = mHit.transform.GetComponent<Item>();

                    if (item == null)
                    {
                        ItemInfoDisappear();
                        return;
                    }

                    if (mCurrentItem == item)
                    {
                        return;
                    }

                    // 기존 상점 아이템 초기화
                    mCurrentStoreItem = null;
                    mCurrentItem = mHit.transform.GetComponent<Item>();
                    // mItemRaycastInfoText.EnableText(mHit.transform.position + Vector3.up * item.IndicatorHeight, mCurrentItem.Item); (이 글에서는 설명 X)

                    followMouseImage.SetActive(true); // 아이템 설명 이미지+텍스트 보이도록..


                    if (mCurrentItem != null && mCurrentItem.itemData != null)
                    {
                        explainText.text = mCurrentItem.itemData.itemName + "\n";
                        explainText.text += mCurrentItem.itemData.itemRarity.ToString() + "\n";
                        explainText.text += mCurrentItem.itemData.itemType.ToString() + "\n";
                        explainText.text += mCurrentItem.itemData.value.ToString() + "\n";
                        explainText.text += mCurrentItem.itemData.dirty.ToString() + "\n";
                        explainText.text += mCurrentItem.itemData.description;

                        mIsPickupActive = true;
                    }
                    else
                    {
                        followMouseImage.SetActive(false);
                        ItemInfoDisappear();
                        return;
                    }

                    return;
                }

                //레이캐스트 닿았을 때, 아이템이 아닌경우에는 비활성화
                else
                {
                    ItemInfoDisappear();
                }

                if (mHit.transform.tag == "note")
                {
                    mIsPickupActive = true;
                }

            }
            //레이캐스트 결과가 없으면 비활성화
            else
            {
                // 레이캐스트가 아무것도 감지하지 못함
                //Debug.Log("레이캐스트 결과 없음");
                ItemInfoDisappear();
            }
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
        mCurrentStoreItem = null;

        followMouseImage.SetActive(false);
    }

    /// <summary>
    /// 아이템을 습득한다.
    /// </summary>
    private void TryPickUp()
    {
        if (mIsPickupActive)
        {
            if (mCurrentItem == null)
            {
                ItemInfoDisappear();
                return;
            }

            if (mCurrentItem.itemData == null)
            {
                ItemInfoDisappear();
                return;
            }


            // 인벤토리에 아이템 추가 시도
            bool success = InventoryManager.Instance.AddItem(mCurrentItem.itemData, mCurrentItem);

            if (success)
            {
                Destroy(mCurrentItem.gameObject);
            }
            else
            {
                Debug.Log($"'{mCurrentItem.itemData.itemName}' 줍기 실패 - 인벤토리가 가득참");
            }

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
                itemCollider.isTrigger = true;

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
                itemCollider.isTrigger = false;

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

    public void ItemOutLiner()
    {
        
    }
}
using UnityEngine;

public class SnackItem : MonoBehaviour
{

    [Header("레이캐스트를 쏠 카메라")]
    [SerializeField] private Camera mRayCamera; //레이를 쏠 카메라 (메인카메라)
    public float rayDistance = 100f;

    /// <summary>
    /// 레이캐스트 된 아이템
    /// </summary>
    private RaycastHit mHit;

    public SnackData snackItemData;
    public BuySystem buySystem;

    private void Awake()
    {
        if (buySystem == null)
            buySystem = FindObjectOfType<BuySystem>();

        if (snackItemData == null)
            snackItemData = GetComponent<SnackData>(); // 또는 직접 생성하거나 리소스에서 로드
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(mRayCamera.transform.position, mRayCamera.transform.forward, out hit, rayDistance))
            {
                var snack = hit.transform.GetComponent<SnackItem>();
                if (snack != null && snack.snackItemData != null)
                {
                    InventoryManager.Instance.AddSnack(snack.snackItemData);

                    // ✅ 장바구니에 바로 추가
                    if (snack.buySystem != null)
                    {
                        snack.buySystem.AddToSnackCart(snack.snackItemData);
                    }
                    else
                    {
                        Debug.LogWarning("BuySystem이 연결되지 않았습니다!");
                    }

                    Destroy(hit.transform.gameObject); // 아이템 제거
                    Debug.Log($"{snack.snackItemData.snackName}을(를) 주웠습니다!");
                }

            }
        }

    }
}
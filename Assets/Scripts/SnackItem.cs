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

    // Update에서 입력 처리 제거 - 중앙에서 관리하도록 변경
    // private void Update()
    // {
    //     // 입력 처리는 플레이어나 중앙 매니저에서 처리
    // }

    // 외부에서 호출할 수 있는 줍기 메서드
    public void PickupSnack()
    {
        if (snackItemData != null)
        {
            if (buySystem != null)
            {
                buySystem.AddToSnackCart(snackItemData);
                Debug.Log($"{snackItemData.snackName}을(를) 주웠습니다!");
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("BuySystem이 연결되지 않았습니다!");
            }
        }
    }
}
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
        // Inspector에서 할당되지 않은 경우 싱글톤 사용
        if (buySystem == null)
        {
            // 싱글톤 Instance 먼저 확인
            if (BuySystem.Instance != null)
            {
                buySystem = BuySystem.Instance;
                Debug.Log($"SnackItem: BuySystem 싱글톤을 사용합니다: {buySystem.gameObject.name}");
            }
            else
            {
                // 싱글톤이 없으면 직접 찾기
                buySystem = FindObjectOfType<BuySystem>();
                
                // 비활성화된 것도 포함해서 찾기
                if (buySystem == null)
                {
                    buySystem = FindObjectOfType<BuySystem>(true);
                }
                
                if (buySystem == null)
                {
                    Debug.LogError("SnackItem: BuySystem을 찾을 수 없습니다! 씬에 BuySystem이 있는지 확인하세요.");
                    Debug.LogError("해결방법: 1) BuySystem GameObject가 활성화되어 있는지 확인 2) Inspector에서 직접 할당");
                }
                else
                {
                    Debug.Log($"SnackItem: BuySystem을 자동으로 찾았습니다: {buySystem.gameObject.name}");
                }
            }
        }

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
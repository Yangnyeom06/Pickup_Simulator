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

    public SnackItemData snackItemData;
    public BuySystem buySystem;
    private bool isProcessing = false; // 중복 호출 방지 플래그
    private static bool globalProcessing = false; // 전역 중복 방지 플래그

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
                buySystem = FindFirstObjectByType<BuySystem>();
                
                // 비활성화된 것도 포함해서 찾기
                if (buySystem == null)
                {
                    buySystem = FindFirstObjectByType<BuySystem>(FindObjectsInactive.Include);
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
            snackItemData = GetComponent<SnackItemData>(); // 또는 직접 생성하거나 리소스에서 로드
    }



    // 외부에서 호출할 수 있는 줍기 메서드 (하위 호환성을 위해 유지)
    public void PickupSnack()
    {
        // 전역 및 로컬 중복 호출 방지
        if (isProcessing || globalProcessing)
        {
            Debug.Log($"PickupSnack 중복 호출 방지됨: isProcessing={isProcessing}, globalProcessing={globalProcessing}");
            return;
        }
        
        isProcessing = true;
        globalProcessing = true;
        
        Debug.Log($"PickupSnack 시작: {snackItemData?.snackName ?? "null"}");
        
        if (snackItemData != null)
        {
            if (buySystem != null)
            {
                buySystem.AddToSnackCart(snackItemData);
                Debug.Log($"장바구니에 {snackItemData.snackName} 추가 완료");
                // 반복 구매 가능하도록 오브젝트는 삭제하지 않음
            }
            else
            {
                Debug.LogWarning("BuySystem이 연결되지 않았습니다!");
            }
        }
        else
        {
            Debug.LogWarning("SnackItemData가 설정되지 않았습니다!");
        }
        
        // 처리 완료 후 플래그 해제 (약간의 지연을 두어 중복 클릭 방지)
        StartCoroutine(ResetProcessingFlag());
    }
    
    private System.Collections.IEnumerator ResetProcessingFlag()
    {
        yield return new UnityEngine.WaitForSeconds(0.5f); // 0.5초 대기로 증가
        isProcessing = false;
        globalProcessing = false; // 전역 플래그도 해제
        Debug.Log("PickupSnack 처리 플래그 해제됨");
    }
}
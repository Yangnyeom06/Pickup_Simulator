using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 중고트럭의 랜덤 스폰을 관리하는 매니저
/// </summary>
public class UsedCarSpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float spawnProbability = 0.4f; // 40% 확률로 스폰
    [SerializeField] private int minDaysBetweenSpawns = 1; // 최소 간격 (일)
    [SerializeField] private int maxDaysBetweenSpawns = 4; // 최대 간격 (일)
    
    [Header("Truck References")]
    [SerializeField] private GameObject usedCarTruckObject; // 중고트럭 GameObject
    [SerializeField] private UsedCarNPC usedCarNPC; // UsedCarNPC 컴포넌트
    
    [Header("Spawn Locations")]
    [SerializeField] private Transform[] spawnPoints; // 스폰 가능한 위치들
    [SerializeField] private Transform defaultSpawnPoint; // 기본 스폰 위치
    [SerializeField] private float spawnHeightOffset = 0.5f; // 지면 위 높이 오프셋
    [SerializeField] private bool autoCalculateHeight = true; // 오브젝트 크기에 따른 자동 높이 계산
    
    [Header("Debug Info")]
    [SerializeField] private bool isCurrentlySpawned = false;
    [SerializeField] private int lastSpawnDay = -1;
    [SerializeField] private int daysSinceLastSpawn = 0;
    
    // 싱글톤 패턴
    public static UsedCarSpawnManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // 시작시 중고트럭 비활성화
        if (usedCarTruckObject != null)
        {
            usedCarTruckObject.SetActive(false);
            isCurrentlySpawned = false;
        }
        
        // 기본 스폰 포인트 설정
        if (defaultSpawnPoint == null && spawnPoints.Length > 0)
        {
            defaultSpawnPoint = spawnPoints[0];
        }
    }
    
    /// <summary>
    /// 새로운 날이 시작될 때 호출되는 메소드
    /// </summary>
    /// <param name="currentDay">현재 날짜</param>
    public void OnNewDayStarted(int currentDay)
    {
        daysSinceLastSpawn = lastSpawnDay >= 0 ? currentDay - lastSpawnDay : 999;
        
        Debug.Log($"[UsedCarSpawnManager] 새로운 날 시작: {currentDay}일차");
        Debug.Log($"  - 마지막 스폰: {lastSpawnDay}일차");
        Debug.Log($"  - 경과 일수: {daysSinceLastSpawn}일");
        
        // 스폰 여부 결정
        bool shouldSpawn = ShouldSpawnToday();
        
        if (shouldSpawn)
        {
            SpawnUsedCarTruck(currentDay);
        }
        else
        {
            DespawnUsedCarTruck();
        }
    }
    
    /// <summary>
    /// 오늘 중고트럭이 스폰되어야 하는지 결정
    /// </summary>
    /// <returns>스폰 여부</returns>
    private bool ShouldSpawnToday()
    {
        // 최소 간격이 지나지 않았다면 스폰하지 않음
        if (daysSinceLastSpawn < minDaysBetweenSpawns)
        {
            Debug.Log($"[UsedCarSpawnManager] 최소 간격 미충족 ({daysSinceLastSpawn}/{minDaysBetweenSpawns}일)");
            return false;
        }
        
        // 최대 간격이 지났다면 무조건 스폰
        if (daysSinceLastSpawn >= maxDaysBetweenSpawns)
        {
            Debug.Log($"[UsedCarSpawnManager] 최대 간격 도달 - 강제 스폰 ({daysSinceLastSpawn}/{maxDaysBetweenSpawns}일)");
            return true;
        }
        
        // 확률적으로 결정
        float randomValue = Random.Range(0f, 1f);
        bool shouldSpawn = randomValue < spawnProbability;
        
        Debug.Log($"[UsedCarSpawnManager] 확률 체크: {randomValue:F3} < {spawnProbability:F3} = {shouldSpawn}");
        
        return shouldSpawn;
    }
    
    /// <summary>
    /// 중고트럭 스폰
    /// </summary>
    /// <param name="currentDay">현재 날짜</param>
    private void SpawnUsedCarTruck(int currentDay)
    {
        
        // 스폰 위치 결정
        Transform spawnPoint = GetRandomSpawnPoint();
        
        if (spawnPoint != null)
        {
            Vector3 spawnPosition = GetGroundPosition(spawnPoint);
            usedCarTruckObject.transform.position = spawnPosition;
            usedCarTruckObject.transform.rotation = spawnPoint.rotation;
        }
        
        // 트럭 활성화
        usedCarTruckObject.SetActive(true);
        isCurrentlySpawned = true;
        
        // 마지막 스폰 날짜 기록 (현재 날짜 사용)
        lastSpawnDay = currentDay;
        
        Debug.Log($"[UsedCarSpawnManager] 중고트럭 스폰! 위치: {spawnPoint?.name ?? "기본위치"}");
        
        // UI 메시지 표시 (선택사항)
        ShowSpawnNotification();
    }
    
    /// <summary>
    /// 중고트럭 제거
    /// </summary>
    private void DespawnUsedCarTruck()
    {
        if (usedCarTruckObject != null)
        {
            usedCarTruckObject.SetActive(false);
            isCurrentlySpawned = false;
            
        }
    }
    
    /// <summary>
    /// 랜덤한 스폰 포인트 선택
    /// </summary>
    /// <returns>선택된 스폰 포인트</returns>
    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            // 유효한 스폰 포인트들만 필터링
            var validSpawnPoints = new System.Collections.Generic.List<Transform>();
            
            foreach (var point in spawnPoints)
            {
                if (point != null)
                {
                    validSpawnPoints.Add(point);
                }
            }
            
            if (validSpawnPoints.Count > 0)
            {
                int randomIndex = Random.Range(0, validSpawnPoints.Count);
                return validSpawnPoints[randomIndex];
            }
        }
        
        return defaultSpawnPoint;
    }
    
    /// <summary>
    /// 지면 위의 올바른 스폰 위치를 계산
    /// </summary>
    /// <param name="spawnPoint">기준 스폰 포인트</param>
    /// <returns>지면 위의 위치</returns>
    private Vector3 GetGroundPosition(Transform spawnPoint)
    {
        Vector3 spawnPosition = spawnPoint.position;
        float finalOffset = spawnHeightOffset;
        
        // 자동 높이 계산이 활성화되어 있다면 오브젝트의 크기를 고려
        if (autoCalculateHeight && usedCarTruckObject != null)
        {
            // 오브젝트의 Collider 또는 Renderer 경계를 사용해서 높이 계산
            Bounds objectBounds = GetObjectBounds(usedCarTruckObject);
            if (objectBounds.size != Vector3.zero)
            {
                // 오브젝트 높이의 절반 + 추가 오프셋
                finalOffset = (objectBounds.size.y * 0.5f) + spawnHeightOffset;
                Debug.Log($"[UsedCarSpawnManager] 자동 높이 계산: 오브젝트 높이 {objectBounds.size.y:F2}m, 최종 오프셋: {finalOffset:F2}m");
            }
        }
        
        // Raycast를 사용해서 아래쪽으로 지면을 찾기
        RaycastHit hit;
        Vector3 rayStart = spawnPosition + Vector3.up * 10f; // 스폰 포인트 위 10m에서 시작
        
        if (Physics.Raycast(rayStart, Vector3.down, out hit, 20f))
        {
            // 지면을 찾았다면 그 위치에 계산된 오프셋을 추가
            spawnPosition = hit.point + Vector3.up * finalOffset;
            Debug.Log($"[UsedCarSpawnManager] 지면 감지: {hit.point}, 최종 위치: {spawnPosition}");
        }
        else
        {
            // 지면을 찾지 못했다면 기본 오프셋 사용
            spawnPosition.y += finalOffset;
            Debug.LogWarning($"[UsedCarSpawnManager] 지면을 찾지 못함. 기본 오프셋 사용: {spawnPosition}");
        }
        
        return spawnPosition;
    }
    
    /// <summary>
    /// 오브젝트의 경계(Bounds) 계산
    /// </summary>
    /// <param name="obj">대상 오브젝트</param>
    /// <returns>오브젝트의 경계</returns>
    private Bounds GetObjectBounds(GameObject obj)
    {
        Bounds bounds = new Bounds();
        bool hasBounds = false;
        
        // Renderer들의 경계를 확인
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }
        
        // Renderer가 없다면 Collider 확인
        if (!hasBounds)
        {
            Collider[] colliders = obj.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                if (!hasBounds)
                {
                    bounds = collider.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(collider.bounds);
                }
            }
        }
        
        return bounds;
    }
    
    /// <summary>
    /// 스폰 알림 표시 (선택사항)
    /// </summary>
    private void ShowSpawnNotification()
    {
        // 게임에 알림 시스템이 있다면 여기서 호출
        Debug.Log("중고차 상인이 마을에 도착했습니다!");
    }
    
    /// <summary>
    /// 현재 중고트럭이 스폰되어 있는지 확인
    /// </summary>
    /// <returns>스폰 여부</returns>
    public bool IsUsedCarTruckSpawned()
    {
        return isCurrentlySpawned && usedCarTruckObject != null && usedCarTruckObject.activeSelf;
    }
    
    /// <summary>
    /// 강제로 중고트럭 스폰 (디버그/치트용)
    /// </summary>
    [ContextMenu("Force Spawn Truck")]
    public void ForceSpawnTruck()
    {
        SpawnUsedCarTruck(0); // 디버그용이므로 날짜는 0으로 설정
    }
    
    /// <summary>
    /// 강제로 중고트럭 제거 (디버그/치트용)
    /// </summary>
    [ContextMenu("Force Despawn Truck")]
    public void ForceDespawnTruck()
    {
        DespawnUsedCarTruck();
    }
    
    /// <summary>
    /// 스폰 확률 설정
    /// </summary>
    /// <param name="probability">0.0 ~ 1.0 사이의 확률값</param>
    public void SetSpawnProbability(float probability)
    {
        spawnProbability = Mathf.Clamp01(probability);
        Debug.Log($"[UsedCarSpawnManager] 스폰 확률 변경: {spawnProbability:P0}");
    }
    
    /// <summary>
    /// Inspector에서 설정 확인용
    /// </summary>
    private void OnValidate()
    {
        spawnProbability = Mathf.Clamp01(spawnProbability);
        minDaysBetweenSpawns = Mathf.Max(0, minDaysBetweenSpawns);
        maxDaysBetweenSpawns = Mathf.Max(minDaysBetweenSpawns, maxDaysBetweenSpawns);
    }
}

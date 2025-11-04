using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 중고트럭의 랜덤 스폰을 관리하는 매니저
/// </summary>
public class UsedCarSpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float spawnProbability = 0.7f; // 70% 확률로 스폰 (더 자주 나타나도록 증가)
    [SerializeField] private int minDaysBetweenSpawns = 0; // 최소 간격 (일) - 0으로 설정하여 매일 체크 가능
    [SerializeField] private int maxDaysBetweenSpawns = 2; // 최대 간격 (일) - 2일로 줄여서 더 자주 나타나도록
    
    [Header("Truck References")]
    [SerializeField] private GameObject usedCarTruckObject; // 중고트럭 GameObject
    [SerializeField] private UsedCarNPC usedCarNPC; // UsedCarNPC 컴포넌트
    
    [Header("Spawn Locations")]
    [SerializeField] private SpawnMode spawnMode = SpawnMode.TransformPoints; // 스폰 모드 선택
    [SerializeField] private Transform[] spawnPoints; // 스폰 가능한 위치들 (Transform 모드용)
    [SerializeField] private Transform defaultSpawnPoint; // 기본 스폰 위치
    [SerializeField] private GameObject rangeObject; // MeshCollider 범위 오브젝트 (MeshCollider 모드용)
    private MeshCollider meshCollider; // MeshCollider 참조
    
    [Header("Debug Info")]
    [SerializeField] private bool isCurrentlySpawned = false;
    [SerializeField] private int lastSpawnDay = -1;
    [SerializeField] private int daysSinceLastSpawn = 0;
    
    // 싱글톤 패턴
    public static UsedCarSpawnManager Instance { get; private set; }
    
    /// <summary>
    /// 스폰 모드 열거형
    /// </summary>
    public enum SpawnMode
    {
        TransformPoints,    // 고정된 Transform 포인트 사용
        MeshColliderBounds  // MeshCollider bounds 내 랜덤 위치 사용
    }
    
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
        
        // MeshCollider 모드인 경우 초기화
        if (spawnMode == SpawnMode.MeshColliderBounds && rangeObject != null)
        {
            meshCollider = rangeObject.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                Debug.LogWarning($"[UsedCarSpawnManager] rangeObject에 MeshCollider가 없습니다! Transform 모드로 변경됩니다.");
                spawnMode = SpawnMode.TransformPoints;
            }
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
        
        // Transform 모드인 경우 기본 스폰 포인트 설정
        if (spawnMode == SpawnMode.TransformPoints)
        {
            if (defaultSpawnPoint == null && spawnPoints != null && spawnPoints.Length > 0)
            {
                defaultSpawnPoint = spawnPoints[0];
            }
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
        // 첫 스폰인 경우 (lastSpawnDay가 -1) 또는 최소 간격이 지난 경우
        if (lastSpawnDay < 0 || daysSinceLastSpawn >= minDaysBetweenSpawns)
        {
            // 최대 간격이 지났다면 무조건 스폰
            if (daysSinceLastSpawn >= maxDaysBetweenSpawns)
            {
                Debug.Log($"[UsedCarSpawnManager] 최대 간격 도달 - 강제 스폰 ({daysSinceLastSpawn}/{maxDaysBetweenSpawns}일)");
                return true;
            }
            
            // 확률적으로 결정 (더 자주 나타나도록)
            float randomValue = Random.Range(0f, 1f);
            bool shouldSpawn = randomValue < spawnProbability;
            
            Debug.Log($"[UsedCarSpawnManager] 확률 체크: {randomValue:F3} < {spawnProbability:F3} = {shouldSpawn}");
            
            return shouldSpawn;
        }
        
        // 최소 간격이 지나지 않았다면 스폰하지 않음
        Debug.Log($"[UsedCarSpawnManager] 최소 간격 미충족 ({daysSinceLastSpawn}/{minDaysBetweenSpawns}일)");
        return false;
    }
    
    /// <summary>
    /// 중고트럭 스폰
    /// </summary>
    /// <param name="currentDay">현재 날짜</param>
    private void SpawnUsedCarTruck(int currentDay)
    {
        // 스폰 위치 결정 (모드에 따라 다름)
        Vector3 spawnPosition = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;
        
        if (spawnMode == SpawnMode.MeshColliderBounds)
        {
            // MeshCollider bounds 내 랜덤 위치 생성
            spawnPosition = Return_RandomPosition();
            spawnRotation = Quaternion.identity; // 기본 회전
        }
        else
        {
            // Transform 포인트 사용
            Transform spawnPoint = GetRandomSpawnPoint();
            if (spawnPoint != null)
            {
                spawnPosition = spawnPoint.position;
                spawnRotation = spawnPoint.rotation;
            }
            else if (defaultSpawnPoint != null)
            {
                spawnPosition = defaultSpawnPoint.position;
                spawnRotation = defaultSpawnPoint.rotation;
            }
        }
        
        // 트럭 위치 및 회전 설정
        usedCarTruckObject.transform.position = spawnPosition;
        usedCarTruckObject.transform.rotation = spawnRotation;
        
        // 트럭 활성화
        usedCarTruckObject.SetActive(true);
        isCurrentlySpawned = true;
        
        // 마지막 스폰 날짜 기록 (현재 날짜 사용)
        lastSpawnDay = currentDay;
        
        Debug.Log($"[UsedCarSpawnManager] 중고트럭 스폰! 위치: {spawnPosition}, 모드: {spawnMode}");
        
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
    /// 랜덤한 스폰 포인트 선택 (Transform 모드용)
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
    /// MeshCollider bounds 내에서 랜덤 위치 반환 (MeshCollider 모드용)
    /// </summary>
    /// <returns>랜덤 위치</returns>
    private Vector3 Return_RandomPosition()
    {
        if (meshCollider == null || rangeObject == null)
        {
            Debug.LogWarning("[UsedCarSpawnManager] MeshCollider 또는 rangeObject가 null입니다. 기본 위치 반환");
            return Vector3.zero;
        }
        
        Vector3 originPosition = rangeObject.transform.position;
        
        // 콜라이더의 사이즈를 가져오는 bounds.size
        float range_x = meshCollider.bounds.size.x;
        float range_z = meshCollider.bounds.size.z;
        
        float random_x = Random.Range((range_x / 2) * -1, range_x / 2);
        float random_z = Random.Range((range_z / 2) * -1, range_z / 2);
        
        Vector3 RandomPosition = new Vector3(random_x, 0f, random_z); // 랜덤 위치
        Vector3 randomPosition = originPosition + RandomPosition;
        
        return randomPosition;
    }
    
    /// <summary>
    /// 스폰 알림 표시 (선택사항)
    /// </summary>
    private void ShowSpawnNotification()
    {
        // 게임에 알림 시스템이 있다면 여기서 호출
        Debug.Log("🚛 중고차 상인이 마을에 도착했습니다!");
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
        
        // MeshCollider 모드인 경우 meshCollider 초기화
        if (spawnMode == SpawnMode.MeshColliderBounds && rangeObject != null)
        {
            meshCollider = rangeObject.GetComponent<MeshCollider>();
        }
    }
}

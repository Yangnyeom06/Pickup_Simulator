using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 중고트럭의 랜덤 스폰을 관리하는 스크립트
/// </summary>
public class UsedCarSpawn : MonoBehaviour {
    public GameObject rangeObject;
    MeshCollider meshCollider;


    private void Awake() {
        meshCollider = rangeObject.GetComponent<MeshCollider>();
    }

    Vector3 Return_RandomPosition()
    {
        Vector3 originPosition = rangeObject.transform.position;

        //콜라이더의 사이즈를 가져오는 bound.size
        float range_x = meshCollider.bounds.size.x;
        float range_z = meshCollider.bounds.size.z;

        float random_x = Random.Range((range_x/2)*-1, range_x/2);
        float random_z = Random.Range((range_z/2)*-1, range_z/2);
        Vector3 RandomPosition = new Vector3(random_x, 0f, random_z);    // 랜덤 위치

        Vector3 randomPosition = originPosition + RandomPosition;
        return randomPosition;
    }

    // 소환할 Object
    public GameObject usedCarNPC;

    private void Start()
    {
        StartCoroutine(RandomRespawn_Coroutine());
    }

    IEnumerator RandomRespawn_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(10f, 20f));
            
            // 생성 위치 부분에 위에서 만든 Return_RandomPosition() 함수를 사용
            GameObject instantCapsul = Instantiate(usedCarNPC, Return_RandomPosition(), Quaternion.identity);
        }
    }
}
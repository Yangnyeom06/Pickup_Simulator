using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;
public class ItemObject : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	Vector3 DefaultPos;

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
		//올바르지 않은 곳에 드래그 했을 때 돌아갈 위치 저장
		DefaultPos = this.transform.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
		// 현재 터치되고 있는 좌표를 저장해서 오브젝트가 손가락을 따라갈 수 있도록 오브젝트의 좌표로 넣어줌
		Vector3 currentPos = Camera.main.ScreenToWorldPoint(eventData.position);
    currentPos.z = 0; // 필요한 경우, Z 좌표를 0으로 설정
    transform.position = currentPos;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("드래그가 끝났습니다.");
        // 여기서 아이템을 드래그 종료 후 처리
    }
}

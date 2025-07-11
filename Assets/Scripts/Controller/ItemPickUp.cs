using UnityEngine;

/// <summary>
/// Item을 넣는 공간 프리팹에 컴포넌트로 추가하고 인스펙터에 아이템을 할당한다.
/// </summary>
public class ItemPickUp : MonoBehaviour
{
    [Header("해당 오브젝트에 할당되는 아이템")]
    [SerializeField] private Item mItem; // 상호작용 할 게임오브젝트에서 받아올 아이템 정보
    /// <summary>
    /// 상호작용 가능한 객체가 가지고 있는 아이템
    /// /// </summary>
    /// <value></value>
    public Item Item
    {
        get 
        {
            return mItem;
        }
    }

    [Header("해당 오브젝트에 상호작용시, 보여줄 인디케이터의 높이")]
    [SerializeField] private float mIndicatorHeight; // 해당 아이템에 대한 정보를 받아올 때, 보여줄 인디케이터(텍스트 등)에 대한 높이
    /// <summary>
    /// 인디케이터의 높이
    /// /// </summary>
    /// <value></value>
    public float IndicatorHeight
    {
        get
        {
            return mIndicatorHeight;
        }
    }
}
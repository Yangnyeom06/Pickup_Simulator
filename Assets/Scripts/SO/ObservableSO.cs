using UnityEngine;
using System;

public abstract class ObservableSO<T> : ScriptableObject
{
    // T는 아직 타입을 명시적으로 하지 않아 여러 타입용 SO 구조를 공통적으로 만들 수 있음
    [SerializeField]
    private T _value;
    public event Action<T> OnValueChanged;
    public T Value // 변수를 변경할때 특정 함수가 실행되게 하는 목적적
    {
        get => _value; // 이 변수가 다른데에서 사용될 때 실행되는 코드 ex) a = b.Value
        set // 이 변수를 변경할 때 실행되는 코드 ex) b.Value = a 
        {
            if (Equals(_value, value)) return;
            _value = value;
            OnValueChanged?.Invoke(_value);
        }
    }

    public void SetValueWithoutNotify(T newValue) // 변수를 사용할때 get이나 set 안에 있는 함수 실행하지 않고 쓰기
    {
        _value = newValue;
    }

    public void Register(Action<T> callback) => OnValueChanged += callback;
    public void Unregister(Action<T> callback) => OnValueChanged -= callback;
}
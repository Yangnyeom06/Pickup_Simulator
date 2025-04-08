using UnityEngine;
using System;

public abstract class ObservableSO<T> : ScriptableObject
{
    // T는 아직 타입을 명시적으로 하지 않아 여러 타입용 SO 구조를 공통적으로 만들 수 있음음
    [SerializeField]
    private T _value;

    public event Action<T> OnValueChanged;

    public T Value
    {
        get => _value;
        set
        {
            if (Equals(_value, value)) return;
            _value = value;
            OnValueChanged?.Invoke(_value);
        }
    }

    public void SetValueWithoutNotify(T newValue)
    {
        _value = newValue;
    }

    public void Register(Action<T> callback) => OnValueChanged += callback;
    public void Unregister(Action<T> callback) => OnValueChanged -= callback;
}
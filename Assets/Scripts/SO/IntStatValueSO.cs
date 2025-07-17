using UnityEngine;
using System;
using JetBrains.Annotations;

[CreateAssetMenu(menuName = "Observabble/Int/stats")]
public class IntStatValueSO : ObservableSO<int>
{
    public int maxLevel;
    [SerializeField]
    private float _max;
    public event Action<float> OnMaxChanged;
    public float max
    {
        get => _max;
        set
        {
            if (Mathf.Approximately(_max, value)) return;
            _max = value;
            OnMaxChanged?.Invoke(_max);
        }
    }

    [SerializeField]
    private float _current;
    public event Action<float> OnCurrentChanged;
    public float current
    {
        get => _current;
        set
        {
            if (Mathf.Approximately(_current, value)) return;
            _current = value;
            OnCurrentChanged?.Invoke(_current);
        }
    }


    public void SetCurrentWithoutNotify(float value) => _current = value;
    public void SetMaxWithoutNotify(float value) => _max = value;
}
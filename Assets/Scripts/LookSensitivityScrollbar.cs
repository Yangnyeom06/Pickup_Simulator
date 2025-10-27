using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Scrollbar 위치와 LookSensitivity(PlayerPrefs)를 양방향으로 동기화.
/// PlayerController가 없는 씬에서도 작동한다.
/// </summary>
public class LookSensitivityScrollbar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Scrollbar scrollbar;          // Inspector에 연결
    [Header("Range")]
    [SerializeField] private float min = 0.1f;
    [SerializeField] private float max = 10f;

    private const string PREF_KEY_SENS = "LookSensitivity";
    private bool selfUpdate;  // 재귀 호출 방지

    /* ───── 초기 세팅 ───── */
    private void Awake()
    {
        if (scrollbar == null)
            scrollbar = GetComponent<Scrollbar>();

        // Scrollbar → PlayerPrefs
        scrollbar.onValueChanged.AddListener(OnBarChanged);

        // PlayerPrefs → Scrollbar (씬 시작 시)
        float saved = PlayerPrefs.GetFloat(PREF_KEY_SENS, 2f);
        SetBarSilently(Mathf.InverseLerp(min, max, saved));
    }

    /* ───── 매 프레임 PlayerPrefs 변동 감시 (선택사항) ───── */
    private void LateUpdate()
    {
        float prefs = PlayerPrefs.GetFloat(PREF_KEY_SENS, 2f);
        float t = Mathf.InverseLerp(min, max, prefs);

        if (!Mathf.Approximately(scrollbar.value, t))
            SetBarSilently(t);
    }

    /* ───── Scrollbar 조작 시 호출 ───── */
    private void OnBarChanged(float barValue)
    {
        if (selfUpdate) return; // 내부 업데이트면 무시

        float mapped = Mathf.Lerp(min, max, barValue);
        PlayerPrefs.SetFloat(PREF_KEY_SENS, mapped);

        SettingsEvents.RaiseLookSensitivityChanged(mapped);
    }

    /* ───── Helper ───── */
    private void SetBarSilently(float value)
    {
        selfUpdate = true;
        scrollbar.value = value;
        selfUpdate = false;
    }

    private void OnDestroy()
    {
        scrollbar.onValueChanged.RemoveListener(OnBarChanged);
        PlayerPrefs.Save(); // 혹시 모를 유실 방지
    }
}
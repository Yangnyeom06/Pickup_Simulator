using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoOption : MonoBehaviour
{
    [Header("UI")]
    public Dropdown resolutionDropdown;
    public Toggle fullscreenBtn;

    [Header("State (readonly in Inspector)")]
    public int resolutionNum;
    public FullScreenMode screenMode;

    private readonly List<Resolution> resolutions = new List<Resolution>();

    private void Start()
    {
        InitUI();

        // ✅ 안전하게 코드에서 이벤트 연결 (인스펙터 연결되어 있어도 중복은 아님)
        resolutionDropdown.onValueChanged.AddListener(DropboxOptionChange);
        fullscreenBtn.onValueChanged.AddListener(FullScreenBtn);
    }

<<<<<<< HEAD


=======
>>>>>>> origin/dev/newUI
    private void InitUI()
    {
        resolutions.Clear();
        resolutions.AddRange(Screen.resolutions); // 모니터 지원 해상도 목록

        resolutionDropdown.options.Clear();

        int currentIndex = 0;
        int i = 0;
        foreach (var r in resolutions)
        {
            var option = new Dropdown.OptionData
            {
                // Unity 2021+: refreshRate, 2022+: refreshRateRatio 둘 다 존재 가능
                text = $"{r.width}x{r.height} {r.refreshRateRatio}Hz"
            };
            resolutionDropdown.options.Add(option);

            if (r.width == Screen.width && r.height == Screen.height)
                currentIndex = i;

            i++;
        }

        // ✅ 현재 해상도로 보이게
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();

        // ✅ 내부 상태도 일치시켜야 OK 클릭 시 엉뚱한 인덱스로 안 감
        resolutionNum = resolutionDropdown.value;

        // ✅ 현재 전체화면 모드로 토글/변수 동기화
        // 전체화면으로 간주할 모드(ExclusiveFullScreen, FullScreenWindow)
        bool isFullscreen = Screen.fullScreenMode == FullScreenMode.FullScreenWindow
                            || Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen;
        fullscreenBtn.isOn = isFullscreen;

        // 내부 모드도 현재 상태로 초기화(사용자가 토글 안 건드려도 안전)
        screenMode = isFullscreen ? Screen.fullScreenMode : FullScreenMode.Windowed;
        // 만약 전체화면을 일괄 FullScreenWindow로 통일하고 싶다면:
        // screenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    // 드롭다운이 바뀌었을 때
    public void DropboxOptionChange(int idx)
    {
        resolutionNum = idx;
          OKBtnClick(); // ✅ OK 버튼 누른 것처럼 바로 적용
    }

    // 토글이 바뀌었을 때
    public void FullScreenBtn(bool isFull)
    {
        // 필요에 따라 ExclusiveFullScreen으로 바꾸면 주사율 적용이 더 잘 먹히는 환경도 있음
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        OKBtnClick(); // ✅ OK 버튼 누른 것처럼 바로 적용
    }

    // OK 버튼 클릭 시 적용
    public void OKBtnClick()
    {
        if (resolutions.Count == 0 || resolutionNum < 0 || resolutionNum >= resolutions.Count)
        {
            Debug.LogWarning("[VideoOption] No resolutions or invalid index.");
            return;
        }

        var sel = resolutions[resolutionNum];

        // ✅ 먼저 전체화면 모드를 명시적으로 설정(일부 환경에서 신뢰성↑)
        Screen.fullScreenMode = screenMode;

        // ✅ 주사율도 함께 넘겨서 적용 신뢰성↑ (Unity overload)
        //    SetResolution(width, height, FullScreenMode, preferredRefreshRate)
        Screen.SetResolution(sel.width, sel.height, screenMode, sel.refreshRateRatio);
        if (Screen.width != sel.width || Screen.height != sel.height)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Screen.SetResolution(sel.width, sel.height, FullScreenMode.ExclusiveFullScreen, sel.refreshRateRatio);


            Debug.Log($"[VideoOption] Applied {sel.width}x{sel.height} {sel.refreshRateRatio}Hz, Mode={screenMode}");
            Debug.Log("[VideoOption] OKBtnClick called");
        }
    }
    
}
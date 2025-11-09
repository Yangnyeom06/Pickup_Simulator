using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
<<<<<<< HEAD
    public static PauseMenu Instance { get; private set; }
=======
>>>>>>> origin/dev/newUI
    [SerializeField] private GameObject go_BaseUI; // 일시 정지 UI 패널
    private CursorControl cursorControl;

    private bool isPause = false;

<<<<<<< HEAD
=======
    void Start()
    {
        cursorControl= GetComponent<CursorControl>();
    }
>>>>>>> origin/dev/newUI

    void Update()
    {        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPause = !isPause;
            if (isPause)
                CallMenu();
            else
                CloseMenu();
        }
    }

    private void CallMenu()
    {
        isPause = true;
<<<<<<< HEAD
        go_BaseUI.SetActive(true);
        cursorControl.CursorVisible(true);
        
=======
        cursorControl.CursorVisible(true);
        go_BaseUI.SetActive(true);
>>>>>>> origin/dev/newUI
        Time.timeScale = 0f; // 시간의 흐름 설정. 0배속. 즉 시간을 멈춤.
    }

    public void CloseMenu()
    {
        isPause = false;
<<<<<<< HEAD
        go_BaseUI.SetActive(false); 
        cursorControl.CursorVisible(false);
        
=======
        cursorControl.CursorVisible(false);
        go_BaseUI.SetActive(false); 
>>>>>>> origin/dev/newUI
        Time.timeScale = 1f; // 1배속 (정상 속도)
    }

    public void ClickSave()
    {
        Debug.Log("세이브");
    }

    public void ClickLoad()
    {
        Debug.Log("로드");
    }

    public void ClickExit()
    {
        Debug.Log("게임 종료");
        Application.Quit();  // 게임 종료 (에디터 상 실행이기 때문에 종료 눌러도 변화 X)
    }
}

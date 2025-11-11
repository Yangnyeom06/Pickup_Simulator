using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool isPause = false; // 일시 정지 메뉴 창 활성화

    public void CursorVisible()
    {
        if (isPause)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (!isPause)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}

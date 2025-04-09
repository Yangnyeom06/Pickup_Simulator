using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpGrade : MonoBehaviour
{
    public PlayerStats playerStats;
    public TextMeshProUGUI aText;
    public TextMeshProUGUI bText;
    public TextMeshProUGUI cText;
    public TextMeshProUGUI dText;
    public TextMeshProUGUI eText;
    public GameObject UpGradeGround;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("스페이스 키 누름");

            Open();
            UpGradeGround.SetActive(true);
        }
    }

    public void Open()
    {
        aText.text = playerStats.a.Value.ToString();
        bText.text = playerStats.b.Value.ToString();
        cText.text = playerStats.c.Value.ToString();
        dText.text = playerStats.d.Value.ToString();
        eText.text = playerStats.e.Value.ToString();

    }

    public void Exit()
    {
        UpGradeGround.SetActive(false);
    }

    public void aStatUpGrade()
    {
        playerStats.a.Value += 1;
        aText.text = playerStats.a.Value.ToString();
    }

    public void bStatUpGrade()
    {
        playerStats.b.Value += 1;
        bText.text = playerStats.b.Value.ToString();
    }

    public void cStatUpGrade()
    {
        playerStats.c.Value += 1;
        cText.text = playerStats.c.Value.ToString();
    }

    public void dStatUpGrade()
    {
        playerStats.d.Value += 1;
        dText.text = playerStats.d.Value.ToString();
    }

    public void eStatUpGrade()
    {
        playerStats.e.Value += 1;
        eText.text = playerStats.e.Value.ToString();
    }
}

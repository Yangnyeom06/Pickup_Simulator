using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VideoOption : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Toggle fullscreenBtn;
    FullScreenMode screenMode;
    public int resolutionNum;
    List<Resolution> resolutions = new List<Resolution>();

    void Start()
    {
        InitUI();
      
       
        
    }

    void InitUI()
    {
        resolutions.AddRange(Screen.resolutions);
        
        resolutionDropdown.options.Clear();

        int optionNum = 0;            
       foreach (Resolution item in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.width + "x" + item.height + " " + item.refreshRateRatio + "hz";
            resolutionDropdown.options.Add(option);

           if (item.width == Screen.width && item.height == Screen.height)
                resolutionDropdown.value = optionNum;
            optionNum++;

         }
      resolutionDropdown.RefreshShownValue();
fullscreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;



            
        }
    public void DropboxOptionChange(int x)
    {
        resolutionNum = x;
        Debug.Log(resolutionNum);
    }
    public void FullScreenBtn(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Debug.Log("FullScreen");
    }

    public void OKBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width,
        resolutions[resolutionNum].height,
        screenMode);

        Debug.Log("save");
}

    
    }




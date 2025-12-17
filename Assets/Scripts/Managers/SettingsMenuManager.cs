using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("Referências da UI")]
    public TMP_Dropdown resDropdown;
    public Toggle fullscreenToggle;

    private Resolution[] uniqueResolutions;
    private bool isInitializing = false; 

    void OnEnable()
    {
        isInitializing = true;

        InitializeResolutionsAndLoad();

        isInitializing = false;
    }

    private void InitializeResolutionsAndLoad()
    {
        Resolution[] allResolutions = Screen.resolutions;
        List<Resolution> filteredList = new List<Resolution>();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        
        for (int i = 0; i < allResolutions.Length; i++)
        {
            Resolution r = allResolutions[i];
            
            bool duplicate = false;
            foreach (var item in filteredList)
            {
                if (item.width == r.width && item.height == r.height)
                {
                    duplicate = true;
                    break;
                }
            }

            if (!duplicate)
            {
                filteredList.Add(r);
                options.Add(r.width + " x " + r.height);

                if (r.width == Screen.width && r.height == Screen.height)
                {
                    currentResolutionIndex = filteredList.Count - 1;
                }
            }
        }

        uniqueResolutions = filteredList.ToArray();

        if (resDropdown != null)
        {
            resDropdown.ClearOptions();
            resDropdown.AddOptions(options);
        }

        int savedIndex = PlayerPrefs.GetInt("ResolutionIndex", -1);
        if (savedIndex == -1) savedIndex = currentResolutionIndex;

        savedIndex = Mathf.Clamp(savedIndex, 0, uniqueResolutions.Length - 1);

        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        if (resDropdown != null) resDropdown.value = savedIndex;
        if (fullscreenToggle != null) fullscreenToggle.isOn = isFullscreen;

        ApplyGraphics(savedIndex, isFullscreen);
    }

    // --- FUNÇÕES DOS BOTÕES ---

    public void SetResolution(int resolutionIndex)
    {
        if (isInitializing) return;

        if (uniqueResolutions == null || resolutionIndex < 0 || resolutionIndex >= uniqueResolutions.Length) 
            return;

        ApplyGraphics(resolutionIndex, fullscreenToggle.isOn);

        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
        
        Debug.Log("Resolução Salva: " + resolutionIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        if (isInitializing) return;

        ApplyGraphics(resDropdown.value, isFullscreen);

        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("Fullscreen Salvo: " + isFullscreen);
    }

    private void ApplyGraphics(int resIndex, bool fullScreenState)
    {
        if (uniqueResolutions == null || uniqueResolutions.Length == 0) return;

        Resolution resolution = uniqueResolutions[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, fullScreenState);
    }
}
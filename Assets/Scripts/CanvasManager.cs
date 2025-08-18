using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CanvasManager : MonoBehaviour
{
    public void OnStartButtonClickedHandler()
    {
        GameManager.Instance.OnStartGameClicked();
        gameObject.SetActive(false);       
    }
    
    public void OnQuitButtonClickedHandler()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    public void OnSettingsButtonClickedHandler()
    {
        Debug.Log("Settings button clicked.");
    }
}
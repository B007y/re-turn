using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class UIHandler : MonoBehaviour
{
    // UI Elements
    [SerializeField] Button escape;
    [SerializeField] Button restart;
    [SerializeField] Image overlay;
    // Whether the UI is on or not
    bool EscapeMenuOpen;
    
    // Keys and key restrictions
    [SerializeField] InputAction ESCKey;
    [SerializeField] float TimerSeconds = 0;
    float Timer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Key input turning on
        ESCKey.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (Timer > 0) { Timer -= Time.deltaTime; }
        // When the esc key is pressed, the escape menu ui will be shown or hidden
        if (ESCKey.IsPressed() && Timer <= 0)
        {
            Timer = TimerSeconds;
            //Debug.Log("Escape key pressed");
            if(!EscapeMenuOpen)
            { 
                escape.gameObject.SetActive(true); 
                restart.gameObject.SetActive(true);
                overlay.gameObject.SetActive(true);
                EscapeMenuOpen = true;
            }
            else
            {
                escape.gameObject.SetActive(false);
                restart.gameObject.SetActive(false);
                overlay.gameObject.SetActive(false);
                EscapeMenuOpen= false;
            }
        }
    }

    // Quitting game button functionality
    public void Escape()
    {
        Application.Quit();
    }

    // Restart button functionality
    public void Restart()
    {
        // TODO : make sure to set the scene properly
        SceneManager.LoadScene(0);
    }
}

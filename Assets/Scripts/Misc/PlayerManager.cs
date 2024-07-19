using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public KartAgent kart;
    public bool isTiming;
    public event EventHandler OnStartTrack;
    public event EventHandler OnEndTrack;

    private bool reload;

    public void InvokeStart()
    {
        OnStartTrack?.Invoke(this, EventArgs.Empty);
    }

    public void InvokeEnd()
    {
        OnEndTrack?.Invoke(this, EventArgs.Empty);
    }

    public void ClickReturn()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void ClickStart()
    {
        if (!reload)
        {
            Time.timeScale = 1;
            reload = true;
        }
        else
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
            reload = false;
        }
    }
}

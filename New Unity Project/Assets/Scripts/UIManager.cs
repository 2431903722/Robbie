using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    static UIManager instance;
    public TextMeshProUGUI orbText, timeText, deathText, gameOver;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);

        instance = this;
    }

    public static void UpdateOrbUI(int orbCount)
    {
        instance.orbText.text = orbCount.ToString();
    }

    public static void UpdateDeathUI(int deathCount)
    {
        instance.deathText.text = deathCount.ToString();
    }

    public static void UpdateTimeUI(float time)
    {
        int miutes = (int)(time / 60);
        float second = time % 60;
        instance.timeText.text = miutes.ToString("00") + ":" + second.ToString("0");
    }

    public static void DisplayGameOver()
    {
        instance.gameOver.enabled = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    SceneFader fader;
    List<Orb> orbs;
    Door lockedDoor;

    float gameTime;
    bool gameIsOver;

    public int orbNum;
    public int deathNum;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        orbs = new List<Orb>();

        instance = this;

        DontDestroyOnLoad(this);
    }

    public static void PlayerDied()
    {
        instance.fader.FadeOut();
        instance.deathNum++;
        UIManager.UpdateDeathUI(instance.deathNum);
        instance.Invoke("RestartScene", 1f);
    }

    public static void RegisterDoor(Door Door)
    {
        instance.lockedDoor = Door;
    }

    public static void RegisterOrb(Orb orb)
    {
        if (!instance.orbs.Contains(orb))
        {
            instance.orbs.Add(orb);
        }
        UIManager.UpdateOrbUI(instance.orbs.Count);
    }

    void RestartScene()
    {
        instance.orbs.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void RegisterSceneFader(SceneFader obj)
    {
        instance.fader = obj;
    }

    public static void PlayerGrabbedOrb(Orb orb)
    {
        if (instance.orbs.Contains(orb))
        {
            instance.orbs.Remove(orb);
        }
        if (instance.orbs.Count == 0)
        {
            instance.lockedDoor.Open();
        }
        UIManager.UpdateOrbUI(instance.orbs.Count);
    }

    public static void PlayerWon()
    {
        instance.gameIsOver = true;
        AudioManager.PlayerWonAudio();
        UIManager.DisplayGameOver();
    }

    public static bool GameOver()
    {
        return instance.gameIsOver;
    }

    private void Update()
    {
        if (gameIsOver)
        {
            return;
        }
        orbNum = instance.orbs.Count;
        gameTime += Time.deltaTime;
        UIManager.UpdateTimeUI(gameTime);
    }
}

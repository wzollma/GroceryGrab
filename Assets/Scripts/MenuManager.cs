using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Image mainMenu;
    [SerializeField] GameObject HUDObjHolder;
    [SerializeField] Image gameOverMenu;
    [SerializeField] float fadeTime;

    public static MenuManager instance;

    private float startAlpha;

    private void Awake()
    {
        instance = this;

        mainMenu.gameObject.SetActive(true);
        HUDObjHolder.SetActive(false);        
        gameOverMenu.gameObject.SetActive(false);
        Time.timeScale = 0;
    }

    public void startGame()
    {
        AudioManager.instance.stopMenuTrack();
        AudioManager.startThemes();
        AudioManager.playTheme(0, true);

        StartCoroutine(fadeColorImage(mainMenu, false, 0));
    }

    public void lose()
    {
        Time.timeScale = 0;
        
        Color prevColor = gameOverMenu.color;
        gameOverMenu.color = new Color(prevColor.r, prevColor.g, prevColor.b, startAlpha);
        StartCoroutine(fadeColorImage(gameOverMenu, true, startAlpha));
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator fadeColorImage(Image img, bool on, float beginAlpha)
    {
        if (on)
            img.gameObject.SetActive(true);
        else
            Time.timeScale = 1;

        float targetAlpha = on ? startAlpha : 0;

        float timer = 0;

        Color prevColor = img.color;

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;

            prevColor = img.color;
            img.color = new Color(prevColor.r, prevColor.g, prevColor.b, Mathf.Lerp(beginAlpha, targetAlpha, timer / fadeTime));

            yield return null;
        }

        img.color = new Color(prevColor.r, prevColor.g, prevColor.b, targetAlpha);

        if (!on)
        {
            img.gameObject.SetActive(false);
            HUDObjHolder.SetActive(true);
        }
    }
}

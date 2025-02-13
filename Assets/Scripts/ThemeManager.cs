using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Подключаем TextMeshPro
using UnityEngine.SceneManagement;

public class ThemeManager : MonoBehaviour
{
    public Color dayThemeColor = Color.white;
    public Color eveningThemeColor = new Color(1f, 0.9f, 0.7f);
    public Color nightThemeColor = new Color(0.1f, 0.1f, 0.1f);

    public Color dayTextColor = Color.black;
    public Color eveningTextColor = new Color(0.54f, 0.27f, 0.07f);
    public Color nightTextColor = Color.white;

    public Color dayBackground2Color = new Color(0.9f, 0.9f, 0.9f);
    public Color eveningBackground2Color = new Color(0.8f, 0.7f, 0.4f);
    public Color nightBackground2Color = new Color(0.2f, 0.2f, 0.2f);

    public AudioClip helldiversTheme;
    private AudioSource audioSource;

    public enum Theme
    {
        Day,
        Evening,
        Night
    }
    private Theme currentTheme;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // Создаём AudioSource
        LoadTheme(); // Загружаем сохранённую тему
        ApplyTheme(); // Применяем тему при старте
    }

    public void SetDayTheme()
    {
        currentTheme = Theme.Day;
        SaveTheme();
        ApplyTheme();
    }

    public void SetEveningTheme()
    {
        currentTheme = Theme.Evening;
        SaveTheme();
        ApplyTheme();
    }

    public void SetNightTheme()
    {
        currentTheme = Theme.Night;
        SaveTheme();
        ApplyTheme();
    }

    public void PlayDemocraticTheme()
    {
        if (helldiversTheme != null && !audioSource.isPlaying)
        {
            audioSource.clip = helldiversTheme;
            audioSource.Play();
        }
    }

    private void ApplyTheme()
    {
        Color backgroundColor = dayThemeColor;
        Color textColor = dayTextColor;
        Color background2Color = dayBackground2Color;

        switch (currentTheme)
        {
            case Theme.Day:
                backgroundColor = dayThemeColor;
                textColor = dayTextColor;
                background2Color = dayBackground2Color;
                break;
            case Theme.Evening:
                backgroundColor = eveningThemeColor;
                textColor = eveningTextColor;
                background2Color = eveningBackground2Color;
                break;
            case Theme.Night:
                backgroundColor = nightThemeColor;
                textColor = nightTextColor;
                background2Color = nightBackground2Color;
                break;
        }

        SetTheme(backgroundColor, textColor, background2Color);
    }

    private void SetTheme(Color backgroundColor, Color textColor, Color background2Color)
    {
        foreach (GameObject background in GameObject.FindGameObjectsWithTag("Background"))
        {
            if (background.TryGetComponent(out Image image))
            {
                image.color = backgroundColor;
            }
        }

        foreach (GameObject background2 in GameObject.FindGameObjectsWithTag("Background2"))
        {
            if (background2.TryGetComponent(out Image image))
            {
                image.color = background2Color;
            }
        }

        foreach (GameObject textObject in GameObject.FindGameObjectsWithTag("Text"))
        {
            if (textObject.TryGetComponent(out Text uiText))
            {
                uiText.color = textColor;
            }
            else if (textObject.TryGetComponent(out TextMeshProUGUI tmpText))
            {
                tmpText.color = textColor;
            }
        }
    }

    private void SaveTheme()
    {
        PlayerPrefs.SetInt("Theme", (int)currentTheme);
        PlayerPrefs.Save();
    }

    private void LoadTheme()
    {
        if (PlayerPrefs.HasKey("Theme"))
        {
            currentTheme = (Theme)PlayerPrefs.GetInt("Theme");
        }
        else
        {
            currentTheme = Theme.Night; // Ночная тема по умолчанию
        }
    }
}

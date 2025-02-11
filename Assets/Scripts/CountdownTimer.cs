using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; 

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float timeRemaining = 20f;
    private bool isCounting = true;
    public AudioSource gameOverSound;

    public GameObject gameOverImage;
    public Button restartButton; 

    void Start()
    {
        if (timerText == null)
        {
            Debug.LogError("No se asignó un TextMeshProUGUI al contador.");
            return;
        }

        timerText.color = Color.green;
        StartCoroutine(Countdown());

        if (gameOverImage != null)
        {
            gameOverImage.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false); 
            restartButton.onClick.AddListener(RestartGame); 
        }
    }

    IEnumerator Countdown()
    {
        isCounting = true;
        while (timeRemaining > 0 && isCounting)
        {
            UpdateTimerDisplay();
            yield return new WaitForSeconds(1f);
            timeRemaining--;
        }

        if (timeRemaining <= 0)
        {
            GameOver();
        }
    }

    void UpdateTimerDisplay()
    {
        timerText.text = timeRemaining.ToString("0");

        if (timeRemaining <= 10)
        {
            timerText.color = Color.red;
        }
        else if (timeRemaining <= 15)
        {
            timerText.color = new Color(1f, 0.5f, 0f);
        }
    }

    void GameOver()
    {
        if (gameOverImage != null)
        {
            gameOverImage.SetActive(true);
        }

        if (gameOverSound != null)
        {
            gameOverSound.Play();
        }

        timerText.gameObject.SetActive(false);
        isCounting = false;

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true); 
        }
    }

    public void StopTimer()
    {
        isCounting = false;
    }

    public void ResetTimer()
    {
        StopCoroutine(Countdown());
        timeRemaining = 20f;
        timerText.color = Color.green;
        StartCoroutine(Countdown());
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }
}

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 

public class QuizGame : MonoBehaviour
{
    public CountdownTimer countdownTimer;
    public GameObject gameOverImage;
    public Button restartButton; 

    public AudioSource gameOverSound;
    public AudioSource aplausosSound;
    public AudioSource CorrectSound;
    public AudioSource WrongSound;
    public AudioSource RewardSound;

    public GameObject[] rewardObjects;
    public Transform[] spawnPoints;

    private List<int> usedRewards = new List<int>();
    private List<int> usedSpawnPoints = new List<int>();

    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] answers;
        public int correctAnswerIndex;
    }

    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public TextMeshProUGUI scoreText;
    public Question[] questions;

    private int currentQuestionIndex = 0;
    private int score = 0;
    private bool isAnswered = false;

    void Start()
    {
        UpdateScore();
        DisplayQuestion();

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

    void DisplayQuestion()
    {
        isAnswered = false;
        questionText.text = questions[currentQuestionIndex].questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = questions[currentQuestionIndex].answers[i];
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
            answerButtons[i].image.color = Color.white;
            answerButtons[i].gameObject.SetActive(true);
        }
    }

    void CheckAnswer(int selectedIndex)
    {
        if (isAnswered) return;
        isAnswered = true;

        countdownTimer.StopTimer();

        int correctIndex = questions[currentQuestionIndex].correctAnswerIndex;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].image.color = (i == correctIndex) ? Color.green : Color.red;
        }

        if (selectedIndex == correctIndex)
        {
            score++;
            SpawnReward();
            CorrectSound.Play();
            RewardSound.Play();
        }
        else
        {
            if (score - 1 >= 0)
            {
                WrongSound.Play();
            }
            score--;
        }

        UpdateScore();

        if (score < 0)
        {
            GameOver();
            return;
        }

        StartCoroutine(NextQuestion());
    }


    void SpawnReward()
    {
        if (rewardObjects.Length > 0 && spawnPoints.Length > 0)
        {
            if (usedRewards.Count == rewardObjects.Length)
            {
                Debug.LogWarning("⚠ Todas las recompensas ya se han utilizado.");
                return;
            }

            int randomRewardIndex;
            do
            {
                randomRewardIndex = Random.Range(0, rewardObjects.Length);
            } while (usedRewards.Contains(randomRewardIndex));

            usedRewards.Add(randomRewardIndex);

            if (usedSpawnPoints.Count == spawnPoints.Length)
            {
                Debug.LogWarning("⚠ Todos los puntos de spawn ya se han utilizado.");
                return;
            }

            int randomSpawnIndex;
            do
            {
                randomSpawnIndex = Random.Range(0, spawnPoints.Length);
            } while (usedSpawnPoints.Contains(randomSpawnIndex));

            usedSpawnPoints.Add(randomSpawnIndex);

            Instantiate(rewardObjects[randomRewardIndex], spawnPoints[randomSpawnIndex].position, rewardObjects[randomRewardIndex].transform.rotation);
        }
        else
        {
            Debug.LogWarning("⚠ No hay objetos de recompensa o puntos de aparición asignados.");
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

        questionText.text = "Game Over! Puntuación: " + score;
        questionText.gameObject.SetActive(true);

        scoreText.gameObject.SetActive(false);
        countdownTimer.gameObject.SetActive(false);

        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true); 
        }
    }

    IEnumerator NextQuestion()
    {
        yield return new WaitForSeconds(3);

        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }

        currentQuestionIndex++;

        if (currentQuestionIndex < questions.Length)
        {
            questionText.gameObject.SetActive(true);
            DisplayQuestion();
            countdownTimer.ResetTimer();
        }
        else
        {
            questionText.text = "Fin del Juego! Puntuacion: " + score;
            questionText.gameObject.SetActive(true);

            aplausosSound.Play();

            scoreText.gameObject.SetActive(false);
            countdownTimer.gameObject.SetActive(false);

            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(true);
            }
        }
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

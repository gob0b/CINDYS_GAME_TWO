using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SliderChallengeGame : MonoBehaviour
{
    [Header("Game Settings")]
    public float timeLimit = 10f; // Time limit in seconds
    public int requiredSwipes = 5; // Number of up-down cycles required

    [Header("Scene Settings")]
    public string winScene = "WinScene";
    public string loseScene = "LoseScene";

    [Header("UI Elements")]
    public Slider slider; // Single slider for up-down movement
    public Text countdownText; // UI text for countdown
    public Scrollbar timerBar; // UI Scrollbar for the timer

    [Header("Audio Settings")]
    public AudioClip upSound; // Sound for sliding up
    public AudioClip downSound; // Sound for sliding down
    private AudioSource audioSource;

    private float timer;
    private bool gameActive = false;
    private int swipeCount = 0;
    private bool movingUp = true; // Track slider movement direction

    void Start()
    {
        timer = timeLimit;
        timerBar.size = 1f; // Ensure the timer bar starts full
        slider.interactable = false; // Disable slider initially
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        int countdown = 3;
        while (countdown > 0)
        {
            countdownText.text = "" + countdown;
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
        StartGame();
    }

    void StartGame()
    {
        gameActive = true;
        slider.interactable = true; // Enable slider once the game starts
    }

    void Update()
    {
        if (gameActive)
        {
            timer -= Time.deltaTime;
            timerBar.size = Mathf.Clamp01(timer / timeLimit);

            CheckSliderMovement();

            if (swipeCount >= requiredSwipes)
            {
                SceneManager.LoadScene(winScene); // Instantly load win scene
            }
            else if (timer <= 0)
            {
                SceneManager.LoadScene(loseScene); // Lose if time runs out
            }
        }
    }

    void CheckSliderMovement()
    {
        if (movingUp && slider.value >= slider.maxValue)
        {
            audioSource.PlayOneShot(upSound);
            movingUp = false;
        }
        else if (!movingUp && slider.value <= slider.minValue)
        {
            audioSource.PlayOneShot(downSound);
            movingUp = true;
            swipeCount++;
        }
    }
}


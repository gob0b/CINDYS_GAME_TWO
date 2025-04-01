using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TugOfWarGame : MonoBehaviour
{
    [Header("Game Settings")]
    public float timeLimit = 10f; // Time limit in seconds
    public float opponentSpeed = 0.01f; // Speed at which the opponent pulls the slider

    [Header("Scene Settings")]
    public string winScene = "WinScene";
    public string loseScene = "LoseScene";

    [Header("UI Elements")]
    public Slider slider; // Slider for tug of war
    public Text countdownText; // UI text for countdown
    public Scrollbar timerBar; // UI Scrollbar for the timer

    [Header("Audio Settings")]
    public AudioClip spacebarPressSound; // Sound for pressing spacebar
    private AudioSource audioSource;

    private float timer;
    private bool gameActive = false;
    private float middlePoint;

    void Start()
    {
        timer = timeLimit;
        timerBar.size = 1f; // Ensure the timer bar starts full
        audioSource = GetComponent<AudioSource>();
        slider.value = (slider.maxValue + slider.minValue) / 2; // Start slider in the middle
        middlePoint = slider.value; // Define the middle point
        slider.interactable = false; // Prevent manual interaction
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        int countdown = 3;
        while (countdown > 0)
        {
            countdownText.text = "Starting in: " + countdown;
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
    }

    void Update()
    {
        if (gameActive)
        {
            timer -= Time.deltaTime;
            timerBar.size = Mathf.Clamp01(timer / timeLimit);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                slider.value += 0.1f; // Move slider towards the player's side
                audioSource.PlayOneShot(spacebarPressSound);
            }

            slider.value -= opponentSpeed * Time.deltaTime; // Opponent pulling back

            if (slider.value >= slider.maxValue)
            {
                SceneManager.LoadScene(winScene); // Player wins immediately
            }
            else if (timer <= 0)
            {
                if (slider.value >= middlePoint)
                {
                    SceneManager.LoadScene(winScene); // Win if past middle when time runs out
                }
                else
                {
                    SceneManager.LoadScene(loseScene); // Lose if below middle when time runs out
                }
            }
        }
    }
}





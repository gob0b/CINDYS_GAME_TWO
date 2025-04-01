using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TypewriterText : MonoBehaviour
{
    [Header("UI Settings")]
    public Text displayText;        // Assign your UI Text component
    public string[] texts;          // Array of texts to cycle through

    [Header("Timing Settings")]
    public float startDelay = 1f;    // Delay before the first text starts
    public float typeSpeed = 0.05f;  // Speed of typing (lower = faster)
    public float deleteSpeed = 0.03f;// Speed of deleting (lower = faster)
    public float stayDuration = 1.5f;// How long the text stays before deletion
    public float cursorBlinkRate = 0.5f; // How fast the cursor blinks when text is fully typed

    [Header("Sound Settings")]
    public AudioSource audioSource;  // The audio source to play the sound effect
    public AudioClip typingSound;    // Typing sound effect to play

    private int currentIndex = 0;   // Tracks the current text index
    private bool cursorVisible = true;

    void Start()
    {
        if (texts.Length > 0 && displayText != null && audioSource != null && typingSound != null)
        {
            StartCoroutine(TypeLoop());
        }
        else
        {
            Debug.LogError("Assign all required fields: UI Text, AudioSource, and Typing Sound!");
        }
    }

    IEnumerator TypeLoop()
    {
        yield return new WaitForSeconds(startDelay); // Initial delay

        while (true) // Infinite loop
        {
            string fullText = texts[currentIndex]; // Get the current text

            // Type out the text with a cursor
            for (int i = 0; i <= fullText.Length; i++)
            {
                displayText.text = fullText.Substring(0, i) + "|"; // Add cursor
                PlayTypingSound(); // Play the typing sound
                yield return new WaitForSeconds(typeSpeed);
            }

            // Blink cursor while waiting
            StartCoroutine(CursorBlink(fullText));

            yield return new WaitForSeconds(stayDuration);

            // Stop blinking before deletion starts
            StopCoroutine(nameof(CursorBlink));

            // Delete the text
            for (int i = fullText.Length; i >= 0; i--)
            {
                displayText.text = fullText.Substring(0, i) + "|"; // Keep cursor
                yield return new WaitForSeconds(deleteSpeed);
            }

            displayText.text = ""; // Clear text before the next cycle
            yield return new WaitForSeconds(0.5f); // Small delay before next text

            // Move to the next text
            currentIndex = (currentIndex + 1) % texts.Length; // Loops back after the last text
        }
    }

    IEnumerator CursorBlink(string text)
    {
        while (true)
        {
            cursorVisible = !cursorVisible;
            displayText.text = cursorVisible ? text + "|" : text; // Toggle cursor
            yield return new WaitForSeconds(cursorBlinkRate);
        }
    }

    void PlayTypingSound()
    {
        if (audioSource && typingSound)
        {
            audioSource.PlayOneShot(typingSound); // Play the typing sound
        }
    }
}


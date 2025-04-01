using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{
    public GameObject buttonObject; // The GameObject that acts as the button
    public GameObject[] panels; // Array of panels to cycle through
    public Image staticImage; // UI Image to apply the static effect
    public Material staticMaterial; // Material with the moving static shader
    public float transitionDuration = 1f; // Duration for panel transition
    public float fadeDuration = 1f; // Duration for fading between panels
    public ButtonAnimator buttonAnimator; // Reference to ButtonAnimator script

    private int currentPanelIndex = 0; // Index of the current panel

    void Start()
    {
        // Ensure only the first panel is visible, and the rest are hidden
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == 0); // Activate only the first panel
            SetPanelInteractable(panels[i], i == 0); // Only first panel is interactable
        }

        // Set up the static image and initially hide it
        if (staticImage != null && staticMaterial != null)
        {
            staticImage.material = staticMaterial;
            staticImage.enabled = false; // Initially hide the static effect
        }
    }

    void Update()
    {
        // Check if the Space key is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayButtonAnimation();
        }
    }

    // Play button animation and start the panel transition
    private void PlayButtonAnimation()
    {
        if (buttonAnimator != null)
        {
            buttonAnimator.Buttonin();
        }

        StartCoroutine(CyclePanels());
    }

    // Coroutine to handle panel transitions
    private IEnumerator CyclePanels()
    {
        // Fade in the static image before switching panels
        if (staticImage != null)
        {
            staticImage.enabled = true;
            yield return FadeImage(staticImage, 0f, 1f, transitionDuration * 0.5f);
        }

        // Fade out the current panel and disable interaction
        if (panels.Length > 0)
        {
            yield return FadePanel(panels[currentPanelIndex], 1f, 0f, transitionDuration * 0.5f);
            SetPanelInteractable(panels[currentPanelIndex], false);
            panels[currentPanelIndex].SetActive(false);
        }

        yield return new WaitForSeconds(transitionDuration * 0.1f); // Small delay

        // Move to the next panel (looping back when at the last panel)
        currentPanelIndex = (currentPanelIndex + 1) % panels.Length;

        // Activate and fade in the next panel
        if (panels.Length > 0)
        {
            panels[currentPanelIndex].SetActive(true);
            SetPanelInteractable(panels[currentPanelIndex], true);
            yield return FadePanel(panels[currentPanelIndex], 0f, 1f, transitionDuration * 0.5f);
        }

        // Fade out the static image after transitioning
        if (staticImage != null)
        {
            yield return FadeImage(staticImage, 1f, 0f, transitionDuration * 0.5f);
            staticImage.enabled = false;
        }
    }

    // Helper method to fade an image
    private IEnumerator FadeImage(Image img, float startAlpha, float endAlpha, float duration)
    {
        float timeElapsed = 0f;
        Color startColor = img.color;
        startColor.a = startAlpha;
        img.color = startColor;

        Color endColor = img.color;
        endColor.a = endAlpha;

        while (timeElapsed < duration)
        {
            img.color = Color.Lerp(startColor, endColor, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        img.color = endColor; // Ensure final alpha is set
    }

    // Helper method to fade a panel
    private IEnumerator FadePanel(GameObject panel, float startAlpha, float endAlpha, float duration)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = startAlpha;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha; // Ensure final alpha is set
    }

    // Helper method to enable/disable interaction on a panel
    private void SetPanelInteractable(GameObject panel, bool interactable)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }

        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
    }
}
















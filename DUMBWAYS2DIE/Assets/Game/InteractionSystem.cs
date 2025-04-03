using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{
    [Header("General Settings")]
    public GameObject[] panels;          // Panels to cycle through
    public float transitionDuration = 1f; // Duration for panel transitions
    public ButtonAnimator[] buttonAnimators; // Button animations for button press
    public AudioSource audioSource;      // Audio source for button press sound
    public GameObject canvas;            // Canvas to show/hide
    public Light sceneLight;             // Light to control visibility
    public GameObject objectToAnimate;   // GameObject to animate
    public string animationTrigger = "PlayAnimation"; // Animation trigger name

    [Header("Static Image Effect")]
    public Image staticImage;            // Image to apply static effect
    public Material staticMaterial;      // Material with static shader
    public float staticTransitionDuration = 0.5f; // Duration of the static effect

    [Header("Image Sequence Settings")]
    public GameObject imageSequencePanel;  // Panel to display image sequence
    public Sprite[] imageSequence;         // Array of images for the sequence
    public float imageDisplayTime = 3f;    // Time each image stays visible (adjustable)
    public float fadeDuration = 0.5f;     // Duration of fade transition between images
    private int currentImageIndex = 0;     // Tracks the current image in the sequence

    [Header("Overlay Filter")]
    public Image overlayImage;           // Image to serve as overlay filter

    private int currentPanelIndex = 0;     // Index of the current panel
    private bool hasPressedE = false;      // Prevents multiple presses of E

    void Start()
    {
        // Initialize panels and set the first panel active
        InitializePanels();

        // Initialize the static image effect
        InitializeStaticImage();

        // Hide the canvas and set initial conditions
        canvas.SetActive(false);
        sceneLight.enabled = false;
        imageSequencePanel.SetActive(false); // Hide image sequence panel initially

        // Ensure overlay image is active and is on top of everything else
        if (overlayImage != null)
        {
            overlayImage.gameObject.SetActive(true);
            overlayImage.rectTransform.SetAsLastSibling(); // Make sure it overlays everything
        }

        // Ensure overlay image is part of the canvas and set its sorting order to a higher value
        Canvas canvasComponent = overlayImage.GetComponentInParent<Canvas>();
        if (canvasComponent != null)
        {
            canvasComponent.sortingOrder = 10; // Set sorting order to make sure it renders on top
        }
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Trigger button animations on pressing Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayButtonAnimation();
        }

        // Toggle canvas and light on pressing E
        if (Input.GetKeyDown(KeyCode.E) && !hasPressedE)
        {
            ToggleCanvasAndLight();
            PlayObjectAnimation();
            hasPressedE = true;
        }

        // If image sequence panel is active, start cycling through images
        if (imageSequencePanel.activeSelf)
        {
            StartCoroutine(SwitchImageSequence());
        }
    }

    private void InitializePanels()
    {
        // Set only the first panel active and others inactive
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == 0);
            SetPanelInteractable(panels[i], i == 0);
        }
    }

    private void InitializeStaticImage()
    {
        // Set up the static image effect (if any)
        if (staticImage != null && staticMaterial != null)
        {
            staticImage.material = staticMaterial;
            staticImage.enabled = false; // Initially hidden
        }
    }

    private void PlayButtonAnimation()
    {
        // Play the animation for the current button
        if (buttonAnimators.Length > currentPanelIndex && buttonAnimators[currentPanelIndex] != null)
        {
            buttonAnimators[currentPanelIndex].Buttonin();
        }

        // Play sound if any assigned
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Transition panels
        StartCoroutine(CyclePanels());
    }

    private IEnumerator CyclePanels()
    {
        // Fade in static image effect before panel switch
        if (staticImage != null)
        {
            staticImage.enabled = true;
            yield return FadeImage(staticImage, 0f, 1f, staticTransitionDuration);
        }

        // Fade out the current panel and deactivate it
        if (panels.Length > 0)
        {
            yield return FadePanel(panels[currentPanelIndex], 1f, 0f, transitionDuration);
            SetPanelInteractable(panels[currentPanelIndex], false);
            panels[currentPanelIndex].SetActive(false);
        }

        // Wait a short time before switching to the next panel
        yield return new WaitForSeconds(transitionDuration * 0.1f);

        // Move to the next panel
        currentPanelIndex = (currentPanelIndex + 1) % panels.Length;

        // Activate the next panel and fade it in
        if (panels.Length > 0)
        {
            panels[currentPanelIndex].SetActive(true);
            SetPanelInteractable(panels[currentPanelIndex], true);
            yield return FadePanel(panels[currentPanelIndex], 0f, 1f, transitionDuration);
        }

        // Fade out the static image effect
        if (staticImage != null)
        {
            yield return FadeImage(staticImage, 1f, 0f, staticTransitionDuration);
            staticImage.enabled = false;
        }
    }

    private IEnumerator SwitchImageSequence()
    {
        if (imageSequence.Length == 0) yield break;

        Image imageComponent = imageSequencePanel.GetComponent<Image>();

        // Fade out current image and wait
        yield return FadeImage(imageComponent, 1f, 0f, fadeDuration); // Fade out the image before switching

        // Show current image
        imageComponent.sprite = imageSequence[currentImageIndex];
        yield return FadeImage(imageComponent, 0f, 1f, fadeDuration); // Fade in the new image

        // Wait for the specified duration before switching
        yield return new WaitForSeconds(imageDisplayTime);

        // Move to the next image (loop back to the start if necessary)
        currentImageIndex = (currentImageIndex + 1) % imageSequence.Length;
    }

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

    private void ToggleCanvasAndLight()
    {
        // Toggle canvas visibility
        canvas.SetActive(true);

        // Turn the light on if it's assigned
        if (sceneLight != null)
        {
            sceneLight.enabled = true;
        }
    }

    private void PlayObjectAnimation()
    {
        // Trigger the animation on a specified object
        if (objectToAnimate != null)
        {
            Animator animator = objectToAnimate.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger(animationTrigger);
            }
        }
    }
}




























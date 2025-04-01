using UnityEngine;

public class ButtonAnimator : MonoBehaviour
{
    public Animator buttonPressAnimator; // Animator component for the button

    // Method to trigger the "ButtonPress" animation
    public void Buttonin()
    {
        if (buttonPressAnimator != null)
        {
            buttonPressAnimator.SetTrigger("ButtonPress"); // Triggers the animation on the button
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

public class Weqffa : MonoBehaviour
{
    public AudioSource mySound;
    public AudioClip ClickButton;

    public void ClickSound()
    {
        mySound.clip = ClickButton; // Set the clip to play
        mySound.Play(); // Play the sound
    }
}
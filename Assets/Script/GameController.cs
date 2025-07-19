using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required to work with UI elements like sliders

// The GameController class manages game progress and updates the progress slider
public class GameController : MonoBehaviour
{
    int progressAmount;             // Keeps track of the current progress value
    public Slider progressSlider;   // Reference to UI slider to show progress

    void Start()
    {
        progressAmount = 0;         
        progressSlider.value = 0;   // Set the slider to 0 at the start

        // Subscribe the IncreaseProgressAmount method to the OnGemCollect event from the Gem class
        Gem.OnGemCollect += IncreaseProgressAmount;
    }

    // This method increases the progress when a gem is collected
    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;                // Add the amount to the current progress
        progressSlider.value = progressAmount;   // Update the slider to match the new progress

        if (progressAmount >= 100)
        {
            Debug.Log("Level Complete");
        }
    }

}

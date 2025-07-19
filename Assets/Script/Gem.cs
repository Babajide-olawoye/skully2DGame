using System;
using UnityEngine;

// Represents a collectible gem in the game.
// When collected, it triggers an event and is destroyed.
public class Gem : MonoBehaviour, IITEM
{
    // A static event that notifies listeners when a gem is collected.
    // It passes an integer (the gem's worth) to its list of methods
    public static event Action<int> OnGemCollect;

    // The value (or worth) of this gem. Default is 5.
    public int worth = 5;

    // This method is called when the gem is collected by the player.
    public void Collect()
    {
        // Trigger the OnGemCollect event, passing the gem's worth.
        OnGemCollect?.Invoke(worth);

        // Remove the gem from the scene after it is collected.
        Destroy(gameObject);
    }
}

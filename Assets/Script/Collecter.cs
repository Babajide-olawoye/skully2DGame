using UnityEngine;

// Collector script gives object ability to piclup items like a player
// or an item collector area (with a trigger collider).
public class Collecter : MonoBehaviour
{
    // Unity automatically calls this method when another collider enters the trigger
    public void OnTriggerEnter2D(Collider2D collider2D)
    {
        // Looks for other obj with IItem interface
        IITEM item = collider2D.GetComponent<IITEM>();

        // If the other object find IITEM call its Collect() method
        if (item != null)
        {
            item.Collect();
        }
    }
}

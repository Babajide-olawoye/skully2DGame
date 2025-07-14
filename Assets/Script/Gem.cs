using UnityEngine;

// This class represents a Gem obj.
// It implements the IITEM interface, so it can be collected.
public class Gem : MonoBehaviour, IITEM
{
    // Destroys the GameObject.
    public void Collect()
    {
        Destroy(gameObject);
    }
}

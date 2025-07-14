using UnityEngine;

public class Gem : MonoBehaviour, IITEM
{
    public void Collect()
    {
        Destroy(gameObject);
    }
}

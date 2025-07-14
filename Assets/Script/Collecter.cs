using UnityEngine;

public class Collecter : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collider2D)
    {
        IITEM item = collider2D.GetComponent<IITEM>();
        if (item != null)
        {
            item.Collect();
        }
    }
}

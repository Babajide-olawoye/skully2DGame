using UnityEngine;

// This is an item interface. Give to obj that wants
// to be define as an item
public interface IITEM
{
    // Method to be called when the item is collected
    public void Collect();
}

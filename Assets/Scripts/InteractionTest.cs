using UnityEngine;

public class InteractionTest : MonoBehaviour
{
    public void OnInteract()
    {
        Debug.Log($"{gameObject.name} was interacted with.");
    }
}

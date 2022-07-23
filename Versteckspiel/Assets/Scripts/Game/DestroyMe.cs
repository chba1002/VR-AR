using UnityEngine;

public class DestroyMe : MonoBehaviour
{
    public void DestroyThisObject()
    {
        Destroy(this.gameObject);
    }
}

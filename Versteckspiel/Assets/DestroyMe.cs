using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMe : MonoBehaviour
{
 
    public void DestroyThisObject()
    {
        Destroy(this.gameObject);
    }
}

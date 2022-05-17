using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImagePool : MonoBehaviour
{
    public PlayerMovement myPlayerMovementScript;
    public GameObject prefab;
    public int poolSize = 10;
    public List<AfterImage> afterImages;

    public int interval = 10;

    public int time = 0;

    // Start is called before the first frame update
    void Start()
    {
        myPlayerMovementScript = transform.root.GetComponent<PlayerMovement>();
        Debug.Log("Start After Image Pool");
        afterImages = new List<AfterImage>(poolSize);
        for (int i = 0; i < poolSize; i++)
        {
            GameObject nextAfterImage = Instantiate(prefab);
            afterImages.Add(nextAfterImage.GetComponent<AfterImage>());
            nextAfterImage.GetComponent<AfterImage>().targetObject = myPlayerMovementScript.Moth;
            nextAfterImage.GetComponent<AfterImage>().targetAnimator = myPlayerMovementScript.animator;
        }

    }

    // Update is called once per frame
    void Update()
    {
        time++;
        if (time > interval)
        {
            time = 0;
            AddAfterImage();
        }
    }

    void AddAfterImage()
    {
        for (int i = 0; i < poolSize; i++)
        {
            if (!afterImages[i].active)
            {
                afterImages[i].Activate(); break;
            }
        }
    }
}

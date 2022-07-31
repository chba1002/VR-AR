using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessExecutor : MonoBehaviour
{
    public int ActivePostProcessing = 0;

    [SerializeField]
    private PostProcessLayer postProcessLayer;

    private int lastActivePostProcessing;

    private float remainingPostProcessingDurationInSeconds = 0;
    private bool postProcessingIsActive = false;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(postProcessLayer.volumeLayer.value);

        lastActivePostProcessing = ActivePostProcessing;
    }

    // Update is called once per frame
    void Update()
    {
        //ActivePostProvessing = postProcessLayer.volumeLayer.value;

        if(lastActivePostProcessing != ActivePostProcessing)
        {
            Debug.Log("postProcessLayer.volumeLayer.value: " + postProcessLayer.volumeLayer.value);
            postProcessLayer.volumeLayer.value = ActivePostProcessing;
        }

        lastActivePostProcessing = ActivePostProcessing;

        if (postProcessingIsActive)
        {
            if (remainingPostProcessingDurationInSeconds >= 0)
            {
                remainingPostProcessingDurationInSeconds = -Time.deltaTime;
            }
            else
            {
                remainingPostProcessingDurationInSeconds = 0;
                postProcessingIsActive = false;
                ActivePostProcessing = MothBatPostProcessingType.Nothing.GetHashCode();
            }
        }
    }

    public void SetPostProcessing(int durationInSeconds, MothBatPostProcessingType mothBatPostProcessingType)
    {
        if (durationInSeconds <= 0) return;

        postProcessingIsActive = true;
        remainingPostProcessingDurationInSeconds = durationInSeconds;
        ActivePostProcessing = mothBatPostProcessingType.GetHashCode();
    }

    /* Dieses Update kann verwendet werden um die Ids der Post Porcessing Layer herauszufinden
    void Update()
    {
        ActivePostProvessing = postProcessLayer.volumeLayer.value;

        if(lastActivePostProcessing != ActivePostProvessing)
        {
            Debug.Log("postProcessLayer.volumeLayer.value: " + postProcessLayer.volumeLayer.value);
        }

        lastActivePostProcessing = ActivePostProvessing;
    }
    */
}

public enum MothBatPostProcessingType
{
    Nothing = 0,
    Blur = 1,
    MothBatEffect = 64
}

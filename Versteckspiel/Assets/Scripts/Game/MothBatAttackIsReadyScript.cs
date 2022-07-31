using UnityEngine;
using UnityEngine.UI;

public class MothBatAttackIsReadyScript : MonoBehaviour
{
    [SerializeField]
    private Image IsReadyStateImage;

    public bool IsReady { get; private set; } = true;

    /// <summary>
    /// Set the imgage base on the isReady state.
    /// </summary>
    /// <param name="">Number between 0 and 1. 1 == ready.</param>
    public void SetIsReadyInPercent(float readyState)
    {
        readyState = readyState > 1 ? 1 : readyState;
        readyState = readyState < 0 ? 0 : readyState;

        if (readyState < 1)
        {
            IsReady = false;
            // Debug.Log(IsReadyStateImage.color);
            var baseColorIndex = 0.153f;
            baseColorIndex =+ (1 - baseColorIndex) * readyState;
            IsReadyStateImage.color = new Color(baseColorIndex, baseColorIndex, baseColorIndex);
        }
        else
        {
            IsReady = true;
            IsReadyStateImage.color = new Color(1, 1, 1);
        }

        IsReadyStateImage.fillAmount = readyState;
    }
}

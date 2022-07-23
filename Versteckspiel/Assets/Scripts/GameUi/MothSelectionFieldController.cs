using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MothSelectionFieldController : MonoBehaviour
{
    [SerializeField]
    private GameObject image;

    [SerializeField]
    private GameObject text;

    /// <summary>
    /// Initialize the moth filed with a new image and a new Text
    /// </summary>
    /// <param name="newImage">The new moth image.</param>
    /// <param name="newText">The new moth name.</param>
    public void Initialize(Sprite newImage, string newText)
    {
        image.GetComponent<Image>().sprite = newImage;
        text.GetComponent<TextMeshProUGUI>().text = newText;
    }
}

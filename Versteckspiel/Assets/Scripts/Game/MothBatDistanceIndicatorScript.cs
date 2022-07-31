using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MothBatDistanceIndicatorScript : MonoBehaviour
{
    [SerializeField]
    private Image DistanceIndicatorImage;


    [SerializeField]
    private TMP_Text Distance;

    private float maxDistance = 1;
    private float minDistance = 10;


    GameObject _bat = null;

    private GameObject bat
    {
        get
        {
            if (_bat == null) _bat = GameObject.FindGameObjectWithTag("Bat");
            return _bat;
        }
    }

    GameObject moth = null;


    // Start is called before the first frame update
    void Start()
    {
        minDistance = 80; // red
        maxDistance = 200; // white
    }

    public void SetMoth(GameObject moth)
    {
        this.moth = moth;
    }

    // Update is called once per frame
    void Update()
    {
        if(bat != null && moth != null)
        {
            var distance = (bat.transform.position - moth.transform.position).sqrMagnitude;
            Distance.text = distance.ToString();

            var adjustedDistance = distance - minDistance;

            var baseColorIndex = 0f; // 0.153f;

            if(adjustedDistance<= 0)
            {
                DistanceIndicatorImage.color = new Color(1, baseColorIndex, baseColorIndex);
                return;
            }

            baseColorIndex += (1 - baseColorIndex) * distance / maxDistance;
            baseColorIndex = baseColorIndex >= 1 ? 1 : baseColorIndex;

            DistanceIndicatorImage.color = new Color(1, baseColorIndex, baseColorIndex);
        }
        else
        {
            string mothFound = moth == null ? "0" : "1";
            string batFound = bat == null ? "0" : "1";
            Distance.text = $"[moth {mothFound}, bat: {batFound}]";
        }
        
    }
}

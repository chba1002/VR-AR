//
// Sonar FX
//
// Copyright (C) 2013, 2014 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using UnityEngine;
using System.Collections;
using Photon.Pun;
using Assets.Scripts.Shared.Managers;
using Assets.Scripts.Shared.Types;

[RequireComponent(typeof(Camera))]
public class SonarFx : MonoBehaviour
{
    // Sonar mode (directional or spherical)
    public enum SonarMode { Directional, Spherical }
    [SerializeField] SonarMode _mode = SonarMode.Directional;
    public SonarMode mode { get { return _mode; } set { _mode = value; } }

    // Wave direction (used only in the directional mode)
    [SerializeField] Vector3 _direction = Vector3.forward;
    public Vector3 direction { get { return _direction; } set { _direction = value; } }

    // Wave origin (used only in the spherical mode)
    [SerializeField] Vector3 _origin = Vector3.zero;
    public Vector3 origin { get { return _origin; } set { _origin = value; } }

    // Base color (albedo)
    [SerializeField] Color _baseColor = new Color(0.2f, 0.2f, 0.2f, 0);
    public Color baseColor { get { return _baseColor; } set { _baseColor = value; } }

    // Wave color
    [SerializeField] Color _waveColor = new Color(1.0f, 0.2f, 0.2f, 0);
    public Color waveColor { get { return _waveColor; } set { _waveColor = value; } }

    // Wave color amplitude
    [SerializeField] float _waveAmplitude = 2.0f;
    public float waveAmplitude { get { return _waveAmplitude; } set { _waveAmplitude = value; } }

    // Exponent for wave color
    [SerializeField] float _waveExponent = 22.0f;
    public float waveExponent { get { return _waveExponent; } set { _waveExponent = value; } }

    // Interval between waves
    [SerializeField] float _waveInterval = 20.0f;
    public float waveInterval { get { return _waveInterval; } set { _waveInterval = value; } }

    // Wave speed
    [SerializeField] float _waveSpeed = 10.0f;
    public float waveSpeed { get { return _waveSpeed; } set { _waveSpeed = value; } }

    // Additional color (emission)
    [SerializeField] Color _addColor = Color.black;
    public Color addColor { get { return _addColor; } set { _addColor = value; } }

    //public GameObject Motte;

    // Reference to the shader.
    [SerializeField] Shader shader;

    // Private shader variables
    int baseColorID;
    int waveColorID;
    int waveParamsID;
    int waveVectorID;
    int addColorID;

    private float nextActionTime = 3f;
    public float period;
    private PlayerData playerData;
    private PlayerDataProvider playerDataProvider;


    void Awake()
    {
        playerDataProvider = new PlayerDataProvider();
        playerData = playerDataProvider.Provide(PhotonNetwork.LocalPlayer);

        if(playerData?.PlayerMothBatState == null)
        {
            Debug.LogWarning("Moth bat state isn't defined. SonarFx will be deactivated.");
            enabled = false;
            return;
        }
        var localPlayerIsBat = playerData.PlayerMothBatState.MothBatType == MothBatType.Bat.GetHashCode();
        if (!localPlayerIsBat) enabled = false;

        baseColorID = Shader.PropertyToID("_SonarBaseColor");
        waveColorID = Shader.PropertyToID("_SonarWaveColor");
        waveParamsID = Shader.PropertyToID("_SonarWaveParams");
        waveVectorID = Shader.PropertyToID("_SonarWaveVector");
        addColorID = Shader.PropertyToID("_SonarAddColor");
    }

    void OnEnable()
    {
        Update();
    }

    void OnDisable()
    {
        GetComponent<Camera>().ResetReplacementShader();
    }

    void Start()
    {
        //Start the coroutine we define below named SchreiTon.
        StartCoroutine(SchreiTon());
    }

    void Update()
    {
        
        //Schrei schaut nach Position von CenterEyeAnchor im OVRCameraRig, möglicherweise umbenennen, da mehrere Spieler in Szene.
        origin = GameObject.Find("Bat_Network_Player(Clone)/Head").GetComponent<Transform>().position;

        GetComponent<Camera>().SetReplacementShader(shader, null);

        period += Time.deltaTime;
        if (period >= nextActionTime)
        {
            period = 0f;
            print("Fledermaus schreit");
            GameObject.Find("XROrigin/Camera Offset/Main Camera").GetComponent<AudioSource>().Play();
        }

        // Zum Testen mit dem Oculus Controller (hat noch nicht funktioniert...)
        /*if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            print("Fledermaus schreit");
            Echo();
            StartCoroutine(WaveSpeed());
            // Knopf wird gedrückt, 1 Welle entsteht, verschwindet nach Xsec, dann 5sec Pause (Coroutine?), dann wieder frei zum erneuten Drücken
            IEnumerator WaveSpeed()
            {
                yield return new WaitForSeconds(_waveSpeed/5f);
                GetComponent<Camera>().ResetReplacementShader();
            }
        }*/


        Shader.SetGlobalColor(baseColorID, _baseColor);
        Shader.SetGlobalColor(waveColorID, _waveColor);
        Shader.SetGlobalColor(addColorID, _addColor);

        var param = new Vector4(_waveAmplitude, _waveExponent, _waveInterval, _waveSpeed);
        Shader.SetGlobalVector(waveParamsID, param);

        if (_mode == SonarMode.Directional)
        {
            Shader.DisableKeyword("SONAR_SPHERICAL");
            Shader.SetGlobalVector(waveVectorID, _direction.normalized);
        }
        else
        {
            Shader.EnableKeyword("SONAR_SPHERICAL");
            Shader.SetGlobalVector(waveVectorID, _origin);
        }
    }

    IEnumerator SchreiTon()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time); 
        print("Fledermaus schreit");
        GameObject.Find("Bat_Network_Player(Clone)/Head").GetComponent<AudioSource>().Play();
    }

}

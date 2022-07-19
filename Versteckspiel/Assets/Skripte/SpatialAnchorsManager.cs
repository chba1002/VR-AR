using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the spatial anchors of the project
/// </summary>
public class SpatialAnchorsManager : MonoBehaviour
{
    #region Nested classes

    /// <summary>
    /// Data about the
    /// </summary>
    [Serializable]
    public class AnchorData
    {
        /// <summary>
        /// The handle that represents the anchor in this runtime
        /// </summary>
        public ulong spaceHandle;

        /// <summary>
        /// The name of the prefab that should be instantiated for this anchor
        /// </summary>
        public string prefabName;

        /// <summary>
        /// Reference to the gameobject instantiated in scene for this anchor
        /// </summary>
        public GameObject instantiatedObject = null;
    }

    /// <summary>
    /// Data structure used to serialize a <see cref="AnchorData"/> object
    /// </summary>
    [Serializable]
    public class AnchorDataDTO
    {
        /// <summary>
        /// String representation of the unique ID of the anchor
        /// </summary>
        public string spaceUuid;

        /// <summary>
        /// Name of the prefab associated with this anchor
        /// </summary>
        public string prefabName;
    }

    #endregion

    #region Constants

    /// <summary>
    /// Name of the prefab of the first type of objects to spawn
    /// </summary>
    private const string Object1PrefabName = "Object1";

    /// <summary>
    /// Name of the prefab of the second type of objects to spawn
    /// </summary>
    private const string Object2PrefabName = "Object2";

    /// <summary>
    /// Invalid handle for the anchors
    /// </summary>
    public const ulong InvalidHandle = ulong.MaxValue;

    #endregion

    #region Private data

    /// <summary>
    /// Dictionary of created anchors. The space handle ID is the key of the dictionary
    /// </summary>
    private Dictionary<ulong, AnchorData> m_createdAnchors;

    #endregion

    #region MonoBehaviour methods

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        m_createdAnchors = new Dictionary<ulong, AnchorData>();

        //Restore the anchors from the previous session
        LoadAnchorsFromPreviousSession();
    }

    /// <summary>
    /// On Enable
    /// </summary>
    private void OnEnable()
    {
        OVRManager.SpatialEntitySetComponentEnabled += OVRManager_SpatialEntitySetComponentEnabled;
        OVRManager.SpatialEntityQueryResults += OVRManager_SpatialEntityQueryResults;
        OVRManager.SpatialEntityStorageSave += OVRManager_SpatialEntityStorageSave;
    }

    /// <summary>
    /// On Disable
    /// </summary>
    private void OnDisable()
    {
        OVRManager.SpatialEntitySetComponentEnabled -= OVRManager_SpatialEntitySetComponentEnabled;
        OVRManager.SpatialEntityQueryResults -= OVRManager_SpatialEntityQueryResults;
        OVRManager.SpatialEntityStorageSave -= OVRManager_SpatialEntityStorageSave;
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        bool trigger1Pressed = OVRInput.GetDown(OVRInput.GetButtonDown.PrimaryIndexTrigger);
        bool trigger2Pressed = OVRInput.GetDown(OVRInput.GetButtonDown.SecondaryIndexTrigger);
        bool thumbstick1Pressed = OVRInput.GetDown(OVRInput.GetButtonDown.PrimaryThumbstick);
        bool thumbstick2Pressed = OVRInput.GetDown(OVRInput.GetButtonDown.SecondaryThumbstick);

        //if the user has pressed the index trigger on one of the two controllers, generate an object in that position
        if (trigger1Pressed)
            GenerateSpatialAnchor(true);

        if (trigger2Pressed)
            GenerateSpatialAnchor(false);

        //if the user presses any thumbstick, delete all anchors in current session and in persistent storage
        if (thumbstick1Pressed || thumbstick2Pressed)
        {
            EraseAllAnchors();
        }
    }

    /// <summary>
    /// Late update
    /// </summary>
    private void LateUpdate()
    {
        //At every frame we re-assign to the gameobjects the poses of the anchors.
        //We do this because this way we are sure that even if the user recenters or moves the camera rig,
        //the world position of the object remains locked.
        //If we don't execute this late update method, the anchors are correct upon creation, but if the user recenters the camera rig
        //they don't remain fixed in the corresponding world position, but they move as well.

        //for every anchor that has been created
        foreach (var createdAnchorPair in m_createdAnchors)
        {
            //if it is a valid one
            if (createdAnchorPair.Key != InvalidHandle)
            {
                //update its pose
                GenerateOrUpdateGameObjectForAnchor(createdAnchorPair.Key);
            }
        }
    }

    #endregion

    #region Anchors Management methods

    /// <summary>
    /// Generates an object with the same pose of the controller
    /// </summary>
    /// <param name="isLeft">If the controller to take as reference is the left or right one</param>
    private void GenerateSpatialAnchor(bool isLeft)
    {
        //get the pose of the controller in local tracking coordinates
        OVRPose controllerPose = new OVRPose()
        {
            position = OVRInput.GetLocalControllerPosition(isLeft ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch),
            orientation = OVRInput.GetLocalControllerRotation(isLeft ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch)
        };

        //create the information about the spatial anchor (time and position), that should have the same pose of the controller.
        //You can use the below template almost every time
        OVRPlugin.SpatialEntityAnchorCreateInfo createInfo = new OVRPlugin.SpatialEntityAnchorCreateInfo()
        {
            Time = OVRPlugin.GetTimeInSeconds(),
            BaseTracking = OVRPlugin.GetTrackingOriginType(),
            PoseInSpace = controllerPose.ToPosef() //notice that we take the pose in tracking coordinates and convert it from left handed to right handed reference system
        };

        //ask the runtime to create the spatial anchor.
        //The creation is instanteneous, and the identification handle is returned by the runtime inside the "ref" parameter
        ulong spaceHandle = InvalidHandle;

        if (OVRPlugin.SpatialEntityCreateSpatialAnchor(createInfo, ref spaceHandle))
        {
            InitializeSpatialAnchor(spaceHandle, isLeft ? Object1PrefabName : Object2PrefabName);
        }
        else
            Debug.LogError("Creation of spatial anchor failed");
    }

    /// <summary>
    /// Initializes a spatial anchor, creating its components and triggering the generation of the
    /// associated gameobject
    /// </summary>
    /// <param name="spaceHandle">Handle of the spatial anchor</param>
    /// <param name="anchorPrefabName">Name of the prefab to generate with the anchor</param>
    private void InitializeSpatialAnchor(ulong spaceHandle, string anchorPrefabName)
    {
        //add the created anchor to the list of anchors
        m_createdAnchors[spaceHandle] = new AnchorData()
        {
            spaceHandle = spaceHandle,
            prefabName = anchorPrefabName
        };

        //activate the Locatable component so that the anchor can be tracked (otherwise, it is pretty useless).
        //Activate the Storable component so that we can save the anchor to load it at a future execution
        SetAnchorComponent(ref spaceHandle, OVRPlugin.SpatialEntityComponentType.Locatable);
        SetAnchorComponent(ref spaceHandle, OVRPlugin.SpatialEntityComponentType.Storable);
    }

    /// <summary>
    /// Sets a component to an anchor, taking care of all possible cases (e.g. also if the component is already added)
    /// </summary>
    /// <param name="spaceHandle"></param>
    /// <param name="componentType"></param>
    public void SetAnchorComponent(ref ulong spaceHandle, OVRPlugin.SpatialEntityComponentType componentType)
    {
        //we don't care about the request Id for this sample. We so just keep it always at 0.
        //For more complicated applications, it could be useful to identify the callback relative to a particular request
        ulong requestId = 0;

        //We need to send a request to the runtime to enable the component of this anchor, if it is not enabled yet.        
        //So first of all check if it is already enabled. This method returns immediately
        if (OVRPlugin.SpatialEntityGetComponentEnabled(ref spaceHandle, componentType, out bool componentEnabled, out bool changePending))
        {
            //If the component was not enabled, activate it. The operation returns immediately only an error code, but actually the request is anynchronous and gets satisfied by the runtime
            //later in the future. We will get notified about the operation completion with the OVRManager.SpatialEntitySetComponentEnabled event
            if (!componentEnabled)
            {
                if (!OVRPlugin.SpatialEntitySetComponentEnabled(ref spaceHandle, componentType, true, 0, ref requestId))
                    Debug.LogError($"Addition of {componentType.ToString()} component to spatial anchor failed");
            }
            //else if it was already enabled, just call the same callback that the runtime would have called
            //if the set component function succeeded
            else
            {
                OVRManager_SpatialEntitySetComponentEnabled(requestId, true, componentType, spaceHandle);
            }
        }
        else
            Debug.LogError($"Get status of {componentType.ToString()} component to spatial anchor failed");
    }

    /// <summary>
    /// Generates a gameobject to be attached to an anchor, or update its pose for current frame if the anchor has
    /// already a gameobject attached
    /// </summary>
    /// <param name="spaceHandle">Handle to the space anchor</param>
    private void GenerateOrUpdateGameObjectForAnchor(ulong spaceHandle)
    {
        //create the gameobject associated with the anchor, if it didn't exist
        if (m_createdAnchors[spaceHandle].instantiatedObject == null)
            m_createdAnchors[spaceHandle].instantiatedObject = GameObject.Instantiate(Resources.Load<GameObject>(m_createdAnchors[spaceHandle].prefabName));

        //get its pose in world space: at first we get it into headset tracking space,
        //then we convert it to world coordinates
        var anchorPose = OVRPlugin.LocateSpace(ref spaceHandle, OVRPlugin.GetTrackingOriginType());
        var anchorPoseUnity = OVRExtensions.ToWorldSpacePose(anchorPose.ToOVRPose());

        //assign the pose to the object associated to this anchor.
        m_createdAnchors[spaceHandle].instantiatedObject.transform.position = anchorPoseUnity.position;
        m_createdAnchors[spaceHandle].instantiatedObject.transform.rotation = anchorPoseUnity.orientation;
    }

    /// <summary>
    /// Saves an anchor to the persistent storage
    /// </summary>
    /// <param name="spaceHandle">The handle of the anchor to save</param>
    private void SaveAnchor(ulong spaceHandle)
    {
        //we don't care about the request Id for this sample. We so just keep it always at 0.
        //For more complicated applications, it could be useful to identify the callback relative to a particular request
        ulong requestId = 0;

        //request the runtime to save the anchor in some location where it knows how to retrive its info back.
        //We have no control on this process and we will get an answer via the callback associated with OVRManager.SpatialEntityStorageSave
        OVRPlugin.SpatialEntitySaveSpatialEntity(ref spaceHandle, OVRPlugin.SpatialEntityStorageLocation.Local, OVRPlugin.SpatialEntityStoragePersistenceMode.IndefiniteHighPri, ref requestId);
    }

    /// <summary>
    /// Loads the anchors saved in the previous execution of the application, if any
    /// </summary>
    private void LoadAnchorsFromPreviousSession()
    {
        //We have to query the runtime for the anchors saved at previous execution

        //This data structure contains the data of the query. Basically, it means "Give me all the anchors associated with
        //this application that are saved locally on the device".
        //At the time of writing many of the parameters have only one possible choice.
        //Notice that FilterType is None, so we are looking for ALL the anchors associated with this application.
        //In alternative, we can also specify a particular Uuid to have the data only of a particular anghor.
        var queryInfo = new OVRPlugin.SpatialEntityQueryInfo()
        {
            QueryType = OVRPlugin.SpatialEntityQueryType.Action,
            MaxQuerySpaces = 50,
            Timeout = 0,
            Location = OVRPlugin.SpatialEntityStorageLocation.Local,
            ActionType = OVRPlugin.SpatialEntityQueryActionType.Load,
            FilterType = OVRPlugin.SpatialEntityQueryFilterType.None
        };

        //As usual, we don't care about the requestId
        ulong requestId = 0;

        //launch of the query that retrieves all the saved spatial anchors
        if (!OVRPlugin.SpatialEntityQuerySpatialEntity(queryInfo, ref requestId))
        {
            Debug.LogError("Unable to retrieve the spatial anchors saved on the device");
        }
    }

    /// <summary>
    /// Deletes all anchors, in current session and in persistent storage
    /// </summary>
    private void EraseAllAnchors()
    {
        //usual useless request id
        ulong requestId = 0;

        //loop all the created anchors
        foreach (var createdAnchorPair in m_createdAnchors)
        {
            ulong spaceHandle = createdAnchorPair.Key;
            GameObject anchorInstantiatedObject = createdAnchorPair.Value.instantiatedObject;

            //ask the runtime to erase the anchor from the persistent storage
            if (!OVRPlugin.SpatialEntityEraseSpatialEntity(ref spaceHandle, OVRPlugin.SpatialEntityStorageLocation.Local, ref requestId))
                Debug.LogError("Anchor elimination from local storage failed");

            //ask the runtime to destroy the anchor for the current session
            if (!OVRPlugin.DestroySpace(ref spaceHandle))
            {
                Debug.LogError("Anchor elimination from current session failed");
            }

            //destroy the instantiated object, if any
            if (anchorInstantiatedObject != null)
                Destroy(anchorInstantiatedObject);
        }

        //clear the array of created anchors
        m_createdAnchors.Clear();

        //clear all player prefs, so we remove all stale data from it
        PlayerPrefs.DeleteAll();
    }

    #endregion

    #region Spatial Anchors Callbacks

    /// <summary>
    /// Callback called when the addition of a component to a spatial anchor completes successfully
    /// </summary>
    /// <param name="requestId">Request Id passed when the Component Request was issued</param>
    /// <param name="result">Result of the operation</param>
    /// <param name="componentType">Type of component that was requested to add</param>
    /// <param name="spaceHandle">The handle of the space (of the anchor) affected by this request</param>
    private void OVRManager_SpatialEntitySetComponentEnabled(UInt64 requestId, bool result, OVRPlugin.SpatialEntityComponentType componentType, ulong spaceHandle)
    {
        //check the operation completed successfully
        if (result)
        {
            //we should have added the data about the created anchor in the dictionary.
            //If it is not so, abort the operation
            if (!m_createdAnchors.ContainsKey(spaceHandle))
            {
                Debug.LogError("Asked to activate a component on an unknown anchor, aborting");
                return;
            }

            //The anchor has become Locatable, so we can actually spawn an object at its position.
            //Generate an object of the type specified in the dictionary about this anchor, and with the world pose of this anchor
            if (componentType == OVRPlugin.SpatialEntityComponentType.Locatable)
                GenerateOrUpdateGameObjectForAnchor(spaceHandle);
            //The anchor has become Storable, so we can save it to the persistent storage.
            if (componentType == OVRPlugin.SpatialEntityComponentType.Storable)
                SaveAnchor(spaceHandle);

            Debug.Log($"Addition of {componentType.ToString()} component to spatial anchor successfully completed");
        }
        else
            Debug.LogError($"Addition of {componentType.ToString()} component to spatial anchor failed");
    }

    /// <summary>
    /// Callback called when the save operation of the spatial anchor on the persistent storage completes
    /// </summary>
    /// <param name="requestId">Request Id passed when the Component Request was issued</param>
    /// <param name="spaceHandle">The handle of the space (of the anchor) affected by this request</param>
    /// <param name="result">Result of the operation</param>
    /// <param name="uuid">Unique id of the serialized anchor. It is unique of this anchor among all the possible generated anchors and the same in all executions of the program</param>    
    private void OVRManager_SpatialEntityStorageSave(UInt64 requestId, ulong spaceHandle, bool result, OVRPlugin.SpatialEntityUuid uuid)
    {
        //check that the operation succeeded
        if (result)
        {
            //we should have added the data about the created anchor in the dictionary.
            //If it is not so, abort the operation
            if (!m_createdAnchors.ContainsKey(spaceHandle))
            {
                Debug.LogError("Asked to save an unknown anchor, aborting");
                return;
            }

            //We need to save the data about what object was associated this anchor.
            //If we are here, the system has already saved the anchor (included its pose) for future usages in future sessions of this app,
            //but we need to save our custom data associated with the anchor, in this case the gameobject to instantiate on it.
            //We'll save everything in the PlayerPrefs for the sake of clarity of the sample, but we could have also used a file

            //create a serialization structure for the main data of this anchor and then convert it to a json string.            
            AnchorDataDTO spaceAnchorDto = new AnchorDataDTO() { spaceUuid = GetUuidString(uuid), prefabName = m_createdAnchors[spaceHandle].prefabName };
            string spaceAnchorDtoJsonString = JsonConvert.SerializeObject(spaceAnchorDto);

            //save the data about the anchor in the playerprefs, using its unique id as the key
            PlayerPrefs.SetString(spaceAnchorDto.spaceUuid, spaceAnchorDtoJsonString);
        }
        else
            Debug.LogError($"Save operation of spatial anchor failed");
    }

    /// <summary>
    /// Callback called when a query operation on the anchors from the persistent storage completes
    /// </summary>
    /// <param name="requestId">Id of the requests</param>
    /// <param name="numResults">How many anchors have been succesfully obtained from the query</param>
    /// <param name="results">Array of anchors returned by the query. Only the first numResults entries are valid</param>
    private void OVRManager_SpatialEntityQueryResults(ulong requestId, int numResults, OVRPlugin.SpatialEntityQueryResult[] results)
    {
        //for each returned anchor
        for (int i = 0; i < numResults; i++)
        {
            //get its unique Uuid and its handle for the current execution
            //Notice the difference: handle is a number valid only for the current run of the program,
            //while Uuid is always the same among all the possible executions, forever and ever
            var spatialQueryResult = results[i];
            ulong spaceHandle = spatialQueryResult.space;
            string spaceUuid = GetUuidString(spatialQueryResult.uuid);

            //if the anchor is valid
            if (spaceHandle != 0 && spaceHandle != InvalidHandle)
            {
                //Let's obtain the name of the prefab to generate on this anchor from thejson data saved in the player prefs.
                //After we have it, we can initialize the anchor as usual.
                //We use the unique id as the key in the player prefs, since it is the only data unique about an anchor among
                //different executions of the same app
                if (PlayerPrefs.HasKey(spaceUuid))
                {
                    string prefabName = JsonConvert.DeserializeObject<AnchorDataDTO>(PlayerPrefs.GetString(spaceUuid)).prefabName;

                    //initialize the anchor as if the user just created it in this session with his controllers.
                    InitializeSpatialAnchor(spaceHandle, prefabName);
                }
                else
                    Debug.LogError($"Anchor with id {spaceUuid} has no associated info in the PlayerPrefs");
            }
            else
                Debug.LogError($"An invalid anchor has been deserialized");
        }
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Converts a <see cref="OVRPlugin.SpatialEntityUuid"/> object to a string representation
    /// </summary>
    /// <param name="uuid">Unique ID of an anchor</param>
    /// <returns>String representation of the Uuid</returns>
    private string GetUuidString(OVRPlugin.SpatialEntityUuid uuid)
    {
        //Code taken from the Oculus samples
        byte[] uuidData = new byte[16];
        BitConverter.GetBytes(uuid.Value_0).CopyTo(uuidData, 0);
        BitConverter.GetBytes(uuid.Value_1).CopyTo(uuidData, 8);
        return AnchorHelpers.UuidToString(uuidData);
    }

    #endregion
}



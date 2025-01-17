﻿using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// save system
public class SaveSystem : MonoBehaviour
{
    string path;                                        // Path of save
    BinaryFormatter formatter = new BinaryFormatter();  // Formatter used for save & load

    // Objects needed to get and update data from load & save
    RoadPlacer roadPlacer;                              // Road placer object
    [SerializeField] SunManager sunManager;             // Sun object
    [SerializeField] TimeManager timeManager;           // Time object
    [SerializeField] CameraMovement cameraMovement;     // Camera movement object

    [SerializeField] bool firstFrame;

    private void Start()
    {
        roadPlacer = FindObjectOfType<RoadPlacer>();

        path = Application.persistentDataPath + "/" + "Save" + SceneManager.GetActiveScene().name + ".binary";
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void Update()
    {
        //This is needed to fit the roads to the terrain, since the terrain is loaded after the start function
        if (SceneManager.GetActiveScene().name == "SondreScene" || SceneManager.GetActiveScene().name == "PrivateIsland" && !firstFrame)
        {
            firstFrame = true;
            Load();
        }
    }

    // Loads save from file
    public void Load()
    {
        // Ensures load only runs on the island scene
        if (SceneManager.GetActiveScene().name == "PrivateIsland")
        {
            // Load
            if (File.Exists(path))
            {
                // Opens stream
                FileStream stream = new FileStream(path, FileMode.Open);

                try
                {
                    // Loads resources
                    int numResources = (int)formatter.Deserialize(stream);
                    for (int i = 0; i < numResources; i++)
                    {
                        ResourceSave resourceData = formatter.Deserialize(stream) as ResourceSave;

                        // Instantiate resource
                        GameObject resourceObject = Resources.Load("Prefabs/WorldResources/Raw resources/" + RemoveCopyInName(resourceData.objectName)) as GameObject;
                        GameObject worldObject = LoadObject(resourceObject, resourceData.position, resourceData.rotation);

                        ResourceWorldObject resource = worldObject.GetComponentInChildren<ResourceWorldObject>();

                        // ERROR lies here, somehow
                        resource.LoadFromSave(resourceData.resourceAmount);
                    }

                    // Load factories
                    int numFactories = (int)formatter.Deserialize(stream);
                    for (int i = 0; i < numFactories; i++)
                    {
                        FactorySave factoryData = formatter.Deserialize(stream) as FactorySave;

                        // Instantiate factory
                        GameObject factoryObject = Resources.Load("Prefabs/Buildings/Factory/Primary buildings/" + RemoveCopyInName(factoryData.objectName)) as GameObject;
                        GameObject worldObject = LoadObject(factoryObject, factoryData.position, factoryData.rotation);

                        // Updates position, makes sure the building finishes building if it is finished
                        FactoryBuilding factory = worldObject.GetComponent<FactoryBuilding>();
                        factory.LoadFromSave(factoryData.presentHealth, factoryData.buildingFinished, factoryData.yOffset);
                        factory.LoadFactory(factoryData.isWorking, factoryData.remainingTime, factoryData.timeRound, factoryData.index, factoryData.remainingRounds, factoryData.originalRounds);
                    }


                    // Load harvesters
                    int numHarvesters = (int)formatter.Deserialize(stream);
                    for (int i = 0; i < numHarvesters; i++)
                    {
                        BuildingSave harvesterData = formatter.Deserialize(stream) as BuildingSave;

                        // Instantiate harvester
                        GameObject resourceObject = Resources.Load("Prefabs/Buildings/ResourceGathering/" + RemoveCopyInName(harvesterData.objectName)) as GameObject;
                        GameObject worldObject = LoadObject(resourceObject, harvesterData.position, harvesterData.rotation);

                        // Updates position, makes sure the building finishes building if it is finished
                        AbstractResourceHarvesting harvester = worldObject.GetComponent<AbstractResourceHarvesting>();
                        harvester.LoadFromSave(harvesterData.presentHealth, harvesterData.buildingFinished, harvesterData.yOffset);
                    }

                    // Load houses
                    int numHouses = (int)formatter.Deserialize(stream);
                    for (int i = 0; i < numHouses; i++)
                    {
                        BuildingSave houseData = formatter.Deserialize(stream) as BuildingSave;

                        // Instantiate harvester
                        // GameObject resourceObject = Resources.Load("Prefabs/Buildings/" + RemoveCopyInName(houseData.objectName)) as GameObject;
                        GameObject resourceObject = Resources.Load("Prefabs/Buildings/HouseTemplate") as GameObject;
                        GameObject worldObject = LoadObject(resourceObject, houseData.position, houseData.rotation);

                        // Updates position, makes sure the building finishes building if it is finished
                        AbstractHouse house = worldObject.GetComponent<AbstractHouse>();
                        house.LoadFromSave(houseData.presentHealth, houseData.buildingFinished, houseData.yOffset);
                    }

                    // Resource amounts
                    for (int i = 0; i < GameManager.resources.Length; i++)
                    {
                        // Sends in amount of resource
                        GameManager.resources[i].amount = (float)formatter.Deserialize(stream);
                    }

                    // Money
                    GameManager.moneyAmount = (float)formatter.Deserialize(stream);

                    // Roads

                    int numRoads = (int)formatter.Deserialize(stream);
                    roadPlacer.enabled = true;
                    Debug.Log("NumberOfRoads: " + numRoads);

                    for (int i = 0; i < numRoads; i++)
                    {
                        RoadSave roadeData = formatter.Deserialize(stream) as RoadSave;

                        Vector3 startPos = new Vector3(roadeData.startPos_X, roadeData.startPos_Y, roadeData.startPos_Z);
                        Vector3 controllNode1 = new Vector3(roadeData.controllNode1_X, roadeData.controllNode1_Y, roadeData.controllNode1_Z);
                        Vector3 controllNode2 = new Vector3(roadeData.controllNode2_X, roadeData.controllNode2_Y, roadeData.controllNode2_Z);
                        Vector3 endPos = new Vector3(roadeData.endPos_X, roadeData.endPos_Y, roadeData.endPos_Z);
                        
                        roadPlacer.GenerateRoad(startPos, controllNode1, controllNode2, endPos);
                        Debug.Log("LoadingRoads");

                    }
                    roadPlacer.enabled = false;
                }
                catch (System.Exception e)
                {
                    Debug.Log("Error: " + e);
                }

                // Camera position
                Camera.main.transform.position = new Vector3((float)formatter.Deserialize(stream), (float)formatter.Deserialize(stream), (float)formatter.Deserialize(stream));

                // Camera rotation
                if (cameraMovement != null) cameraMovement.LoadAngles((float)formatter.Deserialize(stream), (float)formatter.Deserialize(stream));

                // Loads sun position
                sunManager.UpdatePosition((float[])formatter.Deserialize(stream));

                // Time
                timeManager.UpdateTime((int[])formatter.Deserialize(stream));

                // Camera mode
                GameManager.inputManager.frozenAngle = (bool)formatter.Deserialize(stream);
                Cursor.visible = /*GameManager.isPaused || */ GameManager.inputManager.frozenAngle;
                if (Cursor.visible)
                    Cursor.lockState = CursorLockMode.None;
                else
                    Cursor.lockState = CursorLockMode.Locked;

                // Close stream
                stream.Close();
            }
            else
            {
                Debug.Log("No save file found");
            }
        }
    }

    // Saves save to file
    public void Save()
    {
        // Ensures save only runs on the island scene
        if (SceneManager.GetActiveScene().name == "PrivateIsland")
        {
            GameManager.isPaused = true;                                                                // Pauses game to ensure values does not update needlessly
                                                                                                        // Creates new file or overwrites the old one
            FileStream streamEnsureExists = new FileStream(path, FileMode.Create);
            streamEnsureExists.Close();

            // Opens stream to append to it
            FileStream stream = new FileStream(path, FileMode.Append);

            // Resources
            ResourceWorldObject[] worldResources = FindObjectsOfType<ResourceWorldObject>();            // Gets all resource objects
            formatter.Serialize(stream, worldResources.Length);                                         // Stores number of resources as an int

            for (int i = 0; i < worldResources.Length; i++)
            {
                // Resource
                ResourceSave resourceData = worldResources[i].ReturnResourceSave(worldResources[i].transform.position, worldResources[i].transform.eulerAngles);
                formatter.Serialize(stream, resourceData);
            }

            // Factories
            FactoryBuilding[] factories = FindObjectsOfType<FactoryBuilding>();                         // Gets all resource objects
            formatter.Serialize(stream, factories.Length);                                              // Stores number of factories as an int

            for (int i = 0; i < factories.Length; i++)
            {
                // FactorySave
                FactorySave factory = factories[i].ReturnFactorySave(factories[i].transform.position, factories[i].transform.eulerAngles);
                formatter.Serialize(stream, factory);
            }


            // Resource gatherng buildings
            AbstractResourceHarvesting[] harvesters = FindObjectsOfType<AbstractResourceHarvesting>();  // Gets all resource objects
            formatter.Serialize(stream, harvesters.Length);                                             // Stores number of factories as an int

            for (int i = 0; i < harvesters.Length; i++)
            {
                // FactorySave
                BuildingSave harvester = harvesters[i].ReturnBuildingSave(harvesters[i].transform.position, harvesters[i].transform.eulerAngles);
                formatter.Serialize(stream, harvester);
            }

            // Houses
            AbstractHouse[] houses = FindObjectsOfType<AbstractHouse>();
            formatter.Serialize(stream, houses.Length);                                                 // Stores number of houses as an int

            for (int i = 0; i < houses.Length; i++)
            {
                BuildingSave house = houses[i].ReturnBuildingSave(houses[i].transform.position, houses[i].transform.eulerAngles);
                formatter.Serialize(stream, house);
            }

            // Resource amounts
            for (int i = 0; i < GameManager.resources.Length; i++)
            {
                // Sends in amount of resource
                formatter.Serialize(stream, GameManager.resources[i].amount);
            }

            // Money
            formatter.Serialize(stream, GameManager.moneyAmount);

            //Roads
            RoadStruct[] roads = FindObjectsOfType<RoadStruct>();
            formatter.Serialize(stream, roads.Length);

            for (int i = 0; i < roads.Length; i++)
            {
                formatter.Serialize(stream, roads[i].ReturnRoadSave());
            }

            // Camera position
            formatter.Serialize(stream, Camera.main.transform.position.x);                              // Stores position(x)
            formatter.Serialize(stream, Camera.main.transform.position.y);                              // Stores position(y)
            formatter.Serialize(stream, Camera.main.transform.position.z);                              // Stores position(z)

            // Camera rotation
            formatter.Serialize(stream, Camera.main.transform.eulerAngles.x);                           // Stores rotation(x)
            formatter.Serialize(stream, Camera.main.transform.eulerAngles.y);                           // Stores rotation(y)

            // Sun position
            formatter.Serialize(stream, sunManager.ReturnPosition());                                   // Stores the sun's position

            // Time
            formatter.Serialize(stream, timeManager.ReturnTimeSpeeds());                                // Stores the current and previous time state

            // CameraMode
            formatter.Serialize(stream, cameraMovement.ReturnCameraMode());

            // closes stream
            stream.Close();
        }
    }

    // Removes "Clone" from the end of the name
    string RemoveCopyInName(string name)
    {
        if (name.EndsWith("(Clone)"))
            return name.Substring(0, name.Length - 7);
        else
            return name;
    }

    // send spawned object, position and rotation and get spawned the world object in return
    GameObject LoadObject(GameObject spawnedObject, float[] pos, float[] rot)
    {
        GameObject obj = Instantiate(spawnedObject, FloatsToVectors(pos), Quaternion.Euler(FloatsToVectors(rot)));
        obj.name = spawnedObject.name;
        return obj;
    }

    // Makes array of three floats into Vector3
    Vector3 FloatsToVectors(float[] floats)
    {
        return new Vector3(floats[0], floats[1], floats[2]);
    }
}

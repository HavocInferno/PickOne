using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Custom Editor using SerializedProperties.
public class MapGenerator : MonoBehaviour 
{
    public string seed = "seed";
    public bool useRandomSeed;

    [Header("Map Prefabs")]

    [Tooltip("List of all rooms prefabs to be used for random generation. Rooms usually only connect to corridors.")]
    public List<GameObject> rooms;
    [Tooltip("List of all rooms prefabs to be used for random generation. Corridors usually connect to rooms and junctions.")]
    public List<GameObject> corridors;
    [Tooltip("List of all rooms prefabs to be used for random generation. Junctions usually only connect to corridors.")]
    public List<GameObject> junctions;

    public int numberOfIterations = 5;
    [Range(0, 100)]
    [Tooltip("Chance for an exit to be connected to another map object.")]
    public int connectionChance = 75;

    private System.Random _randomNumberGen;
    private GameObject mapHolder;
    
    public void GenerateMap()
    {
        DestroyImmediate(GameObject.Find("Map"));

        // Create a variable for the map object
        mapHolder = new GameObject();
        mapHolder.name = "Map";

        _InitRandomGenerator();

        // Create a starting point
        Instantiate(_GetRandomRoom(), mapHolder.transform);
        
        //for (int iterIdx = 0; iterIdx < numberOfIterations; iterIdx++)
        //{
        //    _DoIteration(mapHolder);
        //}
    }

    public void PerformIteration()
    {
        if (mapHolder == null)
            return;

        _DoIteration(mapHolder);
    }

    /// <summary>
    /// Initialized the random number generator.
    /// </summary>
    private void _InitRandomGenerator()
    {
        if(useRandomSeed)
        {
            seed = System.DateTime.Now.Ticks.ToString();
        }

        _randomNumberGen = new System.Random(seed.GetHashCode());
    }

    /// <summary>
    /// Fetches a random room from the list of rooms.
    /// </summary>
    private GameObject _GetRandomRoom()
    {
        return rooms[_randomNumberGen.Next(rooms.Count)];
    }

    /// <summary>
    /// Fetches a random corridor from the list of corridors.
    /// </summary>
    private GameObject _GetRandomCorridor()
    {
        return corridors[_randomNumberGen.Next(corridors.Count)];
    }

    /// <summary>
    /// Fetches a random junction from the list of junctions.
    /// </summary>
    private GameObject _GetRandomJunction()
    {
        return junctions[_randomNumberGen.Next(junctions.Count)];
    }

    /// <summary>
    /// Returns a map piece based on the given allowed types.
    /// </summary>
    /// <param name="allowedTypes"></param>
    /// <returns></returns>
    private GameObject _GetRandomMapObject(List<MapObject.ObjectTag> allowedTypes)
    {
        List<GameObject> allowedPieces = new List<GameObject>();

        if (allowedTypes.Contains(MapObject.ObjectTag.Room))
        {
            allowedPieces.AddRange(rooms);
        }

        if (allowedTypes.Contains(MapObject.ObjectTag.Corridor))
        {
            allowedPieces.AddRange(corridors);
        }

        if (allowedTypes.Contains(MapObject.ObjectTag.Junction))
        {
            allowedPieces.AddRange(junctions);
        }

        return allowedPieces[_randomNumberGen.Next(allowedPieces.Count)];
    }

    private void _DoIteration(GameObject mapHolder)
    {
        // Get all active exits
        List<MapExitMarker> availableExits =
            new List<MapExitMarker>(mapHolder.GetComponentsInChildren<MapExitMarker>());

        // Iterate over the exits and connect them
        for (int exitCounter = 0; exitCounter < availableExits.Count; exitCounter++)
        {
            // Don't connect a map object if the probability fails (0 and 100 are exceptions)
            // TODO: Fix connection chances
            if (connectionChance != 100 &&
                (connectionChance == 0 || _randomNumberGen.Next(100) > connectionChance))
            {
                continue;
            }

            // Randomly select an exit and remove it from the list
            MapExitMarker exit = availableExits[_randomNumberGen.Next(availableExits.Count)];
            availableExits.Remove(exit);

            // Instantiate a map object based on the allowed map object type
            GameObject randomMapObject = Instantiate(
                _GetRandomMapObject(exit.GetComponentInParent<MapObject>().allowedConnections),
                mapHolder.transform);

            // TODO: Test whether the object collides with any other map object in the scene
            //bool collidesWithMap = false;
            //for (int mapObjIter = 0; mapObjIter < length; mapObjIter++)
            //{

            //}
            //if (randomMapObject.GetComponent<Collider>().bounds.Intersects())

            // Select an exit on the new object to connect to the available exit
            MapExitMarker[] mapObjectExits = randomMapObject.GetComponentsInChildren<MapExitMarker>();
            MapExitMarker otherExit = mapObjectExits[_randomNumberGen.Next(mapObjectExits.Length)];

            // Disable walls to allow connections
            exit.wallPrefab.SetActive(false);
            otherExit.wallPrefab.SetActive(false);

            // Rotate the map object according to the exit marker
            randomMapObject.transform.rotation = Quaternion.FromToRotation(
                otherExit.transform.forward,
                -exit.transform.forward
                );

            // Move the object so that both exit markers align
            randomMapObject.transform.Translate(
                exit.transform.position - otherExit.transform.position, Space.World);

            // Set the exits as inactive so the are not processed again
            exit.gameObject.SetActive(false);
            otherExit.gameObject.SetActive(false);
        }
    }
}

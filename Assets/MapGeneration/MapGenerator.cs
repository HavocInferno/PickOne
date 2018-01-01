using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class MapGenerator : MonoBehaviour
{
    public int mapWidth = 600,
                mapHeight = 600;
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
    [Tooltip("Chance for an exit to be connected to another map object.")]

    private System.Random _randomNumberGen;
    private GameObject mapHolder;
    private RectInt _mapArea;
    private Color _gizmoColor = new Color(1, 0, 0, 0.13f);

    public void ResetMap()
    {
        DestroyImmediate(GameObject.Find("Map"));

        // Create a variable for the map object
        mapHolder = new GameObject();
        mapHolder.name = "Map";

        _InitRandomGenerator();

        // Create a starting point
        Instantiate(_GetRandomRoom(), mapHolder.transform);
    }

    /// <summary>
    /// A public method to perform a single step of the algorithm.
    /// </summary>
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
        if (useRandomSeed)
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
    private List<GameObject> _GetAllowedMapObjects(List<MapObject.ObjectTag> allowedTypes)
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

        return allowedPieces;
    }

    /// <summary>
    /// Performs a single iteration of the algorithm where it gathers all the available exits
    /// and adds a map piece based on the connection chance.
    /// </summary>
    /// <param name="mapHolder"></param>
    private void _DoIteration(GameObject mapHolder)
    {
        if (mapHolder == null)
        {
            Debug.LogError("Map Holder is a null reference.");
            return;
        }

        // Get all active exits
        List<MapExitMarker> availableExits =
            new List<MapExitMarker>(mapHolder.GetComponentsInChildren<MapExitMarker>());

        // Iterate over the exits and connect them. Loop backwards in order to support removing exits during execution.
        for (int exitCounter = availableExits.Count - 1; exitCounter >= 0; exitCounter--)
        {
            // Don't connect a map object if the probability fails
            if (_randomNumberGen.Next(100) > connectionChance)
            {
                continue;
            }

            // Randomly select an exit
            //MapExitMarker exit = availableExits[_randomNumberGen.Next(availableExits.Count)];

            // Select an exit
            MapExitMarker exit = availableExits[exitCounter];

            // Instantiate a map object based on the allowed map object type
            List<GameObject> allowedObjects = _GetAllowedMapObjects(
                exit.GetComponentInParent<MapObject>().allowedConnections);

            GameObject randomMapObject = Instantiate(
                allowedObjects[_randomNumberGen.Next(allowedObjects.Count)]);

            // Randomly elect an exit on the new object to connect to the available exit
            MapExitMarker[] mapObjectExits = randomMapObject.GetComponentsInChildren<MapExitMarker>();
            MapExitMarker otherExit = mapObjectExits[_randomNumberGen.Next(mapObjectExits.Length)];

            // Rotate the map object according to the exit marker
            // TODO: Make sure the object isn't rotated to be upside down
            randomMapObject.transform.rotation = Quaternion.FromToRotation(
                otherExit.transform.forward,
                -exit.transform.forward
                );

            // If the rotation resulted in the object being upside down, fix it
            // TODO: Remove this redundant rotation
            if (randomMapObject.transform.up != Vector3.up)
            {
                randomMapObject.transform.Rotate(randomMapObject.transform.forward, 180);
            }

            // Move the object so that both exit markers align
            randomMapObject.transform.Translate(
                exit.transform.position - otherExit.transform.position, Space.World);

            // Check if the new object collides with existing map geometry
            if (_IsOutsideMapDimensions(randomMapObject)
                || _CollidesWithExistingMap(randomMapObject))
            {
                DestroyImmediate(randomMapObject);

                // TODO: Perhaps check if any other object can fit instead. If not, 
                // disable this exit for further processing

                continue;
            }

            // Add the object to the map
            randomMapObject.transform.SetParent(mapHolder.transform);

            // Remove this exit so it isn't processed again later
            availableExits.Remove(exit);

            // Connect the exits
            _ConnectExits(exit, otherExit, false);

            // Check if the algorithm generated a loop and allow it to connect
            foreach (var otherExitOfRandObj in randomMapObject.GetComponentsInChildren<MapExitMarker>())
            {
                for (int altExitIdx = availableExits.Count - 1; altExitIdx >= 0; altExitIdx--)
                {
                    var distance = (otherExitOfRandObj.transform.position
                                    - availableExits[altExitIdx].transform.position).magnitude;

                    // If the exit markers are "close enough", they overlap
                    if (distance < 0.1f)
                    {
                        // Connect these exits
                        _ConnectExits(otherExitOfRandObj, availableExits[altExitIdx]);

                        // Remove this exit from the available exits
                        availableExits.RemoveAt(altExitIdx);

                        // Reduce the counter as an extra exit was processed
                        exitCounter--;

                        // As only two exit markers can ever overlap, don't check any others
                        break;
                    }
                }
            }
        }
    }

    private void _AlignTransforms(Transform firstTransform, Transform secondTransform)
    {

    }

    /// <summary>
    /// Connects two exit marker objects by disabling their walls prefabs and their exit marker objects.
    /// </summary>
    /// <param name="firstExit"></param>
    /// <param name="secondExit"></param>
    /// <param name="useChance"></param>
    private void _ConnectExits(MapExitMarker firstExit, MapExitMarker secondExit, bool useChance = true)
    {
        // If chance is not used or connection chance was met then...
        if (!useChance || _randomNumberGen.Next(100) < connectionChance)
        {
            // ... disable walls to allow the connection
            firstExit.wallPrefab.SetActive(false);
            secondExit.wallPrefab.SetActive(false);
        }

        // Set the exits as inactive so the are not processed again
        firstExit.gameObject.SetActive(false);
        secondExit.gameObject.SetActive(false);
    }

    /// <summary>
    /// Checks whether the input map object collides with existing map objects (brute force).
    /// </summary>
    /// <param name="mapObject"></param>
    /// <returns></returns>
    private bool _CollidesWithExistingMap(GameObject mapObject)
    {
        bool isColliding = false;

        var collider = mapObject.GetComponent<Collider>();
        var existingColliders = mapHolder.GetComponentsInChildren<Collider>();

        // TODO: Replace the brute force approach here
        foreach (Collider existingCollider in existingColliders)
        {
            // Create slightly smaller bounds for collision checks
            var reducedBounds = new Bounds(existingCollider.bounds.center,
                                           existingCollider.bounds.size);

            // 0.2f as the amount is overall length rather than from each side
            reducedBounds.Expand(-0.2f);

            if (existingCollider.CompareTag("EditorOnly")
                && collider.bounds.Intersects(reducedBounds))
            {
                isColliding = true;
                break;
            }
        }

        return isColliding;
    }

    /// <summary>
    /// Returns whether the input map object is outside the given map area or not.
    /// </summary>
    /// <param name="mapObject"></param>
    /// <returns></returns>
    private bool _IsOutsideMapDimensions(GameObject mapObject)
    {
        var bounds = mapObject.GetComponent<Collider>().bounds;
        
        return (bounds.max.x > _mapArea.xMax || bounds.min.x < _mapArea.xMin
             || bounds.max.z > _mapArea.yMax || bounds.min.z < _mapArea.yMin);
    }

    /// <summary>
    /// Used to create the mapArea rect iwth apropriate parameters for checking
    /// </summary>
    private void OnValidate()
    {
        _mapArea = new RectInt((int)(-mapWidth * 0.5f), 
            (int)(-mapHeight * 0.5f), mapWidth, mapHeight);

        // TODO: Get map object prefabs automatically

        // Add all room prefabs
        rooms.Clear();

        _AddPrefabsToList(
            "MapGeneration/Rooms",
            rooms);

        // Add all corridor prefabs
        corridors.Clear();

        _AddPrefabsToList(
            "MapGeneration/Corridors",
            corridors);

        // Add all junctions prefabs
        junctions.Clear();

        _AddPrefabsToList(
            "MapGeneration/Junctions",
            junctions);
    }

    /// <summary>
    /// Filters prefabs out of the "prefabsToAdd" list and adds it to the "prefabList".
    /// </summary>
    /// <param name="prefabsToAdd"></param>
    /// <param name="prefabList"></param>
    private void _AddPrefabsToList(string prefabsPath, List<GameObject> prefabList)
    {
        string[] prefabFiles = Directory.GetFiles(
            Application.dataPath + "/" + prefabsPath, 
            "*.prefab", SearchOption.AllDirectories);

        foreach (string prefabFile in prefabFiles)
        {
            string assetPath = "Assets" + prefabFile.Replace(Application.dataPath, "").Replace('\\', '/');
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
            
            if (PrefabUtility.GetPrefabType(prefab) == PrefabType.Prefab)
            {
                prefabList.Add(prefab);
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        if (Selection.Contains(gameObject))
        {
            Gizmos.DrawCube(Vector3.zero, new Vector3(_mapArea.width, 0, _mapArea.height));
        }
    }
#endif
}

using UnityEngine;
using DG.Tweening;

public class ObjectSpawnerManager : MonoBehaviour
{
    //Components 
    public GameObject coinPrefab; 
    public GameObject redHolePrefab;
    public GameObject blueHolePrefab;
    public Transform holeparentobject;
    public Transform coinParentObject;

    //Gameplay Variables
    public int maxCoins = 5; 
    public int maxRedHoles = 5;
    public int maxBlueHoles = 5;
    public float spawnRadius = 1f; 
    public float playerSpawnRadius = 2f;

    public Vector2 spawnAreaMin = new Vector2(-2.47f, -4.69f);
    public Vector2 spawnAreaMax = new Vector2(2.47f, 4.69f);

    //Animation variables
    public float spawnanimationTime = 1;

    private static ObjectSpawnerManager instance;
    public static ObjectSpawnerManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType(typeof(ObjectSpawnerManager)) as ObjectSpawnerManager;

            return instance;
        }
        set
        {
            instance = value;
        }
    }

    void Awake()
    {
        SpawnHoles();
        SpawnCoins();
    }

    //Spawns the golf holes.
    public void SpawnHoles()
    {
        for (int i = 0; i < maxRedHoles; i++)
        {
            SpawnObjectAtRandomPosition(redHolePrefab,holeparentobject);
        }
        for (int i = 0; i < maxBlueHoles; i++)
        {
            SpawnObjectAtRandomPosition(blueHolePrefab,holeparentobject);
        }
    }

    //Spawns the coins.
    public void SpawnCoins()
    {
        for (int i = 0; i < maxCoins; i++)
        {
            SpawnObjectAtRandomPosition(coinPrefab,coinParentObject);
        }
    }

    //Goes through random positions on the screen, checking whether anything collides within a radius and then spawns an object there.
    public void SpawnObjectAtRandomPosition(GameObject objectPrefab,Transform parentT = null)
    {
        Vector2 spawnPosition = GetRandomSpawnPosition();

        if (spawnPosition != Vector2.zero) // Checks whether a valid position has been found.
        {
            GameObject spawnedObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity, parentT);
            PlaySpwaningAnimation(spawnedObject);
        }
    }

    //Plays the spawning animation of scaling an object to 0 and then scaling it back.
    private void PlaySpwaningAnimation(GameObject spawnedObject)
    {
        float initialScale = spawnedObject.transform.localScale.x;
        spawnedObject.transform.localScale = Vector3.zero;
        spawnedObject.transform.DOScale(initialScale, spawnanimationTime).SetEase(Ease.OutBounce);
    }

    //Returns a random valid position if it has been found.
    Vector2 GetRandomSpawnPosition()
    {
        Vector2 spawnPosition;
        bool positionValid;
        int attempts = 0;

        do
        {
            // Generate a random position within the spawn area
            spawnPosition = new Vector2(Random.Range(spawnAreaMin.x, spawnAreaMax.x), Random.Range(spawnAreaMin.y, spawnAreaMax.y));
            // Checks whether the position doesn't overlap with other colliders
            positionValid = IsPositionValid(spawnPosition);
            attempts++;
        } while (!positionValid && attempts < 100); // Prevent infinite loops

        if (!positionValid)
        {
            return Vector2.zero; // Return an invalid position
        }

        return spawnPosition;
    }

    //Checks whether the position intersects another colllider within a radius.
    bool IsPositionValid(Vector2 position)
    {
        // Checks for plyaer.
        Collider2D[] player = Physics2D.OverlapCircleAll(position, playerSpawnRadius, LayerMask.GetMask("Player"));
        if (player.Length > 0)
        {
            return false; 
        }

        // Checks for holes.
        Collider2D[] holes = Physics2D.OverlapCircleAll(position, spawnRadius, LayerMask.GetMask("Holes"));
        if (holes.Length > 0)
        {
            return false; 
        }

        // Checks for coins.
        Collider2D[] coins = Physics2D.OverlapCircleAll(position, spawnRadius, LayerMask.GetMask("Coins"));
        if (coins.Length > 0)
        {
            return false; 
        }

        return true;
    }

}

public enum HoleType
{
    Red,
    Blue
}
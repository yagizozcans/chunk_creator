using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChunkController : MonoBehaviour
{
    public static ChunkController instance;

    public GameObject targetObject;

    public GameObject[,] tiles;

    public Sprite lowerCliff;
    public Sprite leftCliff;
    public Sprite rightCliff;
    public Sprite upperCliff;
    public Sprite upperLeftCliff;
    public Sprite upperRightCliff;
    public Sprite lowerRightCliff;
    public Sprite lowerLeftCliff;

    public GameObject upperLeftChunk;
    public GameObject upperChunk;
    public GameObject upperRightChunk;
    public GameObject rightChunk;
    public GameObject downRightChunk;
    public GameObject downChunk;
    public GameObject downLeftChunk;
    public GameObject leftChunk;
    public GameObject middleChunk;

    public Vector2 offsetUpperChunks;
    public Vector2 offsetMiddleChunks;
    public Vector2 offsetDownChunks;

    public Sprite backgroundSprite;

    public float tileSize;
    public int chunkSizeX;
    public int chunkSizeY;
    public float dirtParticleMaxSpeed;

    [System.NonSerialized]
    public float spriteBound;

    Vector2 origin;

    public float detailScale;

    public Vector2 tileOrigin;

    public Transform tilesParent;

    float currentY;
    float currentX;
    float aspectRatio;


    public void Start()
    {
        instance = this;
        currentX = transform.position.x;
        currentY = transform.position.y;
        aspectRatio = Camera.main.aspect;
        tiles = new GameObject[chunkSizeX, chunkSizeY];
        spriteBound = backgroundSprite.bounds.size.x;
        origin = new Vector2(tileSize * spriteBound * chunkSizeX / 2, tileSize * spriteBound * chunkSizeY / 2);
        Create9Chunks(0, 0);
        tileOrigin = Vector2.zero;
    }

    public void Create9Chunks(int xOrigin, int yOrigin)
    {
        for (int i = 0; i < tilesParent.childCount; i++)
        {
            Destroy(tilesParent.GetChild(i).gameObject);
        }
        middleChunk = CreatingTileChunk(xOrigin, yOrigin);
        rightChunk = CreatingTileChunk(chunkSizeX + xOrigin, yOrigin);
        leftChunk = CreatingTileChunk(-chunkSizeX + xOrigin, yOrigin);
        upperChunk = CreatingTileChunk(xOrigin, yOrigin + chunkSizeY);
        downChunk = CreatingTileChunk(xOrigin, yOrigin + -chunkSizeY);
        upperLeftChunk = CreatingTileChunk(-chunkSizeX + xOrigin, yOrigin + chunkSizeY);
        upperRightChunk = CreatingTileChunk(chunkSizeX + xOrigin, yOrigin + chunkSizeY);
        downLeftChunk = CreatingTileChunk(-chunkSizeX + xOrigin, yOrigin + -chunkSizeY);
        downRightChunk = CreatingTileChunk(chunkSizeX + xOrigin, yOrigin + -chunkSizeY);
    }

    private void Update()
    {
        TargetChunkPositionCheck();
    }

    public void TargetChunkPositionCheck()
    {
        if (targetObject.transform.position.y >= currentY - Camera.main.orthographicSize - spriteBound * tileSize ||
    currentY - tileSize * spriteBound * chunkSizeY * 3 + Camera.main.orthographicSize >= targetObject.transform.position.y)
        {
            CreatingChunksWithCharacter();
        }
        if (targetObject.transform.position.x >= currentX - Camera.main.orthographicSize * aspectRatio - tileSize * spriteBound ||
            currentX - tileSize * spriteBound * chunkSizeX * 3 + Camera.main.orthographicSize * aspectRatio >= targetObject.transform.position.x)
        {
            CreatingChunksWithCharacter();
        }
    }
    public void CreatingChunksWithCharacter()
    {
        Create9Chunks((int)((targetObject.transform.position.x) / tileSize / spriteBound), (int)((targetObject.transform.position.y) / tileSize / spriteBound));
        currentY = targetObject.transform.position.y + tileSize * spriteBound * chunkSizeY * 3 / 2;
        currentX = targetObject.transform.position.x + tileSize * spriteBound * chunkSizeX * 3 / 2;
    }


    public GameObject CreatingTileChunk(int currentx, int currenty)
    {
        GameObject chunk = new GameObject();
        chunk.name = $"[{currentx} , {currenty}]";
        for (int x = currentx; x < chunkSizeX + currentx; x++)
        {
            for (int y = currenty; y < currenty + chunkSizeY; y++)
            {
                float noise = generateNoise(x, y, new Vector2(x * spriteBound * tileSize, y * spriteBound * tileSize));
                if (noise > 0.3f)
                {
                    GameObject nTile = CreatingTile(x, y, backgroundSprite, chunk.transform);
                    nTile.GetComponent<SpriteRenderer>().sortingOrder = -1;
                    CreatingCliffTiles(nTile, x, y, nTile.transform);
                }
                /*tile.AddComponent<BoxCollider2D>();
                tile.GetComponent<BoxCollider2D>().size = Vector2.one * spriteBound;*/
            }
        }
        chunk.transform.position -= new Vector3(origin.x, origin.y, 0);
        chunk.transform.parent = tilesParent;
        return chunk;
    }
    public void CreatingCliffTiles(GameObject forWhichObject, int x, int y,Transform parent)
    {
        if (generateNoise(x + 1, y, new Vector2(forWhichObject.transform.position.x + spriteBound * tileSize,forWhichObject.transform.position.y)) < 0.3f) CreatingTile(x, y, rightCliff,parent);
        if (generateNoise(x - 1, y, new Vector2(forWhichObject.transform.position.x - spriteBound * tileSize, forWhichObject.transform.position.y)) < 0.3f) CreatingTile(x, y, leftCliff,parent);
        if (generateNoise(x, y + 1, new Vector2(forWhichObject.transform.position.x, forWhichObject.transform.position.y + spriteBound * tileSize)) < 0.3f) CreatingTile(x, y, upperCliff,parent);
        if (generateNoise(x, y - 1, new Vector2(forWhichObject.transform.position.x, forWhichObject.transform.position.y - spriteBound * tileSize)) < 0.3f) CreatingTile(x, y, lowerCliff,parent);
        if (generateNoise(x - 1, y - 1, new Vector2(forWhichObject.transform.position.x - spriteBound * tileSize, forWhichObject.transform.position.y - spriteBound * tileSize)) < 0.3f) CreatingTile(x, y, lowerLeftCliff,parent);
        if (generateNoise(x - 1, y + 1, new Vector2(forWhichObject.transform.position.x - spriteBound * tileSize, forWhichObject.transform.position.y + spriteBound * tileSize)) < 0.3f) CreatingTile(x, y, upperLeftCliff,parent);
        if (generateNoise(x + 1, y + 1, new Vector2(forWhichObject.transform.position.x + spriteBound * tileSize, forWhichObject.transform.position.y + spriteBound * tileSize)) < 0.3f) CreatingTile(x, y, upperRightCliff,parent);
        if (generateNoise(x + 1, y - 1, new Vector2(forWhichObject.transform.position.x + spriteBound * tileSize, forWhichObject.transform.position.y - spriteBound * tileSize)) < 0.3f) CreatingTile(x, y, lowerRightCliff,parent);
    }
    public GameObject CreatingTile(int x,int y,Sprite sprite,Transform parent)
    {
        GameObject nTile = CreateTile(x, y);
        nTile.AddComponent<SpriteRenderer>();
        nTile.GetComponent<SpriteRenderer>().sprite = sprite;
        nTile.AddComponent<BoxCollider2D>();
        nTile.GetComponent<BoxCollider2D>().size = Vector3.one * spriteBound;
        nTile.transform.parent = parent;
        return nTile;
    }
    public float generateNoise(int x, int z, Vector2 whichTile)
    {
        float xNoise = (x + whichTile.x) / detailScale;
        float zNoise = (z + whichTile.y) / detailScale;

        return Mathf.PerlinNoise(xNoise, zNoise);
    }
    GameObject CreateTile(int x, int y)
    {
        Vector3 pos = new Vector3(x * tileSize * spriteBound, y * tileSize * spriteBound, 5);
        GameObject tile = new GameObject();
        tile.transform.position = pos;
        tile.transform.localScale = Vector2.one * tileSize;
        return tile;
    }
}


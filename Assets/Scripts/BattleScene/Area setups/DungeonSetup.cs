using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class DungeonSetup : MonoBehaviour, ILevelSetup
{
    //public GameObject SpawnerPrefab;
    public GameObject selfObject => gameObject;
    public TileBase WalkableTile;
    public TileBase BlockableTile;
    private Tilemap walkableTilemap;
    private Tilemap blockingTilemap;
    private NoiseMapGenerator noiseMapGenerator = new NoiseMapGenerator();

    public void InitSetup(Tilemap walkable, Tilemap blockable)
    {
        walkableTilemap = walkable;
        blockingTilemap = blockable;
    }

    public MapData SetupRandomly(int width, int height)
    {
        #region Settings
        int seed = Random.Range(1, 1001);
        float scale = 100;
        int octaves = 4; //Повторение
        float persistence = 0.5f; //Постоянство
        float lacunarity = 2; //Контролирует изменение частоты
        Vector2 offset = new Vector2(0, 0);
        #endregion

        octaves = 4;
        persistence = 0.25f;
        lacunarity = 4;

        float[,] noiseMap = noiseMapGenerator.GenerateNoiseMap(width, height, seed, scale, octaves, persistence, lacunarity, offset);

        Vector2Int start = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        MapData map = new MapData(width, height, (int)transform.position.x, (int)transform.position.y); //Подправить универсальность

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Debug.Log(noiseMap[x, y]);

                //if(Mathf.Abs(x) < width / 2 && Mathf.Abs(y) < height / 2)
                if (x >  0.25f * width && y > 0.25f * height && x <  0.75f * width && y < 0.75f * height)
                {
                    walkableTilemap.SetTile(new Vector3Int(x, y), WalkableTile);
                    map.SetPoint(x, y, true);
                }
                else
                if (noiseMap[x, y] >= 0.5f)
                {
                    walkableTilemap.SetTile(new Vector3Int(x, y), WalkableTile);
                    map.SetPoint(x, y, true);
                }
                else
                {
                    blockingTilemap.SetTile(new Vector3Int(x, y), BlockableTile);
                    map.SetPoint(x, y, false);
                }
            }
        }
        return map;
    }

    public MapData Setup(int size, float noise)
    {
        Vector2Int start = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        MapData map = new MapData(size + 20, size + 20, (int)transform.position.x - 10, (int)transform.position.y - 10); //Подправить универсальность
        for (int x = start.x - 10; x < start.x + size + 10; x++)
        {
            for (int y = start.y - 10; y < start.y + size + 10; y++)
            {

                if (x >= start.x && x < start.x + size && y >= start.y && y < start.y + size)
                {
                    walkableTilemap.SetTile(new Vector3Int(x, y), WalkableTile);
                    map.SetPoint(x, y, true);

                }
                else
                {

                    //blockingTilemap.SetTile(new Vector3Int(x, y), BlockableTile);
                    //map.SetPoint(x, y, false);
                }
            }
        }
        int offsetX1 = 0, offsetY1 = 0, offsetX2 = 0, offsetY2 = 0;
        for (int i = start.x; i < start.x + size + 1; i++)
        {
            if (i % 2 == 0)
            {
                offsetX1 = Random.Range(-2, 3);
                offsetX2 = Random.Range(-2, 3);
                offsetY1 = Random.Range(-2, 3);
                offsetY2 = Random.Range(-2, 3);
            }
            int x1 = Mathf.Clamp(start.x - offsetX1, start.x - 5, start.x);
            int x2 = Mathf.Clamp(start.x + size + offsetX2, start.x + size, start.x + size + 5);
            int y1 = Mathf.Clamp(start.y - offsetY1, start.y - 5, start.y);
            int y2 = Mathf.Clamp(start.y + size + offsetY2, start.y + size, start.y + size + 5);

            for (int k = start.x; k >= x1 - 10; k--)
            {
                if (k >= x1)
                    walkableTilemap.SetTile(new Vector3Int(k, i), WalkableTile);
                else
                    blockingTilemap.SetTile(new Vector3Int(k, i), BlockableTile);
            }

            for (int k = start.x + size; k <= x2 + 10; k++)
            {
                if (k <= x2)
                    walkableTilemap.SetTile(new Vector3Int(k, i), WalkableTile);
                else
                    blockingTilemap.SetTile(new Vector3Int(k, i), BlockableTile);
            }

            for (int k = start.y; k >= y1 - 10; k--)
            {
                if (k >= y1)
                    walkableTilemap.SetTile(new Vector3Int(i, k), WalkableTile);
                else
                    blockingTilemap.SetTile(new Vector3Int(i, k), BlockableTile);
            }

            for (int k = start.y + size; k <= y2 + 10; k++)
            {
                if (k <= y2)
                    walkableTilemap.SetTile(new Vector3Int(i, k), WalkableTile);
                else
                    blockingTilemap.SetTile(new Vector3Int(i, k), BlockableTile);
            }

            //Заполнение углов
            //Лево низ
            for (int x = start.x - 10; x < start.x; x++)
                for (int y = start.y - 10; y < start.y; y++)
                    blockingTilemap.SetTile(new Vector3Int(x, y), BlockableTile);
            //Право верх
            for (int x = start.x + size + 10; x > start.x + size; x--)
                for (int y = start.y + size + 10; y > start.y + size; y--)
                    blockingTilemap.SetTile(new Vector3Int(x, y), BlockableTile);
            //Лево верх
            for (int x = start.x - 10; x < start.x; x++)
                for (int y = start.y + size + 10; y > start.y + size; y--)
                    blockingTilemap.SetTile(new Vector3Int(x, y), BlockableTile);
            //Право низ
            for (int x = start.x + size + 10; x > start.x + size; x--)
                for (int y = start.y - 10; y < start.y; y++)
                    blockingTilemap.SetTile(new Vector3Int(x, y), BlockableTile);
        }
        return map;
    }
    private void createVoid(Vector3Int position, int size)
    {
        for (int i = 0; i < size; i++)
        {
            walkableTilemap.SetTile(new Vector3Int(position.x, position.y), WalkableTile);
            blockingTilemap.SetTile(new Vector3Int(position.x, position.y), BlockableTile);
            for (int j = 0; j < i; j++)
            {

            }
        }
    }


    private void debugSetup()
    {

    }
}

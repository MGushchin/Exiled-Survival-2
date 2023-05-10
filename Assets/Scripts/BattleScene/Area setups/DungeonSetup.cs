using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonSetup : MonoBehaviour, ILevelSetup
{
    //public GameObject SpawnerPrefab;
    public GameObject selfObject => gameObject;
    public TileBase WalkableTile;
    public TileBase BlockableTile;
    private Tilemap walkableTilemap;
    private Tilemap blockingTilemap;

    public void InitSetup(Tilemap walkable, Tilemap blockable)
    {
        walkableTilemap = walkable;
        blockingTilemap = blockable;
    }

    public MapData Setup(int size, float noise)
    {
        Vector2Int start = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        MapData map = new MapData(size + 20, size + 20, (int)transform.position.x - 10, (int)transform.position.y - 10); //���������� ���������������
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
                if(k >= x1)
                    walkableTilemap.SetTile(new Vector3Int(k, i), WalkableTile);
                else
                    blockingTilemap.SetTile(new Vector3Int(k, i), BlockableTile);
            }

            for (int k = start.x + size; k <= x2 + 10; k++)
            {
                if(k <= x2)
                    walkableTilemap.SetTile(new Vector3Int(k, i), WalkableTile);
                else
                    blockingTilemap.SetTile(new Vector3Int(k, i), BlockableTile);
            }

            for (int k = start.y; k >= y1 - 10; k--)
            {
                if(k >= y1)
                    walkableTilemap.SetTile(new Vector3Int(i, k), WalkableTile);
                else
                    blockingTilemap.SetTile(new Vector3Int(i, k), BlockableTile);
            }

            for (int k = start.y + size; k <= y2 + 10; k++)
            {
                if(k <= y2)
                    walkableTilemap.SetTile(new Vector3Int(i, k), WalkableTile);
                else
                    blockingTilemap.SetTile(new Vector3Int(i, k), BlockableTile);
            }

            //���������� �����
            //���� ���
            for(int x = start.x - 10; x < start.x; x++)
                for(int y = start.y - 10; y < start.y; y++)
                blockingTilemap.SetTile(new Vector3Int(x, y), BlockableTile);
            //����� ����
            for (int x = start.x + size + 10; x > start.x + size; x--)
                for (int y = start.y + size + 10; y > start.y + size; y--)
                    blockingTilemap.SetTile(new Vector3Int(x, y), BlockableTile);
            //���� ����
            for (int x = start.x - 10; x < start.x; x++)
                for (int y = start.y + size + 10; y > start.y + size; y--)
                    blockingTilemap.SetTile(new Vector3Int(x, y), BlockableTile);
            //����� ���
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
            for(int j=0; j < i; j++)
            {

            }
        }
    }


    private void debugSetup()
    {

    }
}

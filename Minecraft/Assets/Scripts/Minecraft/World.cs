using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public GameObject player;
    public Material material;
    public static int colHeight = 4;
    public static int chunkSize = 16;
    public static int drawRadius = 1;
    public static int buildRadius = 4;
    public static ConcurrentDictionary<string, Chunk> chunkDict;
    //public static List<string> toRemove = new();
    public static List<string> toRemove = new();
    public static List<string> toInvis = new();
    bool drawing;
    Vector3 lastBuildPos;

    public static string CreateChunkName(Vector3 pos)
    {
        return (int)pos.x + " " + (int)pos.y + " " + (int)pos.z;
    }

    void BuildChunkAt(Vector3 chunkPos)
    {
        string name = CreateChunkName(chunkPos);
        if (!chunkDict.TryGetValue(name, out _))
        {
            Chunk c = new(chunkPos, material);
            c.setStatus(Chunk.ChunkStatus.DRAW);
            c.goChunk.transform.parent = this.transform;
            chunkDict.TryAdd(c.goChunk.name, c);
        }
    }

    IEnumerator BuildRecursiveWorld(Vector3 chunkPos, int rad)
    {
        int x = (int)chunkPos.x;
        int y = (int)chunkPos.y;
        int z = (int)chunkPos.z;

        BuildChunkAt(chunkPos);
        yield return null;

        if (--rad < 0) yield break; ;
        Building(new(x + chunkSize, y, z), rad);
        Building(new(x + chunkSize, y, z + chunkSize), rad);
        Building(new(x + chunkSize, y, z - chunkSize), rad);
        Building(new(x - chunkSize, y, z), rad);
        Building(new(x - chunkSize, y, z + chunkSize), rad);
        Building(new(x - chunkSize, y, z - chunkSize), rad);
        Building(new(x, y + chunkSize, z), rad);
        Building(new(x, y - chunkSize, z), rad);
        Building(new(x, y, z + chunkSize), rad);
        Building(new(x, y, z - chunkSize), rad);
    }

    void Removing()
    {
        StartCoroutine(RemoveChunks());
        StartCoroutine(InvisChunks());
    }

    IEnumerator InvisChunks() {
        foreach (string n in toInvis)
        {
            if (chunkDict.TryGetValue(n, out Chunk c))
            {
                float distToPlayer = Vector3.Distance(player.transform.position, c.goChunk.transform.position);
                if (distToPlayer > chunkSize * buildRadius)
                {
                    c.goChunk.GetComponent<MeshRenderer>().enabled = false;
                    chunkDict.TryRemove(n, out _);
                    toInvis.Remove(n);
                    yield return null;
                }
            }

        }
    }

    IEnumerator RemoveChunks()
    {
        foreach (string n in toRemove)
        {
            if (chunkDict.TryGetValue(n, out Chunk c))
            {
                float distToPlayer = Vector3.Distance(player.transform.position, c.goChunk.transform.position);
                if (distToPlayer > chunkSize * drawRadius)
                {
                    Destroy(c.goChunk);
                    chunkDict.TryRemove(n, out _);
                    toRemove.Remove(n);
                    try
                    {
                        toInvis.Remove(n);
                    }
                    catch (Exception) { }
                    yield return null;
                }
            }

        }
    }

    IEnumerator DrawChunks()
    {
        drawing = true;
        foreach (KeyValuePair<string, Chunk> c in chunkDict)
        {
            if (c.Value.status == Chunk.ChunkStatus.DRAW)
            {
                c.Value.DrawChunk();
                yield return null;
            }
            if (Vector3.Distance(player.transform.position, c.Value.goChunk.transform.position) > chunkSize * buildRadius)
                toInvis.Add(c.Key);
            else if (Vector3.Distance(player.transform.position, c.Value.goChunk.transform.position) > chunkSize * drawRadius)
                toRemove.Add(c.Key);
        }
        Removing();
        drawing = false;
    }

    void Building(Vector3 chunkPos, int rad)
    {
        StartCoroutine(BuildRecursiveWorld(chunkPos, rad));
    }

    void Drawing()
    {
        StartCoroutine(DrawChunks());
    }

    Vector3 WhichChunk(Vector3 position)
    {
        Vector3 chunkPos = new()
        {
            x = Mathf.Floor(position.x / chunkSize) * chunkSize,
            y = Mathf.Floor(position.y / chunkSize) * chunkSize,
            z = Mathf.Floor(position.z / chunkSize) * chunkSize
        };
        return chunkPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        player.SetActive(false);
        chunkDict = new();
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        Vector3 ppos = player.transform.position;
        player.transform.position = new(ppos.x, Utils.GenerateHeight(ppos.x, ppos.z) + 1, ppos.z);
        lastBuildPos = WhichChunk(player.transform.position);
        Building(WhichChunk(lastBuildPos), buildRadius);
        Drawing();
        player.SetActive(true);
    }

    private void Update()
    {
        Vector3 movement = player.transform.position - lastBuildPos;
        if (movement.magnitude > chunkSize)
        {
            lastBuildPos = player.transform.position;
            Building(WhichChunk(lastBuildPos), buildRadius);
            Drawing();
        }
        if (!drawing) Drawing();
    }
}

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
    public static int drawRadius = 2;
    public static int buildRadius = 4;
    public static int heightRadius = 2;
    public static ConcurrentDictionary<string, Chunk> chunkDict;
    //public static List<string> toRemove = new();
    public static ConcurrentDictionary<string, Chunk> toRemove = new();
    public static ConcurrentDictionary<string, Chunk> toInvis = new();
    bool drawing;
    Vector3 lastBuildPos;
    public static float xOffset;
    public static float zOffset;

    public static string CreateChunkName(Vector3 pos)
    {
        return (int)pos.x + " " + (int)pos.y + " " + (int)pos.z;
    }

    void BuildChunkAt(Vector3 chunkPos)
    {
        string name = CreateChunkName(chunkPos);
        if (!chunkDict.TryGetValue(name, out Chunk c))
        {
            c = new(chunkPos, material, false, this);
            c.goChunk.transform.parent = this.transform;
            chunkDict.TryAdd(c.goChunk.name, c);
        }
        else
        {
            c.goChunk.SetActive(true);
        }
        //if (Vector3.Distance(player.transform.position, c.goChunk.transform.position) > drawRadius * chunkSize)
        //{
        //    c.setStatus(Chunk.ChunkStatus.INVIS);
        //    toInvis.TryAdd(name, c);
        //}
        //else if (c.status != Chunk.ChunkStatus.DONE || c.status == Chunk.ChunkStatus.INIT)
        //{ 
        //    c.setStatus(Chunk.ChunkStatus.DRAW);
        //    toInvis.TryRemove(name, out _);
        //}
    }

    public Chunk CheckForChunk(Vector3 pos)
    {
        if (chunkDict.TryGetValue(CreateChunkName(pos), out Chunk c)) return c;

        BuildChunkAt(pos);
        chunkDict.TryGetValue(CreateChunkName(pos), out c);
        return c;
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
        
        yield return null;
    }

    void Removing()
    {
        StartCoroutine(RemoveChunks());
        //StartCoroutine(InvisChunks());
    }

    IEnumerator InvisChunks() {


        foreach (KeyValuePair<string, Chunk> n in toInvis)
        {
            if (chunkDict.TryGetValue(n.Key, out Chunk c))
            {
                float distToPlayer = Vector3.Distance(player.transform.position, c.goChunk.transform.position);
                float distToPlayerY = Mathf.Abs(player.transform.position.y - c.goChunk.transform.position.y);
                if (distToPlayer > chunkSize * buildRadius || distToPlayerY > chunkSize)
                {
                    try
                    {
                        c.goChunk.SetActive(false);
                    } catch { }
                    yield return null;
                }
            }
        }
    }

    IEnumerator RemoveChunks()
    {
        foreach (KeyValuePair<string, Chunk> n in toRemove)
        {
            if (chunkDict.TryGetValue(n.Key, out Chunk c))
            {
                c.goChunk.SetActive(false);
                //chunkDict.TryRemove(n.Key, out _);
                toRemove.TryRemove(n.Key, out _);
                yield return null;

                //float distToPlayer = Vector3.Distance(player.transform.position, c.goChunk.transform.position);
                //float distToPlayerY = Mathf.Abs(player.transform.position.y - c.goChunk.transform.position.y);
                //if (distToPlayer > chunkSize * drawRadius && distToPlayerY > chunkSize * heightRadius)
                //{
                //    Destroy(c.goChunk);
                //    chunkDict.TryRemove(n.Key, out _);
                //    toRemove.TryRemove(n.Key, out _);
                //    try
                //    {
                //        toInvis.TryRemove(n.Key, out _);
                //    }
                //    catch (Exception) { }
                //    yield return null;
                //}
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
            {
                toRemove.TryAdd(c.Key, c.Value);
            }
            //else if (Vector3.Distance(player.transform.position, c.Value.goChunk.transform.position) > chunkSize * drawRadius)
            //{
            //    toInvis.TryAdd(c.Key, c.Value);
            //}
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
        xOffset = UnityEngine.Random.value * 50000;
        zOffset = UnityEngine.Random.value * 50000;
        Debug.Log("xOffset: " + xOffset + ", zOffset: " + zOffset);

        Vector3 ppos = player.transform.position;
        player.transform.position = new(ppos.x, Utils.GenerateHeight(ppos.x, ppos.z, xOffset, zOffset) + 1, ppos.z);
        lastBuildPos = WhichChunk(player.transform.position);
        Building(WhichChunk(lastBuildPos), drawRadius);
        Drawing();
        player.SetActive(true);
    }

    private void Update()
    {
        Vector3 movement = player.transform.position - lastBuildPos;
        if (movement.magnitude > chunkSize)
        {
            lastBuildPos = player.transform.position;
            Building(WhichChunk(lastBuildPos), drawRadius);
            Drawing();
        }
        if (!drawing) Drawing(); 
    }
}

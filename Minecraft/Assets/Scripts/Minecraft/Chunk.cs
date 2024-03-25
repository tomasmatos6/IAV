using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Chunk
{
    public Block[,,] chunkdata;
    public GameObject goChunk;
    public enum ChunkStatus { INIT, DRAW, DONE, INVIS };
    public ChunkStatus status;
    Material material;
    float xOffset;
    float zOffset;

    public Chunk(Vector3 pos, Material material, bool isMine, float xOffset, float zOffset)
    {
        goChunk = new(World.CreateChunkName(pos));
        goChunk.transform.position = pos;
        this.material = material;
        this.xOffset = xOffset;
        this.zOffset = zOffset;
        status = ChunkStatus.INIT;
        BuildChunk();
    }

    public void setStatus(ChunkStatus status)
    {
        this.status = status;
    }

    void BuildChunk()
    {
        bool mineStart = false;
        chunkdata = new Block[World.chunkSize, World.chunkSize, World.chunkSize];

        //Ciclo para colocar o bloco no array
        for (int y = 0; y < World.chunkSize; y++)
        {
            for (int z = 0; z < World.chunkSize; z++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {

                    Vector3 pos = new Vector3(x, y, z);

                    //Coordenada global + coordenada local
                    int worldX = (int)goChunk.transform.position.x + x;
                    int worldY = (int)goChunk.transform.position.y + y;
                    int worldZ = (int)goChunk.transform.position.z + z;

                    //Altura gerada (numeros entre 0 e 40)
                    int h = Utils.GenerateHeight(worldX, worldZ, xOffset, zOffset);
                    int hs = Utils.GenerateStoneHeight(worldX, worldZ, xOffset, zOffset);
                    int hd = Utils.GenerateDiaHeight(worldX, worldZ, xOffset, zOffset);
                    int hbd = Utils.GenerateBRHeight(worldX, worldZ, xOffset, zOffset);

                    if (worldY == hbd)
                    {
                        chunkdata[x, y, z] = new Block(Block.BlockType.BEDROCK, pos, this, material);
                    }

                    //Camada FERRO + PEDRA
                    else if (worldY <= hs && worldY > hd)
                    {
                        if (Utils.fBM3D(worldX, worldY, worldZ, 7, 5.5f, xOffset, zOffset) > 0.495f)
                        {
                            chunkdata[x, y, z] = new Block(Block.BlockType.AIR, pos, this, material);
                        }
                        else
                        {
                            if (Random.Range(0f, 1f) < 0.025f)
                            {
                                chunkdata[x, y, z] = new Block(Block.BlockType.IRON, pos, this, material);
                            }
                            else if (Random.Range(0f, 1f) > 0.1f && Random.Range(0f, 1f) < 0.15f)
                            {
                                chunkdata[x, y, z] = new Block(Block.BlockType.COAL, pos, this, material);
                            }
                            else
                                chunkdata[x, y, z] = new Block(Block.BlockType.STONE, pos, this, material);
                        }

                    }
                    //Camada DIAMANTE + FERRO + PEDRA
                    else if (worldY <= hd)
                    {

                        if (Utils.fBM3D(worldX, worldY, worldZ, 7, 5.5f, xOffset, zOffset) > 0.495f)
                        {
                            chunkdata[x, y, z] = new Block(Block.BlockType.AIR, pos, this, material);
                        }
                        else
                        {
                            if (Random.Range(0f, 1f) < 0.0125f)
                            {
                                //Diamante
                                chunkdata[x, y, z] = new Block(Block.BlockType.DIAMOND, pos, this, material);
                            }
                            else if (Random.Range(0f, 1f) > 0.025f && Random.Range(0f, 1f) < 0.03f)
                            {
                                //Ferro
                                chunkdata[x, y, z] = new Block(Block.BlockType.IRON, pos, this, material);
                            }
                            else
                                chunkdata[x, y, z] = new Block(Block.BlockType.STONE, pos, this, material);
                        }
                    }
                    //Camada PEDRA
                    else if (worldY <= hs)
                    {
                        if (Utils.fBM3D(worldX, worldY, worldZ, 2, 0.5f, xOffset, zOffset) > 0.495f)
                        {
                            chunkdata[x, y, z] = new Block(Block.BlockType.AIR, pos, this, material);
                        }
                        else
                        {
                            chunkdata[x, y, z] = new Block(Block.BlockType.STONE, pos, this, material);
                        }

                    }
                    //Camada ERVA
                    else if (worldY == h)
                    {
                        chunkdata[x, y, z] = new Block(Block.BlockType.GRASS, pos, this, material);
                        if(Random.Range(0f, 5f) > 4.9f)
                        {
                            SpawnTree(pos, (int)Random.Range(7f, 10f));
                            
                        }
                    }
                    //Camada TERRA
                    else if (worldY < h)
                    {
                        chunkdata[x, y, z] = new Block(Block.BlockType.DIRT, pos, this, material);
                    }
                    //Camada Ar
                    else
                    {
                        if(chunkdata[x, y, z] == null)
                            chunkdata[x, y, z] = new Block(Block.BlockType.AIR, pos, this, material);
                    }
                }
            }
        }
    }

    public void SpawnTree(Vector3 pos, int altura)
    {
        Vector3 newPos = pos;
        
        if(++newPos.y < altura)
        {
            
            chunkdata[(int)newPos.x, (int)newPos.y, (int)newPos.z] = new Block(Block.BlockType.WOOD, newPos, this, material);
            SpawnTree(newPos, altura);
        }
    }

    public void DrawChunk()
    {
        //Renderizar blocos
        for (int y = 0; y < World.chunkSize; y++)
            for (int z = 0; z < World.chunkSize; z++)
                for (int x = 0; x < World.chunkSize; x++)
                    chunkdata[x, y, z].Draw();
        CombineQuads();

        //Adicionar collider 
        MeshCollider collider = goChunk.AddComponent<MeshCollider>();
        collider.sharedMesh = goChunk.GetComponent<MeshFilter>().mesh;

        status = ChunkStatus.DONE;
    }

    void CombineQuads()
    {
        //1. Combine all children meshes
        MeshFilter[] meshFilters = goChunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        //2. Create a new mesh on the parent object
        MeshFilter mf = goChunk.AddComponent<MeshFilter>();
        mf.mesh = new();

        //3. Add combined meshes on children as the parent's mesh
        mf.mesh.CombineMeshes(combine);

        //4. Create a renderer for the parent
        MeshRenderer renderer = goChunk.AddComponent<MeshRenderer>();
        renderer.material = material;

        //5. Delete all uncombined children
        foreach (Transform quad in goChunk.transform)
        {
            GameObject.Destroy(quad.gameObject);
        }
    }
}

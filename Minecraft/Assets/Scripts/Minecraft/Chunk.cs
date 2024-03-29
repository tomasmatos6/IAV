using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.UI.GridLayoutGroup;

public class Chunk
{
    public Block[,,] chunkdata;
    public GameObject goChunk;
    public enum ChunkStatus { INIT, DRAW, DONE};
    public ChunkStatus status;
    Material material;
    public World world;

    public Chunk(Vector3 pos, Material material, bool isMine, World world)
    {
        goChunk = new(World.CreateChunkName(pos));
        goChunk.transform.position = pos;
        this.material = material;
        this.world = world;
        status = ChunkStatus.INIT;
        BuildChunk();
        //PlaceTrees();
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
                    int h = Utils.GenerateHeight(worldX, worldZ, World.xOffset, World.zOffset);
                    int hs = Utils.GenerateStoneHeight(worldX, worldZ, World.xOffset, World.zOffset);
                    int hd = Utils.GenerateDiaHeight(worldX, worldZ, World.xOffset, World.zOffset);
                    int hbd = Utils.GenerateBRHeight(worldX, worldZ, World.xOffset, World.zOffset);

                    int temperature = Utils.GenerateTemperature(worldX, worldZ, World.xOffset, World.zOffset);
                    int humidity = Utils.GenerateHumidity(worldX, worldZ, World.xOffset, World.zOffset);

                    Biome.BiomeType biome = Biome.GetBiome(temperature, humidity);

                    if (worldY == hbd)
                    {
                        chunkdata[x, y, z] = new Block(Block.BlockType.BEDROCK, pos, this, material);
                    }

                    //Camada FERRO + PEDRA
                    else if (worldY <= hs && worldY > hd)
                    {
                        if (Utils.fBM3D(worldX, worldY, worldZ, 7, 5.5f, World.xOffset, World.zOffset) > 0.495f)
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

                        if (Utils.fBM3D(worldX, worldY, worldZ, 7, 5.5f, World.xOffset, World.zOffset) > 0.495f)
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
                        if (Utils.fBM3D(worldX, worldY, worldZ, 2, 0.5f, World.xOffset, World.zOffset) > 0.495f)
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
                        chunkdata[x, y, z] = new Block(Biome.GetBiomeDirt(biome), pos, this, material);
                        if (x > 1 && z > 1 && x < 14 && z < 14 && worldY >= 40)
                        {
                            //if (Random.value * 5 > 4.95)
                            //{
                            //    Tree tree = new(pos + Vector3.up, this, material);
                            //    tree.SpawnTree(Random.Range(3, 5));
                            //}
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
        status = ChunkStatus.DRAW;
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

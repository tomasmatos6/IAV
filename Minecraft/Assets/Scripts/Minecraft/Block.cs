using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    enum Cubeside { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK };
    public enum BlockType { GRASS, DIRT, STONE, TNT, AIR, IRON, COAL, DIAMOND, BEDROCK, WOOD, LEAVES };
    Material material;
    BlockType bType;
    Chunk owner;
    Vector3 pos;
    bool isSolid;

    //Left bottom corner
    static Vector2 GrassSide_LBC = new(3f / 24, 33f / 34);
    static Vector2 GrassTop_LBC = new(0f / 24, 33f / 34);
    static Vector2 Dirt_LBC = new(2f / 24, 33f / 34);
    static Vector2 Stone_LBC = new(1f / 24, 33f / 34);
    static Vector2 Iron_LBC = new(1f / 24, 31f / 34);
    static Vector2 Coal_LBC = new(2f / 24, 31f / 34);
    static Vector2 Diamond_LBC = new(2f / 24, 30f / 34);
    static Vector2 Bedrock_LBC = new(1f / 24, 32f / 34);
    static Vector2 WoodSide_LBC = new(4f / 24, 32f / 34);
    static Vector2 WoodTopBottom_LBC = new(5f / 24, 32f / 34);
    static Vector2 Leaves_LBC = new(5f / 24, 30f / 34);

    Vector2[,] blockUVs = {
                                //Grass Top
                                {   GrassTop_LBC,
                                    GrassTop_LBC + new Vector2(1f / 24, 0f / 34),
                                    GrassTop_LBC + new Vector2(0f / 24, 1f / 34),
                                    GrassTop_LBC + new Vector2(1f / 24, 1f / 34)
                                },
                                //Grass Side
                                {   GrassSide_LBC,
                                    GrassSide_LBC + new Vector2(1f / 24, 0f / 34),
                                    GrassSide_LBC + new Vector2(0f / 24, 1f / 34),
                                    GrassSide_LBC + new Vector2(1f / 24, 1f / 34)
                                },
                                //Dirt
                                {   Dirt_LBC,
                                    Dirt_LBC + new Vector2(1f / 24, 0f / 34),
                                    Dirt_LBC + new Vector2(0f / 24, 1f / 34),
                                    Dirt_LBC + new Vector2(1f / 24, 1f / 34)
                                },
                                //Stone
                                {   Stone_LBC,
                                    Stone_LBC + new Vector2(1f / 24, 0f / 34),
                                    Stone_LBC + new Vector2(0f / 24, 1f / 34),
                                    Stone_LBC + new Vector2(1f / 24, 1f / 34)
                                },
                                //Carvao
                                {
                                    Coal_LBC,
                                    Coal_LBC + new Vector2(1f / 24, 0f / 34),
                                    Coal_LBC + new Vector2(0f / 24, 1f / 34),
                                    Coal_LBC + new Vector2(1f / 24, 1f / 34)
                                },
                                //Ferro
                                {
                                    Iron_LBC,
                                    Iron_LBC + new Vector2(1f / 24, 0f / 34),
                                    Iron_LBC + new Vector2(0f / 24, 1f / 34),
                                    Iron_LBC + new Vector2(1f / 24, 1f / 34)
                                },
                                //Diamante
                                {
                                    Diamond_LBC,
                                    Diamond_LBC + new Vector2(1f / 24, 0f / 34),
                                    Diamond_LBC + new Vector2(0f / 24, 1f / 34),
                                    Diamond_LBC + new Vector2(1f / 24, 1f / 34)
                                },
                                //Bedrock
                                {
                                    Bedrock_LBC,
                                    Bedrock_LBC + new Vector2(1f / 24, 0f / 34),
                                    Bedrock_LBC + new Vector2(0f / 24, 1f / 34),
                                    Bedrock_LBC + new Vector2(1f / 24, 1f / 34)
                                },
                                //Árvore PAU
                                {
                                    WoodSide_LBC,
                                    WoodSide_LBC + new Vector2(1f / 24, 0f / 34),
                                    WoodSide_LBC + new Vector2(0f / 24, 1f / 34),
                                    WoodSide_LBC + new Vector2(1f / 24, 1f / 34)
                                },
                                //Wood Top/Bottom
                                {
                                    WoodTopBottom_LBC,
                                    WoodTopBottom_LBC + new Vector2(1f / 24, 0f / 34),
                                    WoodTopBottom_LBC + new Vector2(0f / 24, 1f / 34),
                                    WoodTopBottom_LBC + new Vector2(1f / 24, 1f / 34)
                                },
                                //Wood Side
                                {
                                    Leaves_LBC,
                                    Leaves_LBC + new Vector2(1f / 24, 0f / 34),
                                    Leaves_LBC + new Vector2(0f / 24, 1f / 34),
                                    Leaves_LBC + new Vector2(1f / 24, 1f / 34)
                                }
                            };


    public Block(BlockType bType, Vector3 pos, Chunk owner, Material material)
    {
        this.bType = bType;
        this.pos = pos;
        this.owner = owner;
        this.material = material;
        //todos os blocos sao solido por default
        if (bType == BlockType.AIR) isSolid = false;
        else isSolid = true;
    }

    public void SetType(BlockType bType)
    {
        this.bType = bType;
        isSolid = bType != BlockType.AIR;
    }

    void CreateQuad(Cubeside side)
    {
        //mesh component
        Mesh mesh = new Mesh();

        //vertices coods
        Vector3 v0 = new(-0.5f, -0.5f, 0.5f);
        Vector3 v1 = new(0.5f, -0.5f, 0.5f);
        Vector3 v2 = new(0.5f, -0.5f, -0.5f);
        Vector3 v3 = new(-0.5f, -0.5f, -0.5f);
        Vector3 v4 = new(-0.5f, 0.5f, 0.5f);
        Vector3 v5 = new(0.5f, 0.5f, 0.5f);
        Vector3 v6 = new(0.5f, 0.5f, -0.5f);
        Vector3 v7 = new(-0.5f, 0.5f, -0.5f);

        //UV coords
        Vector2 uv00 = new(0, 0);
        Vector2 uv01 = new(0, 1);
        Vector2 uv10 = new(1, 0);
        Vector2 uv11 = new(1, 1);

        if (bType == BlockType.GRASS && side == Cubeside.TOP)
        {
            uv00 = blockUVs[0, 0];
            uv10 = blockUVs[0, 1];
            uv01 = blockUVs[0, 2];
            uv11 = blockUVs[0, 3];
        }
        else if (bType == BlockType.GRASS && side == Cubeside.BOTTOM || bType == BlockType.DIRT)
        {
            uv00 = blockUVs[2, 0];
            uv10 = blockUVs[2, 1];
            uv01 = blockUVs[2, 2];
            uv11 = blockUVs[2, 3];
        }
        else if (bType == BlockType.GRASS)
        {
            uv00 = blockUVs[1, 0];
            uv10 = blockUVs[1, 1];
            uv01 = blockUVs[1, 2];
            uv11 = blockUVs[1, 3];
        }
        else if (bType == BlockType.STONE)
        {
            uv00 = blockUVs[3, 0];
            uv10 = blockUVs[3, 1];
            uv01 = blockUVs[3, 2];
            uv11 = blockUVs[3, 3];
        }
        else if (bType == BlockType.COAL)
        {
            uv00 = blockUVs[4, 0];
            uv10 = blockUVs[4, 1];
            uv01 = blockUVs[4, 2];
            uv11 = blockUVs[4, 3];
        }
        else if (bType == BlockType.IRON)
        {
            uv00 = blockUVs[5, 0];
            uv10 = blockUVs[5, 1];
            uv01 = blockUVs[5, 2];
            uv11 = blockUVs[5, 3];
        }
        else if (bType == BlockType.DIAMOND)
        {
            uv00 = blockUVs[6, 0];
            uv10 = blockUVs[6, 1];
            uv01 = blockUVs[6, 2];
            uv11 = blockUVs[6, 3];
        }
        else if (bType == BlockType.BEDROCK)
        {
            uv00 = blockUVs[7, 0];
            uv10 = blockUVs[7, 1];
            uv01 = blockUVs[7, 2];
            uv11 = blockUVs[7, 3];
        }
        else if (bType == BlockType.WOOD && (side == Cubeside.TOP || side == Cubeside.BOTTOM))
        {
            uv00 = blockUVs[9, 0];
            uv10 = blockUVs[9, 1];
            uv01 = blockUVs[9, 2];
            uv11 = blockUVs[9, 3];
        }
        else if (bType == BlockType.WOOD)
        {
            uv00 = blockUVs[8, 0];
            uv10 = blockUVs[8, 1];
            uv01 = blockUVs[8, 2];
            uv11 = blockUVs[8, 3];
        }
        else if (bType == BlockType.LEAVES)
        {
            uv00 = blockUVs[10, 0];
            uv10 = blockUVs[10, 1];
            uv01 = blockUVs[10, 2];
            uv11 = blockUVs[10, 3];
        }
        else
        {
            uv00 = blockUVs[(int)(bType + 1), 0];
            uv10 = blockUVs[(int)(bType + 1), 1];
            uv01 = blockUVs[(int)(bType + 1), 2];
            uv11 = blockUVs[(int)(bType + 1), 3];
        }


        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        int[] triangles = new int[6];
        Vector2[] uv = new Vector2[4];

        switch (side)
        {
            case Cubeside.FRONT:
                vertices = new Vector3[] { v4, v5, v1, v0 };
                normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                break;

            case Cubeside.BOTTOM:
                vertices = new Vector3[] { v0, v1, v2, v3 };
                normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                break;

            case Cubeside.TOP:
                vertices = new Vector3[] { v7, v6, v5, v4 };
                normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
                break;

            case Cubeside.LEFT:
                vertices = new Vector3[] { v7, v4, v0, v3 };
                normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
                break;

            case Cubeside.RIGHT:
                vertices = new Vector3[] { v5, v6, v2, v1 };
                normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
                break;

            case Cubeside.BACK:
                vertices = new Vector3[] { v6, v7, v3, v2 };
                normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
                break;

        }

        triangles = new int[] { 3, 1, 0, 3, 2, 1 };
        uv = new Vector2[] { uv11, uv01, uv00, uv10 };

        //mesh properties 
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
        mesh.uv = uv;

        //Rever
        mesh.RecalculateBounds();

        GameObject quad = new GameObject("quad");
        quad.transform.position = this.pos;
        quad.transform.parent = owner.goChunk.transform;

        //create association between mesh and meshfilter
        MeshFilter mf = quad.AddComponent<MeshFilter>();
        mf.mesh = mesh;

        //render the mesh
        //MeshRenderer mr = quad.AddComponent<MeshRenderer>();
        //mr.material = material;
    }

    int ConvertToLocalIndex(int i)
    {
        //como chunks sao de 0 a 15
        //caso esteja no indice -1, converte para 15 
        if (i == -1)
            return World.chunkSize - 1;
        //caso seja 16, converte para 0 
        if (i == World.chunkSize)
            return 0;
        return i;
    }

    bool HasSolidNeighbour(int x, int y, int z)
    {
        Block[,,] chunkdata;

        //x = -1, chunk anterior, x>= 16, chunk seguinte
        if (x < 0 || x >= World.chunkSize || y < 0 || y >= World.chunkSize || z < 0 || z >= World.chunkSize)
        {
            Vector3 neighChunkPos = owner.goChunk.transform.position + new Vector3(
                (x - (int)pos.x) * World.chunkSize,
                (y - (int)pos.y) * World.chunkSize,
                (z - (int)pos.z) * World.chunkSize);

            string chunkName = World.CreateChunkName(neighChunkPos);

            //converter para coordenadas locais
            x = ConvertToLocalIndex(x);
            y = ConvertToLocalIndex(y);
            z = ConvertToLocalIndex(z);

            Chunk neighChunk;
            if (World.chunkDict.TryGetValue(chunkName, out neighChunk))
            {
                chunkdata = neighChunk.chunkdata;
            }
            else return false;
        }
        else
            chunkdata = owner.chunkdata;
        try
        {
            return chunkdata[x, y, z].isSolid;
        }
        catch (System.IndexOutOfRangeException) { }

        return false;
    }

    public void Draw()
    {
        //se o bloco for AIR
        if (bType == BlockType.AIR) return;

        //Avaliar vizinhos
        if (!HasSolidNeighbour((int)pos.x - 1, (int)pos.y, (int)pos.z))
            CreateQuad(Cubeside.LEFT);
        if (!HasSolidNeighbour((int)pos.x + 1, (int)pos.y, (int)pos.z))
            CreateQuad(Cubeside.RIGHT);
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y - 1, (int)pos.z))
            CreateQuad(Cubeside.BOTTOM);
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y + 1, (int)pos.z))
            CreateQuad(Cubeside.TOP);
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y, (int)pos.z - 1))
            CreateQuad(Cubeside.BACK);
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y, (int)pos.z + 1))
            CreateQuad(Cubeside.FRONT);
    }
}
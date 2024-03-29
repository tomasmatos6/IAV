using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree
{
    private List<Block> treeBody = new();
    private List<Chunk> chunksWTree = new();

    public Chunk raiz;
    public Vector3 pos;
    public Material material;

    public Tree(Vector3 pos, Chunk raiz, Material material)
    {
        this.pos = pos;

        if(pos.y >= World.chunkSize)
        {
            this.raiz = raiz.world.CheckForChunk(raiz.goChunk.transform.position + new Vector3(0,World.chunkSize,0));
            this.pos.y = 0;
        }
        else this.raiz = raiz;
        
        this.material = material;
    }

    public Chunk VerifyChunk(Vector3 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;

        int chunkOffsetX = x < 0 ? -1 : x > 15 ? 1 : 0;
        int chunkOffsetY = y < 0 ? -1 : y > 15 ? 1 : 0;
        int chunkOffsetZ = z < 0 ? -1 : z > 15 ? 1 : 0;

        int dataPosX = x < 0 ? World.chunkSize + x : x > 15 ? x - World.chunkSize : x;
        int dataPosZ = z < 0 ? World.chunkSize + z : z > 15 ? z - World.chunkSize : z;

        Chunk occupiedC = raiz.world.CheckForChunk(
            raiz.goChunk.transform.position + new Vector3(World.chunkSize * chunkOffsetX, World.chunkSize * chunkOffsetY, World.chunkSize * chunkOffsetZ));

        return occupiedC;
    }

    public void SpawnTree(int altura)
    {
        Vector3 newPos = pos;
        int y = (int)newPos.y;

        for (int i = 1; i <= altura; i++)
        {
            Chunk chunk = VerifyChunk(newPos);
            y = y < 0 ? World.chunkSize + y : y > 15 ? y - World.chunkSize : y;
            chunk.chunkdata[(int)newPos.x, y, (int)newPos.z] = new Block(Block.BlockType.WOOD, newPos, chunk, material);
            newPos = new Vector3(newPos.x, ++y, newPos.z);
        }
        // CROSS
        //Cross(newPos);
        // 3x3
        //MakeLeaves(newPos, 3, 0, true);
        //// 5x5
        //MakeLeaves(newPos, 5, 1, false);
        //MakeLeaves(newPos, 5, 2, false);
    }

    private void MakeLeaves(Vector3 newPos, int n, int h, bool top)
    {
        int offset = n / 2;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i == j && i == offset && !top) continue;

                int x = ((int)newPos.x + i - offset); // 4 + 0 - 1
                int y = (int)newPos.y - h;
                int z = ((int)newPos.z + j - offset); // 4 + 0 - 1

                //if (x < 0 || x >= World.chunkSize || y < 0 || y >= World.chunkSize || z < 0 || z >= World.chunkSize)
                //{
                //    Vector3 neighChunkPos = raiz.transform.position + new Vector3(
                //        (x - (int)newPos.x) * World.chunkSize,
                //        (y - (int)newPos.y) * World.chunkSize,
                //        (z - (int)newPos.z) * World.chunkSize);

                //    string chunkName = World.CreateChunkName(neighChunkPos);

                //    //converter para coordenadas locais
                

                //    if (World.chunkDict.TryGetValue(chunkName, out Chunk neighChunk))
                //    {
                //        chunkdata = neighChunk.chunkdata;
                //    }

                //}

                Chunk chunk = VerifyChunk(pos);
                x = x < 0 ? World.chunkSize + x : x > 15 ? x - World.chunkSize : x;
                y = y < 0 ? World.chunkSize + y : y > 15 ? y - World.chunkSize : y;
                z = z < 0 ? World.chunkSize + z : z > 15 ? z - World.chunkSize : z;

                chunk.chunkdata[x, y, z] =
                    new Block(Block.BlockType.LEAVES, new(x, y, z), raiz, material);
                
            }
        }
    }
}

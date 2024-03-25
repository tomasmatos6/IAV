using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInteraction : MonoBehaviour
{
    public Camera cam;
    enum InteractionType { DESTROY, BUILD};
    InteractionType interactionType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool interaction = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
        if(interaction)
        {
            interactionType = Input.GetMouseButtonDown(0) ? InteractionType.DESTROY : InteractionType.BUILD;
            RaycastHit hit;
            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10))
            {
                string chunkName = hit.collider.gameObject.name;
                float chunkx = hit.collider.gameObject.transform.position.x;
                float chunky = hit.collider.gameObject.transform.position.y;
                float chunkz = hit.collider.gameObject.transform.position.z;

                Vector3 hitBlock;
                if(interactionType == InteractionType.DESTROY)
                {
                    hitBlock = hit.point - hit.normal / 2f;
                }
                else
                {
                    hitBlock = hit.point + hit.normal / 2f;
                }

                int blockx = (int)(Mathf.Round(hitBlock.x) - chunkx);
                int blocky = (int)(Mathf.Round(hitBlock.y) - chunky);
                int blockz = (int)(Mathf.Round(hitBlock.z) - chunkz);

                if(World.chunkDict.TryGetValue(chunkName, out Chunk c))
                {
                    if (interactionType == InteractionType.DESTROY)
                        c.chunkdata[blockx, blocky, blockz].SetType(Block.BlockType.AIR);
                    else
                        c.chunkdata[blockx, blocky, blockz].SetType(Block.BlockType.STONE);
                }
                List<string> updates = new();
                updates.Add(chunkName);

                if (blockx == 0)
                    updates.Add(World.CreateChunkName(new(chunkx - World.chunkSize, chunky, chunkz)));
                if (blockx == World.chunkSize)
                    updates.Add(World.CreateChunkName(new(chunkx + World.chunkSize, chunky, chunkz)));
                if (blocky == 0)
                    updates.Add(World.CreateChunkName(new(chunkx, chunky - World.chunkSize, chunkz)));
                if (blocky == World.chunkSize)
                    updates.Add(World.CreateChunkName(new(chunkx, chunky + World.chunkSize, chunkz)));
                if (blockz == 0)
                    updates.Add(World.CreateChunkName(new(chunkx, chunky, chunkz - World.chunkSize)));
                if (blockz == World.chunkSize)
                    updates.Add(World.CreateChunkName(new(chunkx, chunky, chunkz + World.chunkSize)));

                foreach(string name in updates)
                {
                    if(World.chunkDict.TryGetValue(name, out c))
                    {
                        DestroyImmediate(c.goChunk.GetComponent<MeshFilter>());
                        DestroyImmediate(c.goChunk.GetComponent<MeshRenderer>());
                        DestroyImmediate(c.goChunk.GetComponent<MeshCollider>());
                        c.DrawChunk();
                    }
                }
            }
        }
    }
}
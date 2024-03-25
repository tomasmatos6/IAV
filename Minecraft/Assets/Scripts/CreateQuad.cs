using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateQuad : MonoBehaviour
{
    enum Cubeside { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK};
    public enum BlockType { GRASS, DIRT, STONE };
    public Material material;
    public BlockType bType;

    static Vector2 GrassSide_LBC = new Vector2(3f, 15f) / 16;
    static Vector2 GrassTop_LBC = new Vector2(2f, 6f) / 16;
    static Vector2 Dirt_LBC = new Vector2(2f, 15f) / 16;
    static Vector2 Stone_LBC = new Vector2(0f, 15f) / 16;

    Vector2[,] blocksUVs =
    {
        /*GRASS TOP*/       {GrassTop_LBC, GrassTop_LBC + new Vector2(1f,0f)/16,
                                GrassTop_LBC + new Vector2(0f,1f)/16, GrassTop_LBC + new Vector2(1f,1f)/16},
        /*GRASS SIDE*/      {GrassSide_LBC, GrassSide_LBC + new Vector2(1f,0f)/16,
                                GrassSide_LBC + new Vector2(0f,1f)/16, GrassSide_LBC + new Vector2(1f,1f)/16},
        /*DIRT*/            {Dirt_LBC, Dirt_LBC + new Vector2(1f,0f)/16,
                                Dirt_LBC + new Vector2(0f,1f)/16, Dirt_LBC + new Vector2(1f,1f)/16},
        /*STONE*/           {Stone_LBC, Stone_LBC + new Vector2(1f,0f)/16,
                                Stone_LBC + new Vector2(0f,1f)/16, Stone_LBC + new Vector2(1f,1f)/16}
    };

    void Quad(Cubeside side)
    {
        Mesh mesh = new();

        Vector3 v0 = new(-0.5f, -0.5f, 0.5f);
        Vector3 v1 = new(0.5f, -0.5f, 0.5f);
        Vector3 v2 = new(0.5f, -0.5f, -0.5f);
        Vector3 v3 = new(-0.5f, -0.5f, -0.5f);
        Vector3 v4 = new(-0.5f, 0.5f, 0.5f);
        Vector3 v5 = new(0.5f, 0.5f, 0.5f);
        Vector3 v6 = new(0.5f, 0.5f, -0.5f);
        Vector3 v7 = new(-0.5f, 0.5f, -0.5f);

        Vector2 uv00, uv01, uv10, uv11;

        if(bType == BlockType.GRASS && side == Cubeside.TOP)
        {
            uv00 = blocksUVs[0, 0];
            uv10 = blocksUVs[0, 1];
            uv01 = blocksUVs[0, 2];
            uv11 = blocksUVs[0, 3];
        }
        else if(bType == BlockType.GRASS && side == Cubeside.BOTTOM)
        {
            uv00 = blocksUVs[2, 0];
            uv10 = blocksUVs[2, 1];
            uv01 = blocksUVs[2, 2];
            uv11 = blocksUVs[2, 3];
        }
        else
        {
            uv00 = blocksUVs[(int)(bType + 1), 0];
            uv10 = blocksUVs[(int)(bType + 1), 1];
            uv01 = blocksUVs[(int)(bType + 1), 2];
            uv11 = blocksUVs[(int)(bType + 1), 3];
        }

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];

        switch (side)
        {
            case Cubeside.FRONT:
                vertices = new Vector3[] { v4, v5, v1, v0 };
                normals = new Vector3[] { Vector3.forward, Vector3.forward, 
                                        Vector3.forward, Vector3.forward};
                break;
            case Cubeside.BOTTOM:
                vertices = new Vector3[] { v0, v1, v2, v3 };
                normals = new Vector3[] {Vector3.down, Vector3.down,
                                        Vector3.down, Vector3.down};
                break;
            case Cubeside.TOP:
                vertices = new Vector3[] { v7, v6, v5, v4 };
                normals = new Vector3[] {Vector3.up, Vector3.up,
                                        Vector3.up, Vector3.up};
                break;
            case Cubeside.LEFT:
                vertices = new Vector3[] { v7, v4, v0, v3 };
                normals = new Vector3[] {Vector3.left, Vector3.left,
                                        Vector3.left, Vector3.left};
                break;
            case Cubeside.RIGHT:
                vertices = new Vector3[] { v5, v6, v2, v1 };
                normals = new Vector3[] {Vector3.right, Vector3.right,
                                        Vector3.right, Vector3.right};
                break;
            case Cubeside.BACK:
                vertices = new Vector3[] { v6, v7, v3, v2 };
                normals = new Vector3[] {Vector3.back, Vector3.back,
                                        Vector3.back, Vector3.back};
                break;
        }

        int[] triangles = new int[] { 3, 1, 0, 3, 2, 1 };
        Vector2[] uv = new Vector2[] { uv11, uv01, uv00, uv10 };

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
        mesh.uv = uv;

        GameObject quad = new("quad");
        quad.transform.parent = this.gameObject.transform;

        MeshFilter mf = quad.AddComponent<MeshFilter>();
        mf.mesh = mesh;
    }

    void CombineQuads()
    {
        //1. Combine all children meshes
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while(i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        //2. Create a new mesh on the parent object
        MeshFilter mf = this.gameObject.AddComponent<MeshFilter>();
        mf.mesh = new();

        //3. Add combined meshes on children as the parent's mesh
        mf.mesh.CombineMeshes(combine);

        //4. Create a renderer for the parent
        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
        renderer.material = material;

        //5. Delete all uncombined children
        foreach(Transform quad in this.transform)
        {
            Destroy(quad.gameObject);
        }
    }

    void CreateCube()
    {
        Quad(Cubeside.LEFT);
        Quad(Cubeside.RIGHT);
        Quad(Cubeside.TOP);
        Quad(Cubeside.BOTTOM);
        Quad(Cubeside.FRONT);
        Quad(Cubeside.BACK);

        CombineQuads();
    }
    // Start is called before the first frame update
    void Start()
    {
        CreateCube();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

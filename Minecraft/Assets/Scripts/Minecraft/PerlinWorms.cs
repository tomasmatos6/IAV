using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinWorms : MonoBehaviour
{
    private Transform cubeStart;
    private Transform cubeEnd;
    private float smooth;
    private float offset = 23456;
    private float maxTurningAngle = 30f;
    Vector3 end, start;
    [SerializeField] private List<Vector3> wormSequence;
    float weightTarget;
    Vector3[] dirNeigh = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

    public PerlinWorms(Transform cubeStart, Transform cubeEnd, float smooth, float offset, float maxTurningAngle, float weightTarget)
    {
        this.smooth = smooth;
        this.offset = offset;
        this.maxTurningAngle = maxTurningAngle;
        this.weightTarget = weightTarget;
        start = cubeStart.position;
        end = cubeEnd.position;
        wormSequence = PerlinWorm(start, end);
        StartCoroutine(RenderWorm());
    }

    IEnumerator EnlargeWorm()
    {
        foreach(Vector3 v in wormSequence)
        {
            foreach(Vector3 dir in dirNeigh)
            {
                Vector3 neighPos = v + dir;
                GameObject newgo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newgo.transform.parent = this.transform;
                newgo.transform.position = neighPos;
                newgo.GetComponent<MeshRenderer>().material.color = Color.cyan;
                yield return null;
            }
        }
    }

    List<Vector3> PerlinWorm(Vector3 start, Vector3 end, int maxSteps = 100)
    {
        List<Vector3> sequence = new();
        Vector3 pos = start;
        Vector3 dirRef = (end - start).normalized;
        for (int i = 0; i < maxSteps; i++)
        {
            float yawRotation = convert2Angle(Mathf.PerlinNoise(pos.x * smooth + offset, pos.z * smooth + offset));
            float pitchRotation = convert2Angle(Mathf.PerlinNoise(pos.y * smooth + offset, pos.z * smooth + offset));

            Vector3 dir = Quaternion.AngleAxis(yawRotation, Vector3.up) * dirRef;
            dir = Quaternion.AngleAxis(pitchRotation, Vector3.right) * dir;
            dir = (1 - weightTarget) * dir + weightTarget * dirRef;
            pos += dir;
            if (Vector3.Distance(pos, end) < 1) break;
            sequence.Add(roundVector3(pos));
        }
        while(Vector3.Distance(pos, end) > 1.5f)
        {
            Vector3 dirToTarget = (end - pos).normalized;
            pos += dirToTarget;
            sequence.Add(roundVector3(pos));
        }
        return sequence;
    }

    IEnumerator RenderWorm()
    {
        foreach (Vector3 v in wormSequence)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = v;
            go.transform.parent = this.transform;
            go.transform.name = v.x + " " + v.y + " " + v.z;
            go.GetComponent<MeshRenderer>().material.color = Color.yellow;
            yield return new WaitForSeconds(0.1f);
        }
        StartCoroutine(EnlargeWorm());
    }

    // Start is called before the first frame update
    void Start()
    {
        cubeStart.GetComponent<MeshRenderer>().material.color = Color.green;
        cubeEnd.GetComponent<MeshRenderer>().material.color = Color.red;
        start = cubeStart.position;
        end = cubeEnd.position;
        wormSequence = PerlinWorm(start, end);
        StartCoroutine(RenderWorm());
    }

    float convert2Angle(float p)
    {
        return (2 * p - 1) * maxTurningAngle;
    }

    Vector3 roundVector3(Vector3 pos)
    {
        return new Vector3(
            Mathf.Floor(pos.x + 0.5f),
            Mathf.Floor(pos.y + 0.5f),
            Mathf.Floor(pos.z + 0.5f));
    }

    // Update is called once per frame
    void Update()
    {

    }
}

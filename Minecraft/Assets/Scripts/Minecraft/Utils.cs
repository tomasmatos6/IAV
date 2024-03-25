using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    //Quanto mais pequeno mais "suave" é o terreno
    static float smooth = 0.002f;
    static float smoothTeste = 0.01f;
    static float smooth3D = 10f * smooth;

    //seed
    static float offset = 32000;
    //Maximo de altura do terreno
    //static int maxHeight = 255;
    //Acima de 40 tudo é AIR
    static int maxHeight = 150;

    static int octaves = 6;
    static float persistence = 0.7f;

    //Gerar altitude a partir de latitude e longitude de AR
    public static int GenerateHeight(float x, float z)
    {
        return (int)Map(0, maxHeight, 0, 1, fBM(x * smooth, z * smooth, octaves, persistence));
    }

    //Gerar altitude para Pedra
    public static int GenerateStoneHeight(float x, float z)
    {
        return (int)Map(0, maxHeight - 10, 0, 1, fBM(x * 3 * smooth, z * 3 * smooth, octaves - 1, 1.2f * persistence));
    }

    //Gerar altitude para Diamante
    public static int GenerateDiaHeight(float x, float z)
    {
        return (int)Map(0, maxHeight * 0.2f, 0, 1, fBM(x * smoothTeste, z * smoothTeste, octaves - 1, persistence));
    }

    //Gerar altitude para Bedrock
    public static int GenerateBRHeight(float x, float z)
    {
        return (int)Map(0, maxHeight - (maxHeight - 1), 0, 1, fBM(x * smooth, z * smooth, octaves, persistence));
    }

    //Converte intervalos de valores 
    static float Map(float newmin, float newmax, float orimin, float orimax, float val)
    {
        return Mathf.Lerp(newmin, newmax, Mathf.InverseLerp(orimin, orimax, val));
    }

    //Fractal bronian motion 3D
    public static float fBM3D(float x, float y, float z, int octaves, float persistence)
    {
        float xy = fBM(x * smooth3D, y * smooth3D, octaves, persistence);
        float yx = fBM(y * smooth3D, x * smooth3D, octaves, persistence);
        float xz = fBM(x * smooth3D, z * smooth3D, octaves, persistence);
        float zx = fBM(z * smooth3D, x * smooth3D, octaves, persistence);
        float yz = fBM(y * smooth3D, z * smooth3D, octaves, persistence);
        float zy = fBM(z * smooth3D, y * smooth3D, octaves, persistence);

        //float val = (xy + yx + xz + zx + yz + zy) / 6;
        //return (val - 0.5f) * 2;
        return (xy + yx + xz + zx + yz + zy) / 6;
    }

    // Random Inicio
    // Random Fim
    // Random smooth
    // Turning angle 45?
    // weightTarget

    static float fBM(float x, float z, int octaves, float persistence)
    {
        float total = 0;
        float amplitude = 1;
        float frequency = 1;
        float maxValue = 0;

        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise((x + offset) * frequency, (z + offset) * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2;
        }

        //Garantir que fica entre 0 e 1
        return total / maxValue;
    }


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome
{
    public enum BiomeType { FOREST, SNOW, DESERT, SWAMP};

    // Humidade para implementação de biomas futuros

    public static BiomeType GetBiome(int temperature, int humidity)
    {
        BiomeType biome = BiomeType.FOREST;
        if (temperature < 30 && humidity < 40) biome = BiomeType.SNOW;
        if (temperature < 30 && humidity > 40) biome = BiomeType.SWAMP;
        if (temperature > 30 && humidity < 40) biome = BiomeType.DESERT;
        if (temperature > 30 && humidity > 40) biome = BiomeType.FOREST;
        return biome;
    }

    public static Block.BlockType GetBiomeDirt(BiomeType bType)
    {
        switch (bType)
        {
            case BiomeType.SNOW:
                return Block.BlockType.SNOW;
            case BiomeType.SWAMP:
                return Block.BlockType.MUD;
            case BiomeType.DESERT:
                return Block.BlockType.SAND;
            default:
                return Block.BlockType.GRASS;
        }
    }
}

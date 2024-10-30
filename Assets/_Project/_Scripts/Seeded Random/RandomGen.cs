using System;
using System.Collections.Generic;

public static class RandomGen
{
    public static int Seed { get; private set; }
    private const string DEFAULT = "default";
    private static Dictionary<string, System.Random> randomGenerators = new();

    private struct RandomInfo
    {
        public int StartingSeed;

        public RandomInfo(int startingSeed)
        {
            StartingSeed = startingSeed;
        }
    }

    public static void Initialize()
    {
        Seed = DateTime.Now.Second;
        randomGenerators.Clear();
    }

    public static void Initialize(int seed)
    {
        Seed = seed;
        randomGenerators.Clear();
    }

    // key would be for example battle or roomGeneration
    public static int Get(string key)
    {
        if(!randomGenerators.ContainsKey(key))
        {
            randomGenerators[key] = new System.Random(Seed);
        }

        return randomGenerators[key].Next();
    }

    public static int Get() => Get(DEFAULT);
}

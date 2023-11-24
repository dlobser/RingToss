using System;
// using UnityEngine;

public class RandomWithSeed
{
    private System.Random random;
    private int _seed;

    // Public property to get and set the seed
    public int Seed
    {
        get
        {
            return _seed;
        }
        set
        {
            _seed = value;
            random = new System.Random(_seed); // Reinitialize the random instance with the new seed
        }
    }

    // Constructor
    public RandomWithSeed(int initialSeed)
    {
        Seed = initialSeed; // Set the initial seed
    }

    // Method to get a random integer
    public int Random(int min, int max)
    {
        return random.Next(min, max);
    }

    public float Random()
    {
        return random.Next();
    }

    // Method to get a random float
    public float Random(float min, float max)
    {
        double range = max - min;
        double sample = random.NextDouble();
        double scaled = (sample * range) + min;
        return (float)scaled;
    }
}

// Usage example:
// RandomWithSeed myRandom = new RandomWithSeed(12345);
// int randomNumber = myRandom.GetRandomInt(0, 100);
// float randomFloat = myRandom.GetRandomFloat(0.0f, 1.0f);
// myRandom.Seed = 67890; // Changing the seed

using System;
using System.Collections;
using System.Collections.Generic;

public static class RandomGenerator  {

    public static Random rand = new Random((int) DateTime.Now.Ticks & 0x0000FFFF) ;

    public static int GetRandom()
    {
        return rand != null ? rand.Next() : 0;
    }

    public static int GetRandomInRange(int min, int max)
    {
        return rand != null ? rand.Next(min, max) : 0;
    }

    public static void Shuffle<T>(this IList<T> collectionToShuffle)
    {
        int n = collectionToShuffle.Count;
        while (n > 1)
        {
            n--;
            int k = RandomGenerator.GetRandomInRange(0, n);
            T value = collectionToShuffle[k];
            collectionToShuffle[k] = collectionToShuffle[n];
            collectionToShuffle[n] = value;
        }
    }
}

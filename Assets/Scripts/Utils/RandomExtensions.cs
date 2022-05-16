 using System;

 public static class RandomExtensions
 {
     public static bool IsSameOrSubclass(this Type potentialBase, Type potentialDescendant)
     {
         return potentialDescendant.IsSubclassOf(potentialBase)
                || potentialDescendant == potentialBase;
     }

     public static long RandomLong(this Random random, long min = 0, long max = long.MaxValue)
     {
         byte[] buf = new byte[8];
         random.NextBytes(buf);

         long longRand = BitConverter.ToInt64(buf, 0);

         return Math.Abs(longRand % (max - min)) + min;
     }
 }
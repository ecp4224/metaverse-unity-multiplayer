using System;

public class RandomHelper
{
    private static readonly Random RANDOM = new Random();
    private const double LAMBDA = 5;

    public static bool RandomBoolean()
    {
        return RANDOM.Next(0, 2) == 1;
    }

    public static int Random(int max, RandomDistribution distribution = RandomDistribution.UNIFORM, double lambda = LAMBDA)
    {
        return Random(0, max, distribution, lambda);
    }

    public static int Random(int min, int max, RandomDistribution distribution = RandomDistribution.UNIFORM, double lambda = LAMBDA)
    {
        double u;
        switch (distribution)
        {
            case RandomDistribution.UNIFORM:
                u = UniformRandom();
                break;
            case RandomDistribution.NEGATIVE_EXPONENTIAL:
                u = NegativeExponentialRandom(lambda);
                break;
            case RandomDistribution.POSITIVE_EXPONENTIAL:
                u = PositiveExponentialRandom(lambda);
                break;
            default:
                u = UniformRandom();
                break;
        }

        return min + (int) (((max - min) * u));
    }

    public static float Random(float max, RandomDistribution distribution = RandomDistribution.UNIFORM, double lambda = LAMBDA)
    {
        return Random(0, max, distribution, lambda);
    }

    public static float Random(float min, float max, RandomDistribution distribution = RandomDistribution.UNIFORM, double lambda = LAMBDA)
    {
        double u;
        switch (distribution)
        {
            case RandomDistribution.UNIFORM:
                u = UniformRandom();
                break;
            case RandomDistribution.NEGATIVE_EXPONENTIAL:
                u = NegativeExponentialRandom(lambda);
                break;
            case RandomDistribution.POSITIVE_EXPONENTIAL:
                u = PositiveExponentialRandom(lambda);
                break;
            default:
                u = UniformRandom();
                break;
        }

        return (float)(min + (max - min) * u);
    }

    public static double UniformRandom()
    {
        return RANDOM.NextDouble();
    }

    public static double NegativeExponentialRandom(double lambda = LAMBDA)
    {
        return -Math.Log(1 - (1 - Math.Exp(-lambda)) * RANDOM.NextDouble()) / lambda;
    }

    public static double PositiveExponentialRandom(double lambda = LAMBDA)
    {
        return Math.Log(1 - (1 - Math.Exp(lambda)) * RANDOM.NextDouble()) / lambda;
    }
}
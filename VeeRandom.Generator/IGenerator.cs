namespace VeeRandom.Generator;

public interface IGenerator
{
    public long GenerateRandomNumber();

    public long GetPeriod();
}

using Microsoft.Extensions.Options;

namespace VeeRandomGenerator;

public class Generator : IGenerator
{

    private readonly long _m;
    private readonly long _a;
    private readonly long _c;
    private long _x;

    private bool _isZeroXReturned;

    public Generator(IOptions<GeneratorSettings> options)
    {
        GeneratorSettings settings = options.Value;

        _m = settings.Modulus;
        _a = settings.Multiplier;
        _c = settings.Increment;
        _x = settings.InitialValue;
        _isZeroXReturned = false;
    }

    public long GenerateRandomNumber()
    {
        if (!_isZeroXReturned)
        {
            _isZeroXReturned = true;

            return _x;
        }

        _x = (_a * _x + _c) % _m;

        return _x;
    }

    public long GetPeriod()
    {
        var numbers = new List<long>();
        long period;

        while (true)
        {
            long number = GenerateRandomNumber();

            if (numbers.Contains(number))
            {
                int index = numbers.IndexOf(number);
                period = numbers.Count - index;
                break;
            }

            numbers.Add(number);
        }

        return period;
    }
}
namespace MatchingAlgorithm;

public struct SweepParameter
{
    public double Min;
    public double Max;
    public double StepSize;

    public static implicit operator List<double>(SweepParameter sweepParameter)
    {
        if (sweepParameter.StepSize == 0)
            throw new ArgumentOutOfRangeException(nameof(sweepParameter.StepSize));
        
        var list = new List<double>();
        for(var i = sweepParameter.Min; i <= sweepParameter.Max; i+= sweepParameter.StepSize)
            list.Add(i);

        return list;
    }
}
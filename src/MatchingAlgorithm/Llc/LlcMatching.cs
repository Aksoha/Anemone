namespace MatchingAlgorithm.Llc;

public class LlcMatching : IEnergyMatching<LlcMatchingParameter, LlcMatchingResult>
{
    private readonly ILlcTopology _topology;

    private LlcMatchingParameter _workingData;
    private LlcMatchingParameter _backupData;
    private IEnumerable<double> Frequency => _workingData.Frequency;
    private List<double> Temperature { get; set; }
    private IEnumerable<double> Inductance => _workingData.Inductance;
    private IEnumerable<double> Capacitance => _workingData.Capacitance;
    private double VoltageLimit => _workingData.VoltageLimit;
    private double CurrentLimit => _workingData.CurrentLimit;
    private double ExpectedPower => _workingData.ExpectedPower;

    public LlcMatching(ILlcTopology topology)
    {
        _topology = topology;
    }


    public IEnumerable<LlcMatchingResult> EnergyMatching(LlcMatchingParameter parameters)
    {
        var f = parameters.Frequency.First();
        var min = parameters.Temperature.MinBy(t => _topology.Reactance(f, t));
        var max = parameters.Temperature.MaxBy(t => _topology.Reactance(f, t));
        Temperature = new List<double>
        {
            parameters.Temperature.First(),
            max,
            min,
            parameters.Temperature.Last(),
        };
        _workingData = parameters;
        Backup(parameters);


        var index = 0;
        _topology.Capacitance = Capacitance.ElementAt(index);
        index++;

        var inductance = InductanceRange();
        while (inductance.Min >= inductance.Max)
        {
            // TODO: change exception
            if (index >= Capacitance.Count())
                throw new Exception();

            _topology.Capacitance = Capacitance.ElementAt(index);
            index++;

            inductance = InductanceRange();
        }

        // TODO: change exception
        if (Inductance.First(x => x > inductance.Min) > inductance.Max)
            throw new Exception();


        throw new NotImplementedException();
    }


    private InductanceRange InductanceRange()
    {
        var result = new InductanceRange { Min = double.MinValue, Max = double.MaxValue };

        foreach (var t in Temperature)
        {
            var pairs = ParallelResonanceRange(t);
            if (pairs.Count == 0)
                return new InductanceRange();

            var min = pairs.MinBy(x => x.Reactance);
            var max = pairs.MaxBy(x => x.Reactance);

            var lmin = -max.Reactance / (2 * Math.PI * max.Frequency);
            var lmax = -min.Reactance / (2 * Math.PI * min.Frequency);

            if (lmax < result.Max)
                result.Max = lmax;

            if (lmin > result.Min)
                result.Min = lmin;
        }

        return result;
    }

    private List<FrequencyReactancePair> ParallelResonanceRange(double temperature)
    {
        var capacitiveRange = CapacitiveRange(temperature);
        if (capacitiveRange.Count == 0)
            return capacitiveRange;

        var min = capacitiveRange.MinBy(x => x.Reactance);
        var index = capacitiveRange.IndexOf(min);

        if (index == 0)
            return capacitiveRange;

        capacitiveRange.RemoveRange(0, index);
        return capacitiveRange;
    }

    private List<FrequencyReactancePair> CapacitiveRange(double temperature)
    {
        var results = (from f in Frequency
            let xp = _topology.ParallelReactance(f, temperature)
            where xp < 0
            select new FrequencyReactancePair() { Frequency = f, Reactance = xp }).ToList();
        return results;
    }

    private void Backup(LlcMatchingParameter parameters)
    {
        _backupData = (LlcMatchingParameter)parameters.Clone();
    }

    private void Restore()
    {
        _workingData = _backupData;
    }
}
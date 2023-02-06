namespace MatchingAlgorithm.Llc;

/// <summary>
///     A result of compensation
/// </summary>
public struct LlcCompensationResult
{
    /// <summary>
    ///     minimal value of serial inductance
    /// </summary>
    public double MinInductance;

    /// <summary>
    ///     maximal value of serial inductance
    /// </summary>
    public double MaxInductance;

    /// <summary>
    ///     capacitance of the system
    /// </summary>
    public double Capacitance;


    // in the future this could be replaced with a scoring system of 0-100, where 100 represents a fully compensated system
    // such scoring system should take into consideration not only "is llc system compensated or not" but also determine
    // how unmatched it is (phase shift)
    /// <summary>
    ///     Determines whether this configuration provides a full compensation
    /// </summary>
    public bool FullCompensation;
}
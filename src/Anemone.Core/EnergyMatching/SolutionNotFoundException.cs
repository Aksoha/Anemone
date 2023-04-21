using System;

namespace Anemone.Core.EnergyMatching;

public class SolutionNotFoundException : Exception
{
    public SolutionNotFoundException()
    {
        
    }
    
    public SolutionNotFoundException(string? message) : base(message)
    {
        
    }
    
    public SolutionNotFoundException(string? message, Exception? innerException): base(message, innerException)
    {
        
    }
}
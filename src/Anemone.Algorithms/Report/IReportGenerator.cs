using System.Data;
using Anemone.Algorithms.Models;

namespace Anemone.Algorithms.Report;

public interface IReportGenerator
{
    DataTable Generate(MatchingResultBase matchingResult);
}
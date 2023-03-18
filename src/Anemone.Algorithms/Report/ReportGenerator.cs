using System;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using Anemone.Algorithms.Models;

namespace Anemone.Algorithms.Report;

public class ReportGenerator : IReportGenerator
{
    public DataTable Generate(MatchingResultBase matchingResult)
    {
        return matchingResult switch
        {
            LlcMatchingResult llc => Generate_Llc(llc),
            _ => throw new NotSupportedException($"the type {matchingResult.GetType()} is currently not supported")
        };
    }

    private static DataTable Generate_Llc(LlcMatchingResult matchingResult)
    {
        var pointsType = matchingResult.Points.GetType().GetElementType();
        if (pointsType.IsSubclassOf(typeof(MatchingResultPoint)) is false)
            throw new UnreachableException(
                $"expected first generic parameter to be of type {typeof(MatchingResultPoint)}");

        var dt = new DataTable();

        // create columns
        var properties = pointsType.GetProperties();
        foreach (var property in properties)
        {
            var columnAttribute = property.GetCustomAttribute<ExportColumnAttribute>();
            var column = columnAttribute is not null ? MapFromAttribute(columnAttribute) : MapFromProperty(property);
            dt.Columns.Add(column);
        }
        
        // add data
        foreach (var point in matchingResult.Points)
        {
            var row = dt.NewRow();

            // get row value for given column from attribute
            var pointProperties = point.GetType().GetProperties();
            var columnIdx = 0;
            foreach (var pointProperty in pointProperties)
            {
                row[columnIdx] = pointProperty.GetValue(point);
                columnIdx++;
            }

            dt.Rows.Add(row);
        }

        return dt;
    }

    private static DataColumn MapFromProperty(PropertyInfo property)
    {
        var column = new DataColumn();
        column.ColumnName = property.Name;
        column.DataType = property.PropertyType;
        return column;
    }

    private static DataColumn MapFromAttribute(ExportColumnAttribute columnAttribute)
    {
        var column = new DataColumn();
        column.ColumnName = columnAttribute.Name;
        column.DataType = columnAttribute.Type;
        return column;
    }
}
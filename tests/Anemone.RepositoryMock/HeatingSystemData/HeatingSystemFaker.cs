﻿using Anemone.Repository.HeatingSystemData;
using Bogus;

namespace Anemone.RepositoryMock.HeatingSystemData;

public static  class HeatingSystemFaker
{
   private static readonly Faker<HeatingSystem> _testHeatingSystem;
   private static readonly Faker<HeatingSystemPoint> _testHeatingSystemPoint;

   static HeatingSystemFaker()
   {
      _testHeatingSystemPoint = new Faker<HeatingSystemPoint>()
         .RuleFor(o => o.Type, f => f.Random.Enum<HeatingSystemPointType>())
         .RuleFor(o => o.TypeValue, f => f.Random.Double(0, 300e3))
         .RuleFor(o => o.Resistance, f => f.Random.Double(0, 100e-3))
         .RuleFor(o => o.Inductance, f => f.Random.Double(0, 50e-6));
      
      _testHeatingSystem = new Faker<HeatingSystem>()
         .RuleFor(o => o.Name, f => f.Commerce.ProductName())
         .RuleFor(o => o.HeatingSystemPoints, () => _testHeatingSystemPoint.Generate(20));
   }

   public static HeatingSystem GenerateHeatingSystem() => _testHeatingSystem.Generate();
   public static IEnumerable<HeatingSystem> GenerateHeatingSystem(int count) => _testHeatingSystem.Generate(count);

   public static HeatingSystemPoint GeneratePoint() => _testHeatingSystemPoint.Generate();
   public static IEnumerable<HeatingSystemPoint> GeneratePoint(int count) => _testHeatingSystemPoint.Generate(count);
}
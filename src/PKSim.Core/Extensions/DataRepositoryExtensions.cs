using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Extensions
{
   class ExtendedPropertyComparer : IEqualityComparer<IExtendedProperty>
   {
      private readonly Func<IExtendedProperty, object> _funcDistinct;

      public ExtendedPropertyComparer(Func<IExtendedProperty, object> funcDistinct)
      {
         _funcDistinct = funcDistinct;
      }

      public bool Equals(IExtendedProperty x, IExtendedProperty y)
      {
         return _funcDistinct(x).Equals(_funcDistinct(y));
      }

      public int GetHashCode(IExtendedProperty obj)
      {
         return _funcDistinct(obj).GetHashCode();
      }
   }

   public static class DataRepositoryExtensions
   {
      /// <summary>
      /// Gets the intersecting metadata for an enumeration of data repositories. The intersection is all metadata with keys contained in all repositories meta data
      /// </summary>
      /// <param name="dataRepositories">The repositories being scanned for intersecting metadata</param>
      /// <returns>An enumerable of IExtendedProperty which is a key-value pairing of metadata</returns>
      public static IEnumerable<IExtendedProperty> IntersectingMetaData(this IReadOnlyList<DataRepository> dataRepositories)
      {
         return dataRepositories.SelectMany(repository => repository.ExtendedProperties).
            Distinct(new ExtendedPropertyComparer(iExtendedProperty => iExtendedProperty.Name)).
            Where(x => dataRepositories.All(repos => repos.ExtendedProperties.Contains(x.Name))).
            Select(x => new ExtendedProperty<string>
            {
               Name = x.Name,
               Value = valueMapper(dataRepositories.SelectMany(repository => repository.ExtendedProperties).Where(iExtendedProperty => iExtendedProperty.Name.Equals(x.Name)), x)
            });
      }

      public static void AddColumns(this DataRepository repository, IEnumerable<DataColumn> columns)
      {
         columns.Each(repository.Add);
      }

      public static bool ColumnIsInRelatedColumns(this DataRepository repository, DataColumn column)
      {
         return repository.SelectMany(x => x.RelatedColumns).Contains(column);
      }

      private static string valueMapper(IEnumerable<IExtendedProperty> properties, IExtendedProperty value)
      {
         return valueMapper(properties, value.ValueAsObject.ToString());
      }

      private static string valueMapper(IEnumerable<IExtendedProperty> properties, string value)
      {
         return properties.All(x => x.ValueAsObject.ToString().Equals(value)) ? value : string.Empty;
      }
   }
}

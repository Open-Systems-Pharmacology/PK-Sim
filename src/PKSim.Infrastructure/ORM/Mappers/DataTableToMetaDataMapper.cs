using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Reflection;
using PKSim.Assets;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IDataTableToMetaDataMapper<T> : IMapper<DataTable, IEnumerable<T>> where T : new()
   {
   }

   public class DataTableToMetaDataMapper<T> : IDataTableToMetaDataMapper<T> where T : new()
   {
      private readonly ICache<PropertyInfo, SetHandler> _propertyCache;

      public DataTableToMetaDataMapper()
      {
         var allProperties = typeof(T).AllProperties().Where(propertyCanBeMapped);
         _propertyCache = new Cache<PropertyInfo, SetHandler>();
         allProperties.Each(x => _propertyCache.Add(x, DelegateFactory.CreateSet(x)));
      }

      private bool propertyCanBeMapped(PropertyInfo propertyInfo)
      {
         return propertyInfo.CanWrite;
      }

      public IEnumerable<T> MapFrom(DataTable dt)
      {
         checkDataTableStructure(dt);
         return from DataRow dr in dt.Rows select mapFrom(dr);
      }

      private T mapFrom(DataRow dataRow)
      {
         var newObj = new T();

         foreach (var propertyInfo in _propertyCache.Keys)
         {
            var value = dataRow[propertyInfo.Name];
            var propertyType = propertyInfo.PropertyType;
            //for DB null values, nullable target type is supposed
            //so just replace DBNull with null value.
            if (value == DBNull.Value)
               value = null;

            var setHandler = _propertyCache[propertyInfo];
            if (propertyType.IsAnImplementationOf<bool>())
            {
               if (value != null)
                  setHandler(newObj, int.Parse(value.ToString()) == 1);
            }
            else if (propertyType.IsAnImplementationOf<Enum>())
            {
               if (value != null)
                  setHandler(newObj, Enum.Parse(propertyType, value.ToString(), true));
            }
            else if ((propertyType.IsAnImplementationOf<int>() || propertyType.IsAnImplementationOf<int?>()) && value.IsAnImplementationOf<long>())
               setHandler(newObj, Convert.ToInt32(value));
            else
               setHandler(newObj, value);
         }

         return newObj;
      }

      /// <summary>
      ///    Check that datatable passed is OK for the property info list:
      ///    - Same named data column is available for every property
      /// </summary>
      private void checkDataTableStructure(DataTable dt)
      {
         //check that data table passed contains columns with exactly
         //the names of the properties
         var errorList = new List<string>();
         var allPropertyNames = _propertyCache.Keys.Select(x => x.Name).ToList();
         foreach (DataColumn dataColumn in dt.Columns)
         {
            if (!allPropertyNames.Contains(dataColumn.ColumnName))
               errorList.Add(PKSimConstants.Error.MissingColumnInView(dataColumn.ColumnName));
         }

         //remove keys that are not defined in query
         foreach (var property in _propertyCache.Keys.ToList())
         {
            if (!dt.Columns.Contains(property.Name))
               _propertyCache.Remove(property);
         }

         if (errorList.Count == 0)
            return;

         throw new ArgumentException($"{errorList.ToString("\n")} for type '{typeof(T).FullName}'");
      }
   }
}
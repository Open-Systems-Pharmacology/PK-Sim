using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using NUnit.Framework;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.Mappers;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure
{
   internal static class ObjectExtensions
   {
      public static T MemberwiseClone<T>(this T obj) where T : new()
      {
         var propertyInfos = typeof (T).AllProperties();
         var newObj = new T();

         foreach (var propertyInfo in propertyInfos)
            propertyInfo.SetValue(newObj, propertyInfo.GetValue(obj, null), null);

         return newObj;
      }

      public static bool AreAllMembersEqual<T>(this T obj1, T obj2)
      {
         var propertyInfos = typeof (T).AllProperties();

         foreach (var propertyInfo in propertyInfos)
         {
            if (!propertyCanBeMapped(propertyInfo))
               continue;

            object value1 = propertyInfo.GetValue(obj1, null);
            object value2 = propertyInfo.GetValue(obj2, null);

            if ((value1 == null) && (value2 == null))
               continue;

            if (((value1 == null) && (value2 != null)) ||
                ((value1 != null) && (value2 == null)))
               return false;

            if (!value1.Equals(value2))
               return false;
         }

         return true;
      }

      private static bool propertyCanBeMapped(PropertyInfo propertyInfo)
      {
         return true;
      }
   }

   public abstract class concern_for_DatatableToMetaDataMapper : ContextSpecification<IDataTableToMetaDataMapper<ParameterValueMetaData>>
   {
      protected IList<ParameterValueMetaData> _srcParamValueDefs;
      protected DataTable _dt;

      protected override void Context()
      {
         sut = new DataTableToMetaDataMapper<ParameterValueMetaData>();

         var pvd1 = new ParameterValueMetaData
            {
               CanBeVaried = false,
               ContainerName = "Cell",
               ContainerType = CoreConstants.ContainerType.Compartment,
               DefaultValue = 17.5,
               Dimension = "NO_DIM",
               GroupName = "123",
               BuildingBlockType = PKSimBuildingBlockType.Individual,
               ReadOnly = false,
               MaxIsAllowed = true,
               MaxValue = 343.12345678,
               MinIsAllowed = false,
               MinValue = -347.12e-2,
               ContainerId = 93,
               ParameterName = "P1",
               ParameterValueVersion = "DUMMY",
               Species = "Maulwurf",
               Visible = true
            };

         pvd1.ParentContainerPath = new ObjectPath(new[] { Constants.ROOT, Constants.ORGANISM, "Liver", "Cells" }).ToString();

         var pvd2 = pvd1.MemberwiseClone();
         pvd2.MinValue = null;
         pvd2.MaxValue = null;

         _srcParamValueDefs = new List<ParameterValueMetaData>();
         _srcParamValueDefs.Add(pvd1);
         _srcParamValueDefs.Add(pvd2);
         _dt = ListToTable(_srcParamValueDefs);
      }

      public DataTable ListToTable<T>(IEnumerable<T> rows)
      {
         var dt = new DataTable();
         var props = typeof (T).AllProperties().ToList();
         foreach (var p in props)
         {
            Type typeForDataTable = p.PropertyType;

            if (p.PropertyType == typeof (double?))
               typeForDataTable = typeof (double);

            if (p.PropertyType == typeof (bool))
               typeForDataTable = typeof (int);

            dt.Columns.Add(p.Name, typeForDataTable);
         }

         foreach (var r in rows)
         {
            var vals = new object[props.Count];
            for (int idx = 0; idx < vals.Length; idx++)
            {
               //special handling for bool values (set 0 or 1)
               if (props[idx].PropertyType == typeof (bool))
               {
                  vals[idx] = (bool) props[idx].GetValue(r, null) ? 1 : 0;
                  continue;
               }

               //special handling for nullable double
               if (props[idx].PropertyType == typeof (double?))
               {
                  var value = (double?) props[idx].GetValue(r, null);

                  if (!value.HasValue)
                  {
                     vals[idx] = DBNull.Value;
                     continue;
                  }
               }

               //all other cases - just set value from source
               vals[idx] = props[idx].GetValue(r, null);
            }
            dt.Rows.Add(vals);
         }
         return dt;
      }
   }

   public class when_converting_from_datatable : concern_for_DatatableToMetaDataMapper
   {
      protected IEnumerable<ParameterValueMetaData> _mappedParamDefs;

      protected override void Because()
      {
         _mappedParamDefs = sut.MapFrom(_dt);
      }

      [Observation]
      public void mapped_objects_should_contain_right_values()
      {
         int srcIdx = 0;
         foreach (var mappedParamValueDef in _mappedParamDefs)
         {
            var srcParamValueDef = _srcParamValueDefs[srcIdx];
            mappedParamValueDef.AreAllMembersEqual(srcParamValueDef).ShouldBeTrue();
            srcIdx += 1;
         }
      }
   }

   public class when_converting_from_invalid_datatable : concern_for_DatatableToMetaDataMapper
   {
      protected override void Context()
      {
         base.Context();
         _dt.Columns.Add("BLA BLA");
      }

      [Observation]
      public void should_throw_exception()
      {
         The.Action(() => sut.MapFrom(_dt)).ShouldThrowAn<Exception>();
      }
   }
}
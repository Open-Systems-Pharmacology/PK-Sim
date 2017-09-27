using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public class PivotPosition
   {
      public PivotArea Area { get; set; }
      public int Index { get; set; }
   }

   public class PopulationPivotAnalysis : PopulationAnalysis, IComparer<string>, IComparer<object>
   {
      private readonly ICache<IPopulationAnalysisField, PivotPosition> _fieldPositions;

      public PopulationPivotAnalysis()
      {
         _fieldPositions = new Cache<IPopulationAnalysisField, PivotPosition>(onMissingKey: x => newPosition(PivotArea.FilterArea, 0));
      }

      public override void Remove(IPopulationAnalysisField populationAnalysisField)
      {
         base.Remove(populationAnalysisField);
         _fieldPositions.Remove(populationAnalysisField);
      }

      public virtual void SetPosition(string fieldName, PivotArea? area, int? index)
      {
         SetPosition(FieldByName(fieldName), area, index);
      }

      public virtual void SetPosition(string fieldName, PivotPosition pivotPosition)
      {
         SetPosition(FieldByName(fieldName), pivotPosition);
      }

      public virtual void SetPosition(IPopulationAnalysisField field, PivotPosition pivotPosition)
      {
         if (field == null) return;
         _fieldPositions[field] = pivotPosition;
      }

      public virtual void SetPosition(IPopulationAnalysisField field, PivotArea area)
      {
         SetPosition(field, newPosition(area, AllFieldsOn(area).Count));
      }

      public virtual void SetPosition(IPopulationAnalysisField field, PivotArea? area, int? index)
      {
         SetPosition(field, newPosition(area, index));
      }

      private static PivotPosition newPosition(PivotArea? area, int? index)
      {
         var position = new PivotPosition();
         if (area != null)
            position.Area = area.Value;

         if (index != null)
            position.Index = index.Value;

         return position;
      }

      public virtual PivotPosition GetPosition(string fieldName)
      {
         return GetPosition(FieldByName(fieldName));
      }

      public virtual PivotPosition GetPosition(IPopulationAnalysisField field)
      {
         return _fieldPositions[field];
      }

      public virtual PivotArea GetArea(string fieldName)
      {
         return GetArea(FieldByName(fieldName));
      }

      public virtual PivotArea GetArea(IPopulationAnalysisField field)
      {
         return GetPosition(field).Area;
      }

      public virtual int GetAreaIndex(string fieldName)
      {
         return GetAreaIndex(FieldByName(fieldName));
      }

      public virtual int GetAreaIndex(IPopulationAnalysisField field)
      {
         return GetPosition(field).Index;
      }

      public virtual IReadOnlyList<IPopulationAnalysisField> AllFieldsOn(PivotArea area)
      {
         return AllFields.Where(f => GetArea(f).Is(area)).OrderBy(GetAreaIndex).ToList();
      }

      public virtual IReadOnlyList<IPopulationAnalysisField> AllFieldsOn(PivotArea area, Type fieldType)
      {
         return AllFieldsOn(area).Where(f => f.IsAnImplementationOf(fieldType)).ToList();
      }

      public virtual IReadOnlyList<T> AllFieldsOn<T>(PivotArea area) where T : IPopulationAnalysisField
      {
         return AllFieldsOn(area).OfType<T>().ToList();
      }

      public virtual IReadOnlyList<string> AllFieldNamesOn(PivotArea area)
      {
         return AllFieldsOn(area).Select(f => f.Name).ToList();
      }

      public virtual IReadOnlyList<string> StringFieldNamesOn(PivotArea area)
      {
         return AllFieldsOn<IStringValueField>(area).Select(f => f.Name).ToList();
      }

      public virtual ICache<IPopulationAnalysisField, PivotPosition> AllFieldPositions => _fieldPositions;

      /// <summary>
      ///    This compare method s used to give an ordering for PopulationAnalysisFields.
      /// </summary>
      /// <param name="x">Name of PopulationAnalysisField</param>
      /// <param name="y">Name of PopulationAnalysisField</param>
      /// <returns>Order information</returns>
      public int Compare(string x, string y)
      {
         if (GetArea(x) != GetArea(y))
            return 0;

         return Comparer<int>.Default.Compare(GetAreaIndex(x), GetAreaIndex(y));
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var pivotAnalysis = source as PopulationPivotAnalysis;
         if (pivotAnalysis == null) return;

         foreach (var field in pivotAnalysis.AllFields)
         {
            SetPosition(field.Name, pivotAnalysis.GetPosition(field));
         }
      }

      public int Compare(object x, object y)
      {
         return Compare(x.ToString(), y.ToString());
      }

      public virtual IStringValueField ColorField
      {
         get => firstOn<IStringValueField>(PivotArea.ColorArea);
         set => updateToUniqueAreaPosition(value, PivotArea.ColorArea);
      }

      private T firstOn<T>(PivotArea area) where T : IPopulationAnalysisField
      {
         return AllFieldsOn<T>(area).FirstOrDefault();
      }

      public virtual IStringValueField SymbolField
      {
         get => firstOn<IStringValueField>(PivotArea.SymbolArea);
         set => updateToUniqueAreaPosition(value, PivotArea.SymbolArea);
      }

      private void updateToUniqueAreaPosition(IPopulationAnalysisField field, PivotArea area)
      {
         var fieldsOnArea = AllFieldsOn(area);
         var firstAvailableFilterIndex = AllFieldsOn(PivotArea.FilterArea).Count;
         fieldsOnArea.Each(f => SetPosition(f, PivotArea.FilterArea, firstAvailableFilterIndex++));
         SetPosition(field, area, 0);
      }
   }
}
using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public abstract class PopulationAnalysis : IUpdatable, IVisitable<IVisitor>
   {
      private readonly List<IPopulationAnalysisField> _allPopulationAnalysisFields;

      protected PopulationAnalysis()
      {
         _allPopulationAnalysisFields = new List<IPopulationAnalysisField>();
      }

      public virtual IReadOnlyCollection<IPopulationAnalysisField> AllFields => _allPopulationAnalysisFields;

      public virtual void Add(IPopulationAnalysisField populationAnalysisField)
      {
         _allPopulationAnalysisFields.Add(populationAnalysisField);
         populationAnalysisField.PopulationAnalysis = this;
      }

      public virtual bool Has(IPopulationAnalysisField populationAnalysisField)
      {
         if (populationAnalysisField == null)
            return false;

         if (string.IsNullOrEmpty(populationAnalysisField.Id))
            return _allPopulationAnalysisFields.Contains(populationAnalysisField);

         return _allPopulationAnalysisFields.Any(x => string.Equals(x.Id, populationAnalysisField.Id));
      }

      public virtual void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         var populationAnalysis = source as PopulationAnalysis;
         populationAnalysis?.AllFields.Each(f => Add(cloneManager.Clone(f)));
      }

      public virtual bool Has(string fieldName)
      {
         return FieldByName(fieldName) != null;
      }

      public virtual void Remove(IPopulationAnalysisField populationAnalysisField)
      {
         if (populationAnalysisField == null)
            return;

         _allPopulationAnalysisFields.Remove(populationAnalysisField);
      }

      public virtual IPopulationAnalysisField FieldByName(string name)
      {
         return _allPopulationAnalysisFields.FindByName(name);
      }

      public virtual IEnumerable<TField> All<TField>() where TField : IPopulationAnalysisField
      {
         return All(typeof (TField), withDerived: false).Cast<TField>();
      }

      public virtual IEnumerable<IPopulationAnalysisField> All(Type fieldType, bool withDerived = false)
      {
         var allFields = _allPopulationAnalysisFields.Where(x => x.IsAnImplementationOf(fieldType)).ToList();

         if (withDerived == false || fieldType.IsAnImplementationOf<PopulationAnalysisDerivedField>())
            return allFields;

         //not a derived field but we need all derived fields using fiels of the given type
         var allDerivedFields = All<PopulationAnalysisDerivedField>();
         allFields.AddRange(allDerivedFields.Where(x => x.IsDerivedTypeFor(fieldType)));

         return allFields;
      }

      public virtual IEnumerable<IPopulationAnalysisField> All(IReadOnlyCollection<Type> fieldTypes, bool withDerived = false)
      {
         return fieldTypes.SelectMany(fieldType => All(fieldType, withDerived));
      }

      /// <summary>
      ///    Renames the field named <paramref name="oldFieldName" /> to <paramref name="newFieldName" />
      ///    and ensure that all derived fields and grouping depending on that field are updated as well
      /// </summary>
      public virtual void RenameField(string oldFieldName, string newFieldName)
      {
         var field = FieldByName(oldFieldName);
         if (field != null)
            field.Name = newFieldName;

         AllFields.OfType<PopulationAnalysisDerivedField>().Each(f => f.RenameReferencedField(oldFieldName, newFieldName));
      }

      /// <summary>
      ///    Returns all fields referenced (either directly or indirectly) by the <paramref name="derivedField" />.
      /// </summary>
      public virtual IReadOnlyCollection<IPopulationAnalysisField> AllFieldsReferencedBy(PopulationAnalysisDerivedField derivedField)
      {
         var set = new HashSet<IPopulationAnalysisField>();
         derivedField.ReferencedFieldNames.Each(name => set.Add(FieldByName(name)));

         foreach (var subSet in set.OfType<PopulationAnalysisDerivedField>().ToList().Select(AllFieldsReferencedBy))
         {
            subSet.Each(f => set.Add(f));
         }
         return set.ToList();
      }

      /// <summary>
      ///    Returns all fields referencing (either directly or indirectly) the <paramref name="populationAnalysisField" />.
      /// </summary>
      public virtual IReadOnlyCollection<IPopulationAnalysisField> AllFieldsReferencing(IPopulationAnalysisField populationAnalysisField)
      {
         var set = new HashSet<IPopulationAnalysisField>();
         foreach (var derivedField in AllFields.OfType<PopulationAnalysisDerivedField>())
         {
            if (AllFieldsReferencedBy(derivedField).Contains(populationAnalysisField))
               set.Add(derivedField);
         }
         return set.ToList();
      }

      public virtual void AcceptVisitor(IVisitor visitor)
      {
         visitor.Visit(this);
      }
   }
}
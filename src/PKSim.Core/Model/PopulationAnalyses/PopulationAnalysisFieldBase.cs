using System;
using System.Collections;
using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;
using PKSim.Assets;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public interface IPopulationAnalysisField : INotifier, IWithName, IWithDescription, IUpdatable, IValidatable, IComparer<object>
   {
      string Id { get; }
      Type DataType { get; }
      PopulationAnalysis PopulationAnalysis { get; set; }
   }

   public abstract class PopulationAnalysisFieldBase : Notifier, IPopulationAnalysisField
   {
      public Type DataType { get; }
      public IBusinessRuleSet Rules { get; }
      public string Name { get; set; }
      public string Description { get; set; }
      public abstract string Id { get; }

      /// <summary>
      ///    Reference to the population analysis containing the field
      /// </summary>
      public PopulationAnalysis PopulationAnalysis { get; set; }

      protected PopulationAnalysisFieldBase(Type dataType)
      {
         DataType = dataType;
         Rules = new BusinessRuleSet(AllRules.All);
      }

      /// <summary>
      ///    This method defines a sorter for values of the field.
      /// </summary>
      public virtual int Compare(object value1, object value2)
      {
         return Comparer.Default.Compare(value1, value2);
      }

      public virtual void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         var field = source as PopulationAnalysisFieldBase;
         if (field == null) return;
         Name = field.Name;
         Description = field.Description;
      }

      private static class AllRules
      {
         public static IEnumerable<IBusinessRule> All
         {
            get
            {
               yield return nameNotEmpty;
               yield return nameUnique;
            }
         }

         private static IBusinessRule nameNotEmpty
         {
            get { return GenericRules.NonEmptyRule<PopulationAnalysisFieldBase>(x => x.Name); }
         }

         private static IBusinessRule nameUnique
         {
            get
            {
               return CreateRule.For<PopulationAnalysisFieldBase>()
                  .Property(x => x.Name)
                  .WithRule((field, name) =>
                  {
                     var populationAnalysis = field.PopulationAnalysis;

                     var otherField = populationAnalysis?.FieldByName(name);
                     if (otherField == null)
                        return true;

                     return otherField == field;
                  })
                  .WithError((field, name) => PKSimConstants.Error.NameAlreadyExistsInContainerType(name, PKSimConstants.ObjectTypes.PopulationAnalysis));
            }
         }
      }
   }
}
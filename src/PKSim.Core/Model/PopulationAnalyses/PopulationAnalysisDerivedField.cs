using System;
using System.Collections.Generic;
using OSPSuite.Utility;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public abstract class PopulationAnalysisDerivedField : PopulationAnalysisFieldBase
   {
      private readonly ShortGuid _id;

      protected PopulationAnalysisDerivedField(Type dataType) : base(dataType)
      {
         _id = ShortGuid.NewGuid();
      }

      public override string Id
      {
         get { return _id; }
      }

      public abstract string Expression { get; }

      /// <summary>
      ///    Returns true if this is derived field for a field of type <typeparamref name="T" /> otherwise false.
      /// </summary>
      public virtual bool IsDerivedTypeFor<T>()
      {
         return IsDerivedTypeFor(typeof (T));
      }

      /// <summary>
      ///    Returns true if this is derived field for a field of type <paramref name="fieldType" /> otherwise false.
      /// </summary>
      public abstract bool IsDerivedTypeFor(Type fieldType);

      /// <summary>
      ///    Renames any referenced field used named <paramref name="oldFieldName" /> to <paramref name="newFieldName" />.
      ///    Does not rename anything if the field is not being used
      /// </summary>
      public abstract void RenameReferencedField(string oldFieldName, string newFieldName);

      /// <summary>
      ///    Returns the names of all fields referenced bu the this derived field
      /// </summary>
      public abstract IReadOnlyCollection<string> ReferencedFieldNames { get; }

      /// <summary>
      ///    Returns true if the derived field can be used for a data field whose data are of type <paramref name="dataType" />
      ///    otherwise false
      /// </summary>
      public abstract bool CanBeUsedFor(Type dataType);

      public virtual void UpdateExpression(IPopulationDataCollector populationDataCollector)
      {
         //nothing to do here
      }
   }
}
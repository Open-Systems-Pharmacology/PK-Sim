using System.Collections.Generic;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    Configuration for one specific compound
   /// </summary>
   public class CompoundProperties : IWithCalculationMethods
   {
      /// <summary>
      ///    Reference to the compound in the simulation for which the settings apply
      /// </summary>
      public virtual Compound Compound { get; set; }

      /// <summary>
      ///    Reference to parent simulation
      /// </summary>
      public virtual Simulation Simulation { get; set; }

      private readonly IList<CompoundGroupSelection> _compoundGroupSelections;
      public virtual CalculationMethodCache CalculationMethodCache { get; private set; }
      public virtual CompoundProcessesSelection Processes { get; set; }

      public bool AnyProcessesDefined => Processes.Any();

      /// <summary>
      ///    Protocol configuration such as formulation mapping for formulation keys
      /// </summary>
      public virtual ProtocolProperties ProtocolProperties { get; set; }

      public CompoundProperties()
      {
         _compoundGroupSelections = new List<CompoundGroupSelection>();
         Processes = new CompoundProcessesSelection();
         CalculationMethodCache = new CalculationMethodCache();
         ProtocolProperties = new ProtocolProperties();
      }

      public virtual CompoundProperties Clone(ICloneManager cloneManager)
      {
         var clone = new CompoundProperties
         {
            CalculationMethodCache = CalculationMethodCache.Clone(),
            Processes = Processes.Clone(cloneManager),
            ProtocolProperties = ProtocolProperties.Clone(),

            //do not clone: simply update reference that should be changed if required
            Compound = Compound
         };
         CompoundGroupSelections.Each(clone.AddCompoundGroupSelection);
         return clone;
      }

      public virtual void AddCompoundGroupSelection(CompoundGroupSelection compoundGroupSelection)
      {
         _compoundGroupSelections.Add(compoundGroupSelection);
      }

      public virtual void ClearGroupMapping()
      {
         _compoundGroupSelections.Clear();
      }

      public virtual IEnumerable<CompoundGroupSelection> CompoundGroupSelections => _compoundGroupSelections;

      public bool IsAdministered => ProtocolProperties.IsAdministered;
   }
}
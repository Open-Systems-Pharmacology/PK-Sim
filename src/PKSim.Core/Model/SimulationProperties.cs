using System.Collections.Generic;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{

   public class SimulationProperties
   {
      private readonly List<CompoundProperties> _compoundPropertiesList;
      private Simulation _simulation;

      /// <summary>
      ///    Model Configuration
      /// </summary>
      public virtual ModelProperties ModelProperties { get; set; }

      /// <summary>
      ///    Compound configuration such as group mapping, processes selection etc
      /// </summary>
      public virtual IReadOnlyList<CompoundProperties> CompoundPropertiesList
      {
         get { return _compoundPropertiesList; }
      }

      public void AddCompoundProperties(CompoundProperties compoundProperties)
      {
         _compoundPropertiesList.Add(compoundProperties);
         compoundProperties.Simulation = Simulation;
      }

      public virtual void ClearCompoundPropertiesList()
      {
         _compoundPropertiesList.Clear();
      }

      /// <summary>
      ///    Events configuration
      /// </summary>
      public virtual EventProperties EventProperties { get; set; }

      /// <summary>
      /// Interaction configuration (Inhibition, Induction etc.)
      /// </summary>
      public virtual InteractionProperties InteractionProperties { get; set; }

      /// <summary>
      ///    Returns true if the subject should age during simulation otherwise false. Default is false
      /// </summary>
      public virtual bool AllowAging { get; set; }

      public virtual Origin Origin { get; set; }

      /// <summary>
      ///    Reference to simulation with this properties
      /// </summary>
      public virtual Simulation Simulation
      {
         get { return _simulation; }
         set
         {
            _simulation = value;
            _compoundPropertiesList.Each(cp => cp.Simulation = Simulation);
         }
      }

      public SimulationProperties()
      {
         ModelProperties = new ModelProperties();
         _compoundPropertiesList = new List<CompoundProperties>();
         EventProperties = new EventProperties();
         InteractionProperties = new InteractionProperties();
         AllowAging = false;
         Origin = Origins.PKSim;
      }

      /// <summary>
      ///    Returns a clone of the current simulation properties
      /// </summary>
      public virtual SimulationProperties Clone(ICloneManager cloneManager)
      {
         var clone = new SimulationProperties
         {
            ModelProperties = ModelProperties.Clone(cloneManager),
            EventProperties = EventProperties.Clone(cloneManager),
            InteractionProperties = InteractionProperties.Clone(cloneManager),
            AllowAging = AllowAging,
            Origin = Origin
         };

         _compoundPropertiesList.Each(cp => clone.AddCompoundProperties(cp.Clone(cloneManager)));
         return clone;
      }
   }
}
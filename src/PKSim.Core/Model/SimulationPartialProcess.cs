using PKSim.Assets;

namespace PKSim.Core.Model
{
   public enum SimulationPartialProcessStatus
   {
      ProcessNotSelected,
      CanBeUsedInSimulation
   }

   public class SimulationPartialProcess
   {
      /// <summary>
      ///    Selected Process in compound
      /// </summary>
      public virtual PartialProcess CompoundProcess { get; set; }

      /// <summary>
      ///    Selected Protein in individual
      /// </summary>
      public virtual IndividualMolecule IndividualMolecule { get; set; }

      /// <summary>
      ///    Partial process mapping that was used. This is only set when loading a simulation and null otherwise
      /// </summary>
      public virtual IProcessMapping PartialProcessMapping { get; set; }

      /// <summary>
      ///    Status of mapping
      /// </summary>
      public virtual SimulationPartialProcessStatus Status
      {
         get
         {
            if (string.Equals(CompoundProcess.MoleculeName, PKSimConstants.UI.None))
               return SimulationPartialProcessStatus.ProcessNotSelected;

            return SimulationPartialProcessStatus.CanBeUsedInSimulation;
         }
      }

      /// <summary>
      ///    Name of process used in the compound
      /// </summary>
      public virtual string ProcessName => CompoundProcess.Name;

      public virtual string CompoundName
      {
         get
         {
            if (CompoundProcess.ParentCompound == null)
               return string.Empty;

            return CompoundProcess.ParentCompound.Name;
         }
      }

      /// <summary>
      ///    Name of molecule used in the individual
      /// </summary>
      public virtual string MoleculeName => IndividualMolecule.Name;
   }
}
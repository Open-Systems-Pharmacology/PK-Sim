using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class ProcessSelection : IReactionMapping, IUpdatable
   {
      public string MoleculeName { get; set; }
      public string CompoundName { get; set; }
      public string ProcessName { get; set; }

      public string ProductName(string productNameTemplate)
      {
         return CoreConstants.Molecule.ProcessProductName(CompoundName, ProcessName, productNameTemplate);
      }

      public virtual ProcessSelection Clone(ICloneManager cloneManager)
      {
         var clone = new ProcessSelection();
         clone.UpdatePropertiesFrom(this, cloneManager);
         return clone;
      }

      public virtual void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         var sourceProcessSelection = source as ProcessSelection;
         if (sourceProcessSelection == null) return;
         ProcessName = sourceProcessSelection.ProcessName;
         MoleculeName = sourceProcessSelection.MoleculeName;
         CompoundName = sourceProcessSelection.CompoundName;
      }
   }
}
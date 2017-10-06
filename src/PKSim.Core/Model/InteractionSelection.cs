using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class InteractionSelection : IProcessMapping
   {
      public virtual string ProcessName { get; set; }
      public virtual string MoleculeName { get; set; }
      public virtual string CompoundName { get; set; }

      public virtual InteractionSelection Clone(ICloneManager cloneManager)
      {
         return new InteractionSelection { ProcessName = ProcessName, MoleculeName = MoleculeName, CompoundName = CompoundName };
      }

      /// <summary>
      /// Returns <c>true</c> if the <paramref name="interactionProcess"/> matches the compound and process used in this selection otherwise <c>false</c>
      /// </summary>
      public virtual bool Matches(InteractionProcess interactionProcess)
      {
         return string.Equals(ProcessName, interactionProcess.Name) &&
                string.Equals(CompoundName, interactionProcess.ParentCompound.Name);
      }
   }
}
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class SystemicProcessSelection : IReactionMapping
   {
      public SystemicProcessType ProcessType { get; set; }
      public string ProcessName { get; set; }
      public string CompoundName { get; set; }

      public SystemicProcessSelection Clone(ICloneManager cloneManager)
      {
         return new SystemicProcessSelection {ProcessName = ProcessName, CompoundName = CompoundName, ProcessType = ProcessType};
      }

      public string ProductName(string productNameTemplate)
      {
         return $"{CompoundName}-{MoleculeName} {productNameTemplate}";
      }

      public string MoleculeName
      {
         get
         {
            if (ProcessType == SystemicProcessTypes.Hepatic)
               return CoreConstants.Molecule.UndefinedLiver;

            if (ProcessType == SystemicProcessTypes.Biliary)
               return CoreConstants.Molecule.UndefinedLiverTransporter;

            return string.Empty;
         }
         set
         {
            /*nothing to do here*/
         }
      }
   }
}
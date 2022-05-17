using NPOI.HSSF.Util;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;

namespace PKSim.Presentation.DTO.Snapshots
{
   public class LoadFromSnapshotDTO : ValidatableDTO
   {
      private string _snapshotFile;
      //By default, always run simulations
      private bool _runSimulations = true;

      public virtual string SnapshotFile
      {
         get => _snapshotFile;
         set => SetProperty(ref _snapshotFile, value);
      }

      public virtual bool RunSimulations
      {
         get => _runSimulations;
         set => SetProperty(ref _runSimulations, value);
      }

      public LoadFromSnapshotDTO()
      {
         Rules.Add(AllRules.FileExists);
      }

      private static class AllRules
      {
         public static IBusinessRule FileExists { get; } = GenericRules.FileExists<LoadFromSnapshotDTO>(x => x.SnapshotFile);
      }
   }
}
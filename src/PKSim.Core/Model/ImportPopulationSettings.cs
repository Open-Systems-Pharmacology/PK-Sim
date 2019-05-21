using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class ImportPopulationSettings
   {
      private readonly List<PopulationImportFile> _allFiles;
      public Individual BaseIndividual { get; set; }

      public ImportPopulationSettings()
      {
         _allFiles = new List<PopulationImportFile>();
      }

      public void AddFile(PopulationImportFile populationFile)
      {
         _allFiles.Add(populationFile);
      }

      public IReadOnlyCollection<PopulationImportFile> AllFiles => _allFiles.ToList();

      public ImportPopulationSettings Clone(ICloneManager cloneManager)
      {
         var settings = new ImportPopulationSettings
            {
               BaseIndividual = cloneManager.Clone(BaseIndividual)
            };

         _allFiles.Each(f=>settings.AddFile(f.Clone()));
         return settings;
      }

      public override string ToString()
      {
         return PKSimConstants.UI.PopulationSettings;
      }

   }
}
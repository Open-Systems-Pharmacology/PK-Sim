using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Validation;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Populations
{
   public class ImportPopulationSettingsDTO : ValidatableDTO
   {
      public NotifyList<PopulationFileSelectionDTO> PopulationFiles { get; private set; }

      //Based Individual used to create the population
      public virtual PKSim.Core.Model.Individual Individual { get; set; }

      public ImportPopulationSettingsDTO()
      {
         PopulationFiles = new NotifyList<PopulationFileSelectionDTO>();
         Rules.AddRange(AllRules.All());
      }

      private static class AllRules
      {
         private static IBusinessRule atLeastOneFileDefined
         {
            get
            {
               return CreateRule.For<ImportPopulationSettingsDTO>()
                  .Property(item => item.PopulationFiles)
                  .WithRule((item, files) => files.Any())
                  .WithError((item, files) => PKSimConstants.Error.AtLeastOneFileRequiredToStartPopulationImport);
            }
         }

         private static IBusinessRule individualDefined
         {
            get
            {
               return CreateRule.For<ImportPopulationSettingsDTO>()
                  .Property(item => item.Individual)
                  .WithRule((item, ind) => ind != null)
                  .WithError((dto, ind) => PKSimConstants.Error.BuildingBlockNotDefined(PKSimConstants.ObjectTypes.Individual));
            }
         }

         internal static IEnumerable<IBusinessRule> All()
         {
            yield return atLeastOneFileDefined;
            yield return individualDefined;
         }
      }

      public void AddFile(string fileFullPath)
      {
         PopulationFiles.Add(new PopulationFileSelectionDTO {FilePath = fileFullPath});
      }

      public void AddFile(PopulationFileSelectionDTO populationFileSelectionDTO)
      {
         PopulationFiles.Add(populationFileSelectionDTO);
      }

      public void RemoveFile(PopulationFileSelectionDTO populationFileSelectionDTO)
      {
         PopulationFiles.Remove(populationFileSelectionDTO);
      }
   }
}
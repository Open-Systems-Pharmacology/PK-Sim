using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility;
using OSPSuite.Utility.Validation;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Simulations
{
   public class ImportPopulationSimulationDTO : ImportFileSelectionDTO
   {
      private PKSim.Core.Model.Population _population;
      private PopulationImportMode _populationImportMode;

      public ImportPopulationSimulationDTO()
      {
         Rules.AddRange(AllRules.All());
      }

      public PKSim.Core.Model.Population Population
      {
         get { return _population; }
         set
         {
            _population = value;
            OnPropertyChanged(() => Population);
         }
      }

      public PopulationImportMode PopulationImportMode
      {
         get { return _populationImportMode; }
         set
         {
            _populationImportMode = value;
            //Trigger other value changes depending on mode to refresh ui
            OnPropertyChanged(() => PopulationImportMode);
            OnPropertyChanged(() => PopulationFile);
            OnPropertyChanged(() => NumberOfIndividuals);
            OnPropertyChanged(() => Population);
         }
      }

      private string _populationFile;

      public string PopulationFile
      {
         get { return _populationFile; }
         set
         {
            _populationFile = value;
            OnPropertyChanged(() => PopulationFile);
         }
      }

      private int _numberOfIndividuals;

      public int NumberOfIndividuals
      {
         get { return _numberOfIndividuals; }
         set
         {
            _numberOfIndividuals = value;
            OnPropertyChanged(() => NumberOfIndividuals);
         }
      }

      private static class AllRules
      {
         private static IBusinessRule populationFileExists
         {
            get
            {
               return CreateRule.For<ImportPopulationSimulationDTO>()
                  .Property(item => item.PopulationFile)
                  .WithRule((item, file) => item.PopulationImportMode != PopulationImportMode.File || FileHelper.FileExists(file))
                  .WithError((item, file) => PKSimConstants.Error.FileDoesNotExist(file));
            }
         }

         private static IBusinessRule populationDefined
         {
            get
            {
               return CreateRule.For<ImportPopulationSimulationDTO>()
                  .Property(item => item.Population)
                  .WithRule((item, bb) => item.PopulationImportMode != PopulationImportMode.BuildingBlock || bb != null)
                  .WithError((item, bb) => PKSimConstants.Error.BuildingBlockNotDefined(PKSimConstants.ObjectTypes.Population));
            }
         }

         private static IBusinessRule numberOfIndividualsBiggerThan2
         {
            get
            {
               return CreateRule.For<ImportPopulationSimulationDTO>()
                  .Property(item => item.NumberOfIndividuals)
                  .WithRule((item, size) => item.PopulationImportMode != PopulationImportMode.Size || size >= 2)
                  .WithError(PKSimConstants.Rules.Parameter.NumberOfIndividualShouldBeBiggerThan2);
            }
         }

         internal static IEnumerable<IBusinessRule> All()
         {
            yield return populationFileExists;
            yield return populationDefined;
            yield return numberOfIndividualsBiggerThan2;
         }
      }
   }
}
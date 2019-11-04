using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IPopulationExportTask
   {
      /// <summary>
      ///    Export the given <paramref name="population" /> to <paramref name="fileFullPath" />
      /// </summary>
      void ExportToCSV(Population population, string fileFullPath);

      /// <summary>
      ///    Export the given <paramref name="population" /> to <paramref name="fileSelection" />
      /// </summary>
      void ExportToCSV(Population population, FileSelection fileSelection);

      /// <summary>
      ///    Returns a DataTable containing one row per variable parameter in the <paramref name="population" /> and one column
      ///    per individual
      ///    The First Columns contains the consolidated path (Starting with Organism for instance) of the parameter
      ///    <param name="population">The population to export</param>
      ///    <param name="includeUnitsInHeader">If true, then the column header will include the parameter base unit</param>
      /// </summary>
      DataTable CreatePopulationDataFor(Population population, bool includeUnitsInHeader = false);

      /// <summary>
      ///    Export the given <paramref name="populationSimulation" /> to <paramref name="fileFullPath" />
      /// </summary>
      void ExportToCSV(PopulationSimulation populationSimulation, string fileFullPath);

      /// <summary>
      ///    Export the given <paramref name="populationSimulation" /> to <paramref name="fileSelection" />
      /// </summary>
      void ExportToCSV(PopulationSimulation populationSimulation, FileSelection fileSelection);

      /// <summary>
      ///    Returns a DataTable containing one row per advanced parameter in the <paramref name="populationSimulation" /> as
      ///    well as all
      ///    variable parameters defined in the underlying population. One column is created per individual
      ///    The First Columns contains the consolidated path (Starting with Organism for instance) of the parameter.
      ///    <param name="populationSimulation">The simulation using the population to export</param>
      ///    <param name="includeUnitsInHeader">If true, then the column header will include the parameter base unit</param>
      /// </summary>
      DataTable CreatePopulationDataFor(PopulationSimulation populationSimulation, bool includeUnitsInHeader = false);

      IReadOnlyList<string> CreateProjectMetaInfoFrom(string description = null);
   }

   public class PopulationExportTask : IPopulationExportTask
   {
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IProjectRetriever _projectRetriever;
      private readonly IPKSimConfiguration _configuration;

      public PopulationExportTask(
         IEntityPathResolver entityPathResolver,
         ILazyLoadTask lazyLoadTask,
         IProjectRetriever projectRetriever,
         IPKSimConfiguration configuration
      )
      {
         _entityPathResolver = entityPathResolver;
         _lazyLoadTask = lazyLoadTask;
         _projectRetriever = projectRetriever;
         _configuration = configuration;
      }

      public void ExportToCSV(Population population, FileSelection fileSelection)
      {
         exportVectorialParametersContainerToCSV(population, x => CreatePopulationDataFor(x, includeUnitsInHeader: true), fileSelection);
      }

      public void ExportToCSV(Population population, string fileFullPath)
      {
         exportVectorialParametersContainerToCSV(population, x => CreatePopulationDataFor(x, includeUnitsInHeader: true), fileFullPath);
      }

      public void ExportToCSV(PopulationSimulation populationSimulation, FileSelection fileSelection)
      {
         exportVectorialParametersContainerToCSV(populationSimulation, x => CreatePopulationDataFor(x, includeUnitsInHeader: true), fileSelection);
      }

      public void ExportToCSV(PopulationSimulation populationSimulation, string fileFullPath)
      {
         exportVectorialParametersContainerToCSV(populationSimulation, x => CreatePopulationDataFor(x, includeUnitsInHeader: true), fileFullPath);
      }

      private void exportVectorialParametersContainerToCSV<T>(T advancedParameterContainer, Func<T, DataTable> createData, FileSelection fileSelection) where T : IAdvancedParameterContainer
      {
         if (fileSelection == null)
            return;

         exportVectorialParametersContainerToCSV(advancedParameterContainer, createData, fileSelection.FilePath, fileSelection.Description);
      }

      private void exportVectorialParametersContainerToCSV<T>(T advancedParameterContainer, Func<T, DataTable> createData, string fileFullPath, string fileDescription = null) where T : IAdvancedParameterContainer
      {
         var dataTable = createData(advancedParameterContainer);
         dataTable.ExportToCSV(fileFullPath, comments: CreateProjectMetaInfoFrom(fileDescription));
      }

      public IReadOnlyList<string> CreateProjectMetaInfoFrom(string description = null)
      {
         var metaInfo = new List<string> {$"Project: {_projectRetriever.ProjectName}", $"{CoreConstants.PRODUCT_NAME} version: {_configuration.FullVersion}"};
         if (!string.IsNullOrEmpty(description))
            metaInfo.Add(description);

         return metaInfo;
      }

      public DataTable CreatePopulationDataFor(Population population, bool includeUnitsInHeader = false)
      {
         _lazyLoadTask.Load(population);
         var dataTable = new DataTable(population.Name);
         dataTable.BeginLoadData();

         //Create one column for the parameter path
         addCovariates(population, dataTable);

         //add advanced parameters
         var allAdvancedParameters = population.AllAdvancedParameters(_entityPathResolver).ToList();
         var parametersToExport = population.AllVectorialParameters(_entityPathResolver);

         //do not take the one that should never be exported
         parametersToExport = parametersToExport.Where(p => parameterShouldBeExported(p, allAdvancedParameters));

         parametersToExport.Each(p => addParameterToTable(population, dataTable, p, includeUnitsInHeader));
         dataTable.EndLoadData();
         return dataTable;
      }

      private void addCovariates(Population population, DataTable dataTable)
      {
         var individualIds = Enumerable.Range(0, population.NumberOfItems).ToList();

         //add individual ids column
         individualIds.Each(i => dataTable.Rows.Add(dataTable.NewRow()));
         addColumnValues(population, dataTable, Constants.Population.INDIVIDUAL_ID_COLUMN, individualIds);

         //and one column for each individual in the population
         foreach (var covariate in population.AllCovariateNames)
         {
            if (covariate == CoreConstants.Covariates.GENDER)
               addColumnValues(population, dataTable, CoreConstants.Parameters.GENDER, population.AllGenders.Select(x => x.Index).ToList());
            else if (covariate == CoreConstants.Covariates.RACE)
               addColumnValues(population, dataTable, CoreConstants.Parameters.RACE_INDEX, population.AllRaces.Select(x => x.RaceIndex).ToList());
            else
               addColumnValues(population, dataTable, covariate, population.AllCovariateValuesFor(covariate));
         }
      }

      private void addColumnValues<T>(IVectorialParametersContainer parameterContainer, DataTable dataTable, string columnName, IReadOnlyList<T> allValues)
      {
         dataTable.AddColumn<T>(columnName);
         for (int i = 0; i < parameterContainer.NumberOfItems; i++)
         {
            dataTable.Rows[i][columnName] = allValues[i];
         }
      }

      private bool parameterShouldBeExported(IParameter parameter, IEnumerable<IParameter> advancedParameters)
      {
         //BMI and BodyWeight should always be exported
         if (parameter.NameIsOneOf(CoreConstants.Parameters.BMI, CoreConstants.Parameters.WEIGHT)) return true;

         //BMI MeanHeight MeanWeight should never be exported
         if (parameter.NameIsOneOf(CoreConstants.Parameters.MEAN_WEIGHT, CoreConstants.Parameters.MEAN_HEIGHT)) return false;

         //distribution parameter search as mean, std, gsd etc should not be exported
         if (CoreConstants.Parameters.AllDistributionParameters.Contains(parameter.Name)) return false;

         //advanced parameters should always be exported
         if (advancedParameters.Contains(parameter))
            return true;

         return !parameter.Formula.IsExplicit();
      }

      public DataTable CreatePopulationDataFor(PopulationSimulation populationSimulation, bool includeUnitsInHeader = false)
      {
         _lazyLoadTask.Load(populationSimulation);
         var population = populationSimulation.Population;
         //retrieve table for population
         var dataTable = CreatePopulationDataFor(population, includeUnitsInHeader);

         //add advanced parameters
         populationSimulation.AllAdvancedParameters(_entityPathResolver).Each(p => addParameterToTable(populationSimulation, dataTable, p, includeUnitsInHeader));

         return dataTable;
      }

      private void addParameterToTable(IVectorialParametersContainer parameterContainer, DataTable dataTable, IParameter parameter, bool includeUnitsInHeader)
      {
         //some path have changed and the parameter is not found anymore
         if (parameter == null) return;

         var parameterPath = _entityPathResolver.PathFor(parameter);

         var columnName = includeUnitsInHeader ? Constants.NameWithUnitFor(parameterPath, parameter.Dimension.BaseUnit.Name) : parameterPath;

         addColumnForParameterToTable(parameterContainer, dataTable, parameterPath, columnName);
      }

      private void addColumnForParameterToTable(IVectorialParametersContainer parameterContainer, DataTable dataTable, string parameterPath, string columnName)
      {
         addColumnValues(parameterContainer, dataTable, columnName, parameterContainer.AllValuesFor(parameterPath));
      }
   }
}
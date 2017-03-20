using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Core.Extensions;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using OSPSuite.Utility.Format;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class RandomPopulationSettingsTeXBuilder : OSPSuiteTeXBuilder<RandomPopulationSettings>
   {
      private readonly ITeXBuilderRepository _builderRepository;
      private readonly IRepresentationInfoRepository _infoRepository;
      private readonly NumericFormatter<double> _formatter;

      public RandomPopulationSettingsTeXBuilder(ITeXBuilderRepository builderRepository, IRepresentationInfoRepository infoRepository)
      {
         _builderRepository = builderRepository;
         _infoRepository = infoRepository;
         _formatter = new NumericFormatter<double>(NumericFormatterOptions.Instance);
      }

      public override void Build(RandomPopulationSettings populationSettings, OSPSuiteTracker tracker)
      {
         var objectToReports = new List<object>
            {
               new SubSection(PKSimConstants.UI.PopulationProperties),
               this.ReportValue(PKSimConstants.UI.NumberOfIndividuals,populationSettings.NumberOfIndividuals)
            };
         objectToReports.AddRange(populationSettings.GenderRatios.SelectMany(reportFor));
        
         objectToReports.Add(new SubSection(PKSimConstants.UI.PopulationParameterRanges));
         objectToReports.Add(parameterRangeTable(populationSettings.ParameterRanges));
         _builderRepository.Report(objectToReports, tracker);
      }

      private object parameterRangeTable(IEnumerable<ParameterRange> parameterRanges)
      {
         var dataTable = new DataTable { TableName = PKSimConstants.UI.PopulationParameterRanges };
         dataTable.AddColumn(PKSimConstants.UI.Parameter);
         dataTable.AddColumn(PKSimConstants.UI.MinValue);
         dataTable.AddColumn(PKSimConstants.UI.MaxValue);
         dataTable.AddColumn(PKSimConstants.UI.Unit);

         foreach (var parameterRange in parameterRanges)
         {
            var row = dataTable.NewRow();
            row[PKSimConstants.UI.Parameter] = parameterRange.ParameterDisplayName;
            row[PKSimConstants.UI.MinValue] = parameterRange.MinValueInDisplayUnit;
            row[PKSimConstants.UI.MaxValue] = parameterRange.MaxValueInDisplayUnit;
            row[PKSimConstants.UI.Unit] = parameterRange.Unit;

            dataTable.Rows.Add(row);
         }

         return dataTable;
      }

      private IEnumerable<object> reportFor(GenderRatio genderRatio)
      {
         yield return this.ReportValue(_infoRepository.DisplayNameFor(genderRatio.Gender),string.Format("{0}%", _formatter.Format(genderRatio.Ratio)));
      }
   }
}
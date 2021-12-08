using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Format;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class OriginDataReportBuilder : ReportBuilder<OriginData>
   {
      private readonly IReportGenerator _reportGenerator;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly NumericFormatter<double> _formatter;

      public OriginDataReportBuilder(
         IReportGenerator reportGenerator, 
         IDimensionRepository dimensionRepository, 
         IRepresentationInfoRepository representationInfoRepository)     
      {
         _reportGenerator = reportGenerator;
         _dimensionRepository = dimensionRepository;
         _representationInfoRepository = representationInfoRepository;
         _formatter = new NumericFormatter<double>(NumericFormatterOptions.Instance);
      }

      protected override void FillUpReport(OriginData originData, ReportPart reportPart)
      {
         var populationProperties = new TablePart(keyName: PKSimConstants.UI.PopulationProperties, valueName: PKSimConstants.UI.Value) {Title = PKSimConstants.UI.PopulationProperties};
         populationProperties.AddIs(PKSimConstants.UI.Species, originData.Species.DisplayName);
         populationProperties.AddIs(PKSimConstants.UI.Population, originData.Population.DisplayName);
         populationProperties.AddIs(PKSimConstants.UI.Gender, originData.Gender.DisplayName);

         var individualProperties = new TablePart(PKSimConstants.UI.IndividualParameters, PKSimConstants.UI.Value, PKSimConstants.UI.Unit)
         {
            Title = PKSimConstants.UI.IndividualParameters,
            Types =
            {
               [PKSimConstants.UI.Value] = typeof(double)
            }
         };

         if (originData.Population.IsAgeDependent)
         {
            if (originData.Age != null)
               individualProperties.AddIs(PKSimConstants.UI.Age, displayValueFor(originData.Age), originData.Age.Unit);

            if (originData.Population.IsPreterm && originData.GestationalAge != null)
               individualProperties.AddIs(PKSimConstants.UI.GestationalAge, displayValueFor(originData.GestationalAge), originData.GestationalAge.Unit);
         }

         individualProperties.AddIs(PKSimConstants.UI.Weight, displayValueFor(originData.Weight), originData.Weight.Unit);

         if (originData.Population.IsHeightDependent)
         {
            if (originData.Height != null)
               individualProperties.AddIs(PKSimConstants.UI.Height, displayValueFor(originData.Height), originData.Height.Unit);

            if (originData.BMI != null)
               individualProperties.AddIs(PKSimConstants.UI.BMI, displayValueFor(originData.BMI), originData.BMI.Unit);
         }

         reportPart.AddPart(populationProperties);
         reportPart.AddPart(individualProperties);
         var diseaseState = originData.DiseaseState;
         if (diseaseState != null)
         {
            var diseaseStateProperties = new TablePart(PKSimConstants.UI.DiseaseState, PKSimConstants.UI.Value, PKSimConstants.UI.Unit) {Title = diseaseState.DisplayName};
            originData.DiseaseStateParameters.Each(x =>
            {
               var parameter = diseaseState.Parameter(x.Name);
               var displayName = _representationInfoRepository.DisplayNameFor(parameter);
               diseaseStateProperties.AddIs(displayName, displayValueFor(x), x.Unit);
            });
            reportPart.AddPart(diseaseStateProperties);
         }

         reportPart.AddPart(_reportGenerator.ReportFor(originData.SubPopulation));
         reportPart.AddPart(_reportGenerator.ReportFor(originData.AllCalculationMethods()));
      }

      private string displayValueFor(OriginDataParameter originDataParameter)
      {
         var dimension = _dimensionRepository.DimensionForUnit(originDataParameter.Unit);
         var displayUnit = dimension.UnitOrDefault(originDataParameter.Unit);
         return _formatter.Format(dimension.BaseUnitValueToUnitValue(displayUnit, originDataParameter.Value));
      }
   }
}
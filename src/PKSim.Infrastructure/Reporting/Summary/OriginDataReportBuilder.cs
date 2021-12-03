using PKSim.Assets;
using OSPSuite.Utility.Format;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class OriginDataReportBuilder : ReportBuilder<OriginData>
   {
      private readonly IReportGenerator _reportGenerator;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly NumericFormatter<double> _formatter;

      public OriginDataReportBuilder(IReportGenerator reportGenerator, IDimensionRepository dimensionRepository)
      {
         _reportGenerator = reportGenerator;
         _dimensionRepository = dimensionRepository;
         _formatter = new NumericFormatter<double>(NumericFormatterOptions.Instance);
      }

      protected override void FillUpReport(OriginData originData, ReportPart reportPart)
      {
         var populationProperties = new TablePart(keyName:PKSimConstants.UI.PopulationProperties, valueName:PKSimConstants.UI.Value) {Title = PKSimConstants.UI.PopulationProperties};
         populationProperties.AddIs(PKSimConstants.UI.Species, originData.Species.DisplayName);
         populationProperties.AddIs(PKSimConstants.UI.Population, originData.SpeciesPopulation.DisplayName);
         populationProperties.AddIs(PKSimConstants.UI.Gender, originData.Gender.DisplayName);

         var individualProperties = new TablePart(PKSimConstants.UI.IndividualParameters,PKSimConstants.UI.Value,PKSimConstants.UI.Unit) { Title = PKSimConstants.UI.IndividualParameters };
         individualProperties.Types[PKSimConstants.UI.Value] = typeof (double);
         if (originData.SpeciesPopulation.IsAgeDependent)
         {
            if (originData.Age!=null)
               individualProperties.AddIs(PKSimConstants.UI.Age, displayValueFor(originData.Age.Value, _dimensionRepository.AgeInYears, originData.Age.Unit), originData.Age.Unit);

            if (originData.SpeciesPopulation.IsPreterm && originData.GestationalAge!=null)
               individualProperties.AddIs(PKSimConstants.UI.GestationalAge, displayValueFor(originData.GestationalAge.Value, _dimensionRepository.AgeInWeeks, originData.GestationalAge.Unit), originData.GestationalAge.Unit);
         }

         individualProperties.AddIs(PKSimConstants.UI.Weight, displayValueFor(originData.Weight.Value, _dimensionRepository.Mass, originData.Weight.Unit), originData.Weight.Unit);

         if (originData.SpeciesPopulation.IsHeightDependent)
         {
            if (originData.Height!=null)
               individualProperties.AddIs(PKSimConstants.UI.Height, displayValueFor(originData.Height.Value, _dimensionRepository.Length, originData.Height.Unit), originData.Height.Unit);

            if (originData.BMI != null)
               individualProperties.AddIs(PKSimConstants.UI.BMI, displayValueFor(originData.BMI.Value, _dimensionRepository.BMI, originData.BMI.Unit), originData.BMI.Unit);
         }

         reportPart.AddPart(populationProperties);
         reportPart.AddPart(individualProperties);
         reportPart.AddPart(_reportGenerator.ReportFor(originData.SubPopulation));
         reportPart.AddPart(_reportGenerator.ReportFor(originData.AllCalculationMethods()));
      }

      private string displayValueFor(double kernValue, IDimension dimension, string displayUnitNane)
      {
         var displayUnit = dimension.UnitOrDefault(displayUnitNane);
         return _formatter.Format(dimension.BaseUnitValueToUnitValue(displayUnit, kernValue));
      }
   }
}
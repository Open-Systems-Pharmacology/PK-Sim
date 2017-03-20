using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class CalculationMethodsReportBuilder : ReportBuilder<IEnumerable<CalculationMethod>>
   {
      private readonly ICalculationMethodCategoryRepository _calculationMethodCategoryRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public CalculationMethodsReportBuilder(ICalculationMethodCategoryRepository calculationMethodCategoryRepository, IRepresentationInfoRepository representationInfoRepository)
      {
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
         _representationInfoRepository = representationInfoRepository;
      }

      protected override void FillUpReport(IEnumerable<CalculationMethod> calculationMethods, ReportPart reportPart)
      {

         var calcMethodPart= new TablePart(keyName:PKSimConstants.UI.Category,valueName:PKSimConstants.UI.CalculationMethods) {Title = PKSimConstants.UI.CalculationMethods};
         foreach (var calculationMethod in calculationMethods)
         {
            var currentCalculationMethod = calculationMethod;
            var category = _calculationMethodCategoryRepository.FindBy(currentCalculationMethod.Category);
            var allCalculationMethods = category.AllItems().ToList();
            if (allCalculationMethods.Count == 1)
               continue;

            //more than one cm in this category. Check that this is not only due to different models
            allCalculationMethods.Remove(calculationMethod);
            var allModelsUsedInCategory = allCalculationMethods.SelectMany(x => x.AllModels).Distinct();
            var allSpeciesUsedInCategory = allCalculationMethods.SelectMany(x => x.AllSpecies).Distinct();
            
            //at least another category available in the model and species
            if (allModelsUsedInCategory.Any(x => currentCalculationMethod.AllModels.Contains(x)) && 
               allSpeciesUsedInCategory.Any(x => currentCalculationMethod.AllSpecies.Contains(x)))

               calcMethodPart.AddIs(_representationInfoRepository.DisplayNameFor(category), _representationInfoRepository.DisplayNameFor(calculationMethod));
         }
         
         reportPart.AddPart(calcMethodPart);
      }
   }
}
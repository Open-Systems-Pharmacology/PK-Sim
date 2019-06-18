using System.Linq;
using MarkdownLog;
using OSPSuite.Core.Domain;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public class CalculationMethodCacheMarkdownBuilder : MarkdownBuilder<CalculationMethodCache>
   {
      private readonly ICalculationMethodCategoryRepository _calculationMethodCategoryRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public CalculationMethodCacheMarkdownBuilder(ICalculationMethodCategoryRepository calculationMethodCategoryRepository, IRepresentationInfoRepository representationInfoRepository)
      {
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
         _representationInfoRepository = representationInfoRepository;
      }

      public override void Report(CalculationMethodCache calculationMethodCache, MarkdownTracker tracker, int indentationLevel = 0)
      {
         var calculationMethods = calculationMethodCache
            .Select(x => new
            {
               CalculationMethod = x,
               Category = _calculationMethodCategoryRepository.FindBy(x.Category)
            })
            .Where(x => x.Category.AllItems().Count() > 1)
            .Select(x => new
               {
                  Name = _representationInfoRepository.DisplayNameFor(x.Category),
                  Value = _representationInfoRepository.DisplayNameFor(x.CalculationMethod),
               }
            );


         tracker.Add(calculationMethods.ToMarkdownTable());
      }
   }
}
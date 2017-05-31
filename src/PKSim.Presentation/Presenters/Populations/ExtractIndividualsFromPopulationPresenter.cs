using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Populations;
using PKSim.Presentation.Views.Populations;

namespace PKSim.Presentation.Presenters.Populations
{
   public interface IExtractIndividualsFromPopulationPresenter : IDisposablePresenter, IPresenter<IExtractIndividualsFromPopulationView>
   {
      void ExctractIndividuals(Population population, IEnumerable<int> individualIds);
      void UpdateGeneratedOutput(string namingPattern, string individualIdExpression);
   }

   public class ExtractIndividualsFromPopulationPresenter : AbstractDisposablePresenter<IExtractIndividualsFromPopulationView, IExtractIndividualsFromPopulationPresenter>, IExtractIndividualsFromPopulationPresenter
   {
      private readonly IIndividualExtractor _individualExtractor;
      private readonly ILazyLoadTask _lazyLoadTask;
      private ExtractIndividualsDTO _extractIndividualDTO;
      private string _populationName;

      public ExtractIndividualsFromPopulationPresenter(IExtractIndividualsFromPopulationView view, IIndividualExtractor individualExtractor, ILazyLoadTask lazyLoadTask) : base(view)
      {
         _individualExtractor = individualExtractor;
         _lazyLoadTask = lazyLoadTask;
      }

      public void ExctractIndividuals(Population population, IEnumerable<int> individualIds)
      {
         _lazyLoadTask.Load(population);
         _populationName = population.Name;
         _view.Caption = PKSimConstants.UI.ExtractIndividualFromPopulation(_populationName);
         _view.PopulationDescription = PKSimConstants.UI.ExtractIndividualPopulationDescription(_populationName, population.NumberOfItems);

         _extractIndividualDTO = new ExtractIndividualsDTO(population.NumberOfItems)
         {
            NamingPattern = IndividualExtractionOptions.DEFAULT_NAMING_PATTERN,
            IndividualIds = individualIds
         };

         _view.BindTo(_extractIndividualDTO);
         _view.Display();

         if (_view.Canceled)
            return;

         _individualExtractor.ExtractIndividualsFrom(population, extractionOptions);
      }

      public void UpdateGeneratedOutput(string namingPattern, string individualIdExpression)
      {
         var count = _extractIndividualDTO.CountFor(individualIdExpression);
         var individualIds = _extractIndividualDTO.ParseIndividualIds(individualIdExpression);
         var options = extractionOptionsFor(namingPattern, individualIds);
         var generatedIndividualNames = individualIds.Select(id => options.GenerateIndividualName(_populationName, id)).ToList();
         _view.UpdateGeneratedOutputDescription(count, generatedIndividualNames, _populationName);
      }

      private IndividualExtractionOptions extractionOptions => extractionOptionsFor(_extractIndividualDTO.NamingPattern, _extractIndividualDTO.IndividualIds);

      private IndividualExtractionOptions extractionOptionsFor(string namingPattern, IEnumerable<int> individualIds) => new IndividualExtractionOptions
      {
         NamingPattern = namingPattern,
         IndividualIds = individualIds
      };
   }
}
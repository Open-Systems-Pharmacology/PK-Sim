using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualEnzymeExpressionsPresenterNew<TSimulationSubject> : IIndividualProteinExpressionsPresenterNew
   {
   }

   public class IndividualEnzymeExpressionsPresenterNew<TSimulationSubject> : IndividualProteinExpressionsPresenterNew<IndividualEnzyme, TSimulationSubject>, IIndividualEnzymeExpressionsPresenterNew<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      public IndividualEnzymeExpressionsPresenterNew(IIndividualProteinExpressionsViewNew view,
         IEditParameterPresenterTask parameterTask,
         IMoleculeExpressionTask<TSimulationSubject> moleculeExpressionTask,
         IIndividualProteinToIndividualProteinDTOMapper individualProteinMapper,
         IRepresentationInfoRepository representationInfoRepository,
         IIndividualMoleculePropertiesPresenter<TSimulationSubject> moleculePropertiesPresenter,
         IExpressionLocalizationPresenter<TSimulationSubject> expressionLocalizationPresenter)
         : base(view, parameterTask, moleculeExpressionTask, individualProteinMapper, representationInfoRepository, moleculePropertiesPresenter, expressionLocalizationPresenter)
      {
      }
   }
}
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualEnzymeExpressionsPresenter<TSimulationSubject> : IIndividualProteinExpressionsPresenter
   {
   }

   public class IndividualEnzymeExpressionsPresenter<TSimulationSubject> : IndividualProteinExpressionsPresenter<IndividualEnzyme, TSimulationSubject>, IIndividualEnzymeExpressionsPresenter<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      public IndividualEnzymeExpressionsPresenter(IIndividualProteinExpressionsView view, IEditParameterPresenterTask parameterTask, IMoleculeExpressionTask<TSimulationSubject> moleculeExpressionTask, IIndividualProteinToProteinExpressionDTOMapper proteinExpressionDTOMapper,
         IRepresentationInfoRepository representationInfoRepository, IIndividualMoleculePropertiesPresenter<TSimulationSubject> moleculePropertiesPresenter)
         : base(view, parameterTask, moleculeExpressionTask, proteinExpressionDTOMapper, representationInfoRepository, moleculePropertiesPresenter)
      {
      }
   }
}
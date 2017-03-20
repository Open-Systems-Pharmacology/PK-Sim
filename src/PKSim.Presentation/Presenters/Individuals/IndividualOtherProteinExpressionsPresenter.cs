using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualOtherProteinExpressionsPresenter<TSimulationSubject> : IIndividualProteinExpressionsPresenter
   {
   }

   public class IndividualOtherProteinExpressionsPresenter<TSimulationSubject> : IndividualProteinExpressionsPresenter<IndividualOtherProtein, TSimulationSubject>, IIndividualOtherProteinExpressionsPresenter<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      public IndividualOtherProteinExpressionsPresenter(IIndividualProteinExpressionsView view, IEditParameterPresenterTask parameterTask, IMoleculeExpressionTask<TSimulationSubject> moleculeExpressionTask, IIndividualProteinToProteinExpressionDTOMapper proteinExpressionDTOMapper,
         IRepresentationInfoRepository representationInfoRepository, IIndividualMoleculePropertiesPresenter<TSimulationSubject> moleculePropertiesPresenter)
         : base(view, parameterTask, moleculeExpressionTask, proteinExpressionDTOMapper, representationInfoRepository, moleculePropertiesPresenter)
      {
      }
   }
}
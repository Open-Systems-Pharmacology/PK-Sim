using PKSim.Core.Model;
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
      public IndividualOtherProteinExpressionsPresenter(IIndividualProteinExpressionsView view,
         IEditParameterPresenterTask parameterTask,
         IIndividualProteinToIndividualProteinDTOMapper individualProteinMapper,
         IIndividualMoleculePropertiesPresenter<TSimulationSubject> moleculePropertiesPresenter,
         IExpressionLocalizationPresenter<TSimulationSubject> expressionLocalizationPresenter)
         : base(view, parameterTask, individualProteinMapper, moleculePropertiesPresenter, expressionLocalizationPresenter)
      {
      }
   }
}
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualOtherProteinExpressionsPresenter<TSimulationSubject> : IIndividualProteinExpressionsPresenter
   {
   }

   public class IndividualOtherProteinExpressionsPresenter<TSimulationSubject> : IndividualProteinExpressionsPresenter<IndividualOtherProtein, TSimulationSubject>, IIndividualOtherProteinExpressionsPresenter<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      public IndividualOtherProteinExpressionsPresenter(IIndividualProteinExpressionsView view,
         IIndividualProteinToIndividualProteinDTOMapper individualProteinMapper,
         IIndividualMoleculePropertiesPresenter<TSimulationSubject> moleculePropertiesPresenter,
         IExpressionLocalizationPresenter<TSimulationSubject> expressionLocalizationPresenter,
         IExpressionParametersPresenter expressionParametersPresenter)
         : base(view,  individualProteinMapper, moleculePropertiesPresenter, expressionLocalizationPresenter, expressionParametersPresenter)
      {
      }
   }
}
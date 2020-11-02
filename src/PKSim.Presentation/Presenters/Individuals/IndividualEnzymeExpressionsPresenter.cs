using PKSim.Core.Model;
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
      public IndividualEnzymeExpressionsPresenter(IIndividualProteinExpressionsView view,
         IEditParameterPresenterTask parameterTask,
         IIndividualProteinToIndividualProteinDTOMapper individualProteinMapper,
         IIndividualMoleculePropertiesPresenter<TSimulationSubject> moleculePropertiesPresenter,
         IExpressionLocalizationPresenter<TSimulationSubject> expressionLocalizationPresenter)
         : base(view, parameterTask,  individualProteinMapper, moleculePropertiesPresenter, expressionLocalizationPresenter)
      {
      }
   }
}
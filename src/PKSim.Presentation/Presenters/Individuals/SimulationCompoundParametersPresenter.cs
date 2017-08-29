using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface ISimulationCompoundParametersPresenter : ICustomParametersPresenter
   {
   }

   public class SimulationCompoundParametersPresenter : MultiParameterEditPresenter, ISimulationCompoundParametersPresenter
   {
      private readonly IWithIdRepository _withIdRepository;

      public SimulationCompoundParametersPresenter(IMultiParameterEditView view, IScaleParametersPresenter scaleParametersPresenter,
         IEditParameterPresenterTask editParameterPresenterTask, IParameterTask parameterTask, IParameterToParameterDTOMapper parameterDTOMapper,
         IParameterContextMenuFactory contextMenuFactory, IWithIdRepository withIdRepository)
         : base(view, scaleParametersPresenter, editParameterPresenterTask, parameterTask, parameterDTOMapper, contextMenuFactory)
      {
         _withIdRepository = withIdRepository;
      }

      public override void Edit(IEnumerable<IParameter> parameters)
      {
         var allParameters = parameters.ToList();
         base.Edit(allParameters);

         if (oneParameterPerCompoundIsBeingDisplayed(allParameters))
            return;

         //add  grouping by compound name using the standard sorting
         _view.GroupBy(PathElement.Molecule, groupIndex: 1, useCustomSort: false); 
      }

      private Simulation simulationFrom(IReadOnlyList<IParameter> allParameters)
      {
         var firstParameter = allParameters[0];
         return _withIdRepository.Get<Simulation>(firstParameter.Origin.SimulationId);
      }

      protected override void PerformDefaultGrouping(IReadOnlyList<IParameter> parameters)
      {
         //one parameter exactly per compound, simply hide parameter name
         if (oneParameterPerCompoundIsBeingDisplayed(parameters))
            _view.ParameterNameVisible = false;
         else
            base.PerformDefaultGrouping(parameters);
      }

      private bool oneParameterPerCompoundIsBeingDisplayed(IReadOnlyList<IParameter> parameters)
      {
         var simulation = simulationFrom(parameters);
         if (simulation == null)
            return true;

         return simulation.CompoundNames.Count == parameters.Count
                && AllParametersHaveTheSameDisplayName;

      }
   }
}
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationExpressionsPresenter : ICustomParametersPresenter
   {
   }

   public class SimulationExpressionsPresenter : AbstractCommandCollectorPresenter<ISimulationExpressionsView, ISimulationExpressionsPresenter>,
      ISimulationExpressionsPresenter
   {
      private readonly IExpressionParametersToSimulationExpressionsDTOMapper _simulationExpressionsDTOMapper;
      private SimulationExpressionsDTO _simulationExpressionsDTO;
      private readonly IMultiParameterEditPresenter _moleculeParametersPresenter;
      private readonly IExpressionParametersPresenter _expressionParametersPresenter;
      private List<IParameter> _allParameters;
      public string Description { get; set; }
      public bool ForcesDisplay => false;
      public bool AlwaysRefresh => false;

      public IEnumerable<IParameter> EditedParameters => _allParameters;

      public SimulationExpressionsPresenter(
         ISimulationExpressionsView view,
         IExpressionParametersToSimulationExpressionsDTOMapper simulationExpressionsDTOMapper,
         IMultiParameterEditPresenter moleculeParametersPresenter,
         IExpressionParametersPresenter expressionParametersPresenter)
         : base(view)
      {
         _simulationExpressionsDTOMapper = simulationExpressionsDTOMapper;

         _moleculeParametersPresenter = moleculeParametersPresenter;
         _expressionParametersPresenter = expressionParametersPresenter;
         _moleculeParametersPresenter.IsSimpleEditor = true;

         AddSubPresenters(moleculeParametersPresenter, _expressionParametersPresenter);
         view.AddMoleculeParametersView(_moleculeParametersPresenter.View);
         view.AddExpressionParametersView(_expressionParametersPresenter.View);
      }

      public void Edit(IEnumerable<IParameter> parameters)
      {
         _allParameters = parameters.ToList();
         _simulationExpressionsDTO = _simulationExpressionsDTOMapper.MapFrom(_allParameters);
         _moleculeParametersPresenter.Edit(_simulationExpressionsDTO.MoleculeParameters);
         _expressionParametersPresenter.Edit(_simulationExpressionsDTO.ExpressionParameters);
      }
   }
}
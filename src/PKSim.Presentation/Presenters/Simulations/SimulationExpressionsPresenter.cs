using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationExpressionsPresenter : IEditParameterPresenter, ICustomParametersPresenter,
      IListener<AddParameterToFavoritesEvent>,
      IListener<RemoveParameterFromFavoritesEvent>
   {
      void SetRelativeExpression(ExpressionContainerDTO protein, double valueInGuiUnit);
      void SetFavorite(ExpressionContainerDTO protein, bool isFavorite);
   }

   public class SimulationExpressionsPresenter : EditParameterPresenter<ISimulationExpressionsView, ISimulationExpressionsPresenter>, ISimulationExpressionsPresenter
   {
      private readonly IExpressionParametersToSimulationExpressionsDTOMapper _mapper;
      private readonly IMoleculeExpressionTask<Individual> _moleculeExpressionTask;
      private readonly IParameterTask _parameterTask;
      private SimulationExpressionsDTO _simulationExpressionsDTO;
      private PathCache<IParameter> _pathCache;
      public string Description { get; set; }
      public bool ForcesDisplay { get; } =  false;
      public bool AlwaysRefresh { get; } = false;

      public IEnumerable<IParameter> EditedParameters => _pathCache;

      public SimulationExpressionsPresenter(ISimulationExpressionsView view, IExpressionParametersToSimulationExpressionsDTOMapper mapper,
         IEditParameterPresenterTask editParameterPresenterTask, IMoleculeExpressionTask<Individual> moleculeExpressionTask, IEntityPathResolver entityPathResolver, IParameterTask parameterTask)
         : base(view, editParameterPresenterTask)
      {
         _mapper = mapper;
         _moleculeExpressionTask = moleculeExpressionTask;
         _parameterTask = parameterTask;
         _pathCache = new PathCache<IParameter>(entityPathResolver);
      }

      public void Edit(IEnumerable<IParameter> parameters)
      {
         var allParameters = parameters.ToList();
         _pathCache = _parameterTask.PathCacheFor(allParameters);
         _simulationExpressionsDTO = _mapper.MapFrom(allParameters);
         _view.BindTo(_simulationExpressionsDTO);
      }

      public void SetRelativeExpression(ExpressionContainerDTO protein, double valueInGuiUnit)
      {
         AddCommand(_moleculeExpressionTask.SetRelativeExpressionInSimulationFor(protein.RelativeExpressionParameter.Parameter, valueInGuiUnit));
      }

      public void SetFavorite(ExpressionContainerDTO protein, bool isFavorite)
      {
         SetFavorite(protein.RelativeExpressionParameter, isFavorite);
      }

      public virtual void Handle(AddParameterToFavoritesEvent eventToHandle)
      {
         parameterFrom(eventToHandle.ParameterPath).IsFavorite = true;
      }

      private IParameterDTO parameterFrom(string parameterPath)
      {
         if (_pathCache == null || _simulationExpressionsDTO == null)
            return new NullParameterDTO();

         var parameter = _pathCache[parameterPath];
         if (parameter == null)
            return new NullParameterDTO();

         return _simulationExpressionsDTO.AllParameters().First(x => Equals(x.Parameter, parameter));
      }

      public virtual void Handle(RemoveParameterFromFavoritesEvent eventToHandle)
      {
         parameterFrom(eventToHandle.ParameterPath).IsFavorite = false;
      }
   }
}
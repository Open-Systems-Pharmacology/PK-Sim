using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ICommitSimulationParametersPresenter : IDisposablePresenter
   {
      /// <summary>
      ///    Shows the commit dialog for the simulation and returns the commit infos if the user confirmed, null if canceled.
      /// </summary>
      IReadOnlyList<CompoundCommitInfo> ShowCommitDialog(Simulation simulation);
   }

   public class CommitSimulationParametersPresenter : AbstractDisposablePresenter<ICommitSimulationParametersView, ICommitSimulationParametersPresenter>, ICommitSimulationParametersPresenter
   {
      private readonly IContainerTask _containerTask;
      private readonly IPKSimProjectRetriever _projectRetriever;
      private CommitSimulationParametersDTO _dto;

      public CommitSimulationParametersPresenter(
         ICommitSimulationParametersView view,
         IContainerTask containerTask,
         IPKSimProjectRetriever projectRetriever) : base(view)
      {
         _containerTask = containerTask;
         _projectRetriever = projectRetriever;
      }

      public IReadOnlyList<CompoundCommitInfo> ShowCommitDialog(Simulation simulation)
      {
         _dto = buildDTO(simulation);

         if (!_dto.Compounds.Any())
            return null;

         _view.Caption = PKSimConstants.Command.CommitSimulationParametersDescription;
         _view.BindTo(_dto);
         _view.Display();

         if (_view.Canceled)
            return null;

         return buildCommitInfos();
      }

      private CommitSimulationParametersDTO buildDTO(Simulation simulation)
      {
         var dto = new CommitSimulationParametersDTO();
         var trackedPaths = simulation.ParameterChangeTracker.ChangedPaths;
         var parameterCache = _containerTask.CacheAllChildren<IParameter>(simulation.Model.Root);

         var pathsByCompound = trackedPaths
            .Select(p => p.PathAsString)
            .GroupBy(path => simulation.CompoundNameForParameterPath(path))
            .Where(g => g.Key != null);

         pathsByCompound.Each(group =>
         {
            var compoundName = group.Key;
            var simulationCompound = simulation.Compounds.First(c => c.Name == compoundName);
            var templateId = simulation.TemplateBuildingBlockIdUsedBy(simulationCompound);
            var templateCompound = _projectRetriever.Current.BuildingBlockById<Compound>(templateId);

            dto.Compounds.Add(new CompoundCommitDTO
            {
               CompoundName = compoundName,
               TemplateCompound = templateCompound,
               AvailableExistingSets = templateCompound.OverwriteParameterSets,
               NewSetName = compoundName,
               Parameters = group.Select(path =>
               {
                  var parameter = parameterCache[path];
                  return new ParameterCommitDTO
                  {
                     Path = path,
                     DisplayPath = path,
                     Value = parameter?.Value ?? double.NaN
                  };
               }).ToList()
            });
         });

         return dto;
      }

      private IReadOnlyList<CompoundCommitInfo> buildCommitInfos()
      {
         return _dto.Compounds
            .Where(c => c.Selected)
            .Select(c => new CompoundCommitInfo
            {
               Compound = c.TemplateCompound,
               ParameterPaths = c.Parameters.Where(p => p.Selected).Select(p => p.Path).ToList(),
               ExistingOverwriteParameterSet = c.CreateNew ? null : c.SelectedExistingSet,
               NewOverwriteParameterSetName = c.CreateNew ? c.NewSetName : null
            })
            .ToList();
      }
   }
}

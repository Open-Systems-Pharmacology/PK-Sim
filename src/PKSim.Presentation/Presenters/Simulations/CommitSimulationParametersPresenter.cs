using System.Linq;
using OSPSuite.Presentation.Presenters;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ICommitSimulationParametersPresenter : IDisposablePresenter
   {
      /// <summary>
      ///    Shows a modal dialog listing the tracked parameter changes of <paramref name="compound" /> and letting the user
      ///    select the ones to commit and choose whether to update the overwrite parameter set selected in the simulation or
      ///    to create a new one.
      ///    Returns a <see cref="CompoundCommitInfo" /> describing the commit, or <c>null</c> if the user cancels or no
      ///    parameters have uncommitted changes.
      /// </summary>
      CompoundCommitInfo ShowCommitDialog(Simulation simulation, Compound compound);
   }

   public class CommitSimulationParametersPresenter : AbstractDisposablePresenter<ICommitSimulationParametersView, ICommitSimulationParametersPresenter>, ICommitSimulationParametersPresenter
   {
      private readonly ISimulationToCompoundCommitDTOMapper _mapper;

      public CommitSimulationParametersPresenter(
         ICommitSimulationParametersView view,
         ISimulationToCompoundCommitDTOMapper mapper) : base(view)
      {
         _mapper = mapper;
      }

      public CompoundCommitInfo ShowCommitDialog(Simulation simulation, Compound compound)
      {
         var dto = _mapper.MapFrom(simulation, compound);

         if (dto == null)
            return null;

         _view.Caption = PKSimConstants.Command.CommitSimulationParametersDescription;
         _view.BindTo(dto);
         _view.Display();

         if (_view.Canceled)
            return null;

         return commitInfoFrom(dto, compound);
      }

      private CompoundCommitInfo commitInfoFrom(CompoundCommitDTO dto, Compound compound)
      {
         var selectedParameters = dto.VisibleParameters.Where(p => p.Selected).ToList();
         return new CompoundCommitInfo
         {
            TemplateCompoundId = compound.Id,
            ParameterPaths = selectedParameters.Where(p => !p.IsRemoval).Select(p => p.Path).ToList(),
            ParameterPathsToRemove = selectedParameters.Where(p => p.IsRemoval).Select(p => p.Path).ToList(),
            OverwriteParameterSetName = dto.CreateNew ? dto.NewSetName : dto.SetSelectedInSimulation.Name,
            ShouldCreateNew = dto.CreateNew
         };
      }
   }
}
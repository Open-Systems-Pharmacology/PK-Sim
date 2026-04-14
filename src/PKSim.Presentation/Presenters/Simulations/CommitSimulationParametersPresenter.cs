using System.Linq;
using OSPSuite.Presentation.Presenters;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ICommitSimulationParametersPresenter : IDisposablePresenter
   {
      /// <summary>
      ///    Shows a modal dialog allowing the user to select which tracked parameter changes
      ///    should be committed to the compound overwrite parameter set for <paramref name="compound" />.
      ///    Returns a <see cref="CompoundCommitInfo" /> with the user's selections,
      ///    or <c>null</c> if the user cancels or no parameters have uncommitted changes.
      /// </summary>
      CompoundCommitInfo ShowCommitDialog(Simulation simulation, Compound compound);
   }

   public class CommitSimulationParametersPresenter : AbstractDisposablePresenter<ICommitSimulationParametersView, ICommitSimulationParametersPresenter>, ICommitSimulationParametersPresenter
   {
      private readonly ISimulationToCompoundCommitDTOMapper _mapper;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public CommitSimulationParametersPresenter(
         ICommitSimulationParametersView view,
         ISimulationToCompoundCommitDTOMapper mapper,
         IBuildingBlockRepository buildingBlockRepository) : base(view)
      {
         _mapper = mapper;
         _buildingBlockRepository = buildingBlockRepository;
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

         return commitInfoFrom(dto, simulation, compound);
      }

      private CompoundCommitInfo commitInfoFrom(CompoundCommitDTO dto, Simulation simulation, Compound compound)
      {
         var templateId = simulation.TemplateBuildingBlockIdUsedBy(compound);
         var projectCompound = _buildingBlockRepository.ById<Compound>(templateId);

         return new CompoundCommitInfo
         {
            SimulationCompound = dto.TemplateCompound,
            TemplateCompound = projectCompound,
            ParameterPaths = dto.Parameters.Where(p => p.Selected).Select(p => p.Path).ToList(),
            ExistingOverwriteParameterSet = dto.CreateNew ? null : dto.SelectedExistingSet,
            NewOverwriteParameterSetName = dto.CreateNew ? dto.NewSetName : null
         };
      }
   }
}
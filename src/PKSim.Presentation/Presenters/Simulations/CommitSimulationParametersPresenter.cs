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
      ///    Shows a modal dialog allowing the user to select which tracked parameter changes
      ///    should be committed to the compound overwrite parameter set for <paramref name="compound"/>.
      ///    Returns a <see cref="CompoundCommitInfo"/> with the user's selections,
      ///    or <c>null</c> if the user cancels or no parameters have uncommitted changes.
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

         return commitInfoFrom(dto);
      }

      private CompoundCommitInfo commitInfoFrom(CompoundCommitDTO dto)
      {
         return new CompoundCommitInfo
         {
            Compound = dto.TemplateCompound,
            ParameterPaths = dto.Parameters.Where(p => p.Selected).Select(p => p.Path).ToList(),
            ExistingOverwriteParameterSet = dto.CreateNew ? null : dto.SelectedExistingSet,
            NewOverwriteParameterSetName = dto.CreateNew ? dto.NewSetName : null
         };
      }
   }
}

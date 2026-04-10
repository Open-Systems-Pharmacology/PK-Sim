using System.Collections.Generic;
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
      ///    Returns the list of <see cref="CompoundCommitInfo"/> selected by the user,
      ///    or <c>null</c> if the user cancels or no parameters have uncommitted changes.
      /// </summary>
      IReadOnlyList<CompoundCommitInfo> ShowCommitDialog(Simulation simulation, Compound compound);
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

      public IReadOnlyList<CompoundCommitInfo> ShowCommitDialog(Simulation simulation, Compound compound)
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

      private IReadOnlyList<CompoundCommitInfo> commitInfoFrom(CompoundCommitDTO dto)
      {
         return new[]
         {
            new CompoundCommitInfo
            {
               Compound = dto.TemplateCompound,
               ParameterPaths = dto.Parameters.Where(p => p.Selected).Select(p => p.Path).ToList(),
               ExistingOverwriteParameterSet = dto.CreateNew ? null : dto.SelectedExistingSet,
               NewOverwriteParameterSetName = dto.CreateNew ? dto.NewSetName : null
            }
         };
      }
   }
}

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
      ///    should be committed to compound overwrite parameter sets.
      ///    When <paramref name="compoundFilter"/> is specified, only parameters for that compound are shown.
      ///    Returns the list of <see cref="CompoundCommitInfo"/> selected by the user,
      ///    or <c>null</c> if the user cancels or no compounds have uncommitted changes.
      /// </summary>
      IReadOnlyList<CompoundCommitInfo> ShowCommitDialog(Simulation simulation, Compound compoundFilter = null);
   }

   public class CommitSimulationParametersPresenter : AbstractDisposablePresenter<ICommitSimulationParametersView, ICommitSimulationParametersPresenter>, ICommitSimulationParametersPresenter
   {
      private readonly ISimulationToCommitSimulationParametersDTOMapper _mapper;

      public CommitSimulationParametersPresenter(
         ICommitSimulationParametersView view,
         ISimulationToCommitSimulationParametersDTOMapper mapper) : base(view)
      {
         _mapper = mapper;
      }

      public IReadOnlyList<CompoundCommitInfo> ShowCommitDialog(Simulation simulation, Compound compoundFilter = null)
      {
         var dto = _mapper.MapFrom(simulation, compoundFilter);

         if (!dto.Compounds.Any())
            return null;

         _view.Caption = PKSimConstants.Command.CommitSimulationParametersDescription;
         _view.BindTo(dto);
         _view.Display();

         if (_view.Canceled)
            return null;

         return commitInfosFrom(dto);
      }

      private IReadOnlyList<CompoundCommitInfo> commitInfosFrom(CommitSimulationParametersDTO dto)
      {
         return dto.Compounds
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

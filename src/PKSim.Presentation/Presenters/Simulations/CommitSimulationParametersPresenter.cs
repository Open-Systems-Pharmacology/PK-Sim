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
      IReadOnlyList<CompoundCommitInfo> ShowCommitDialog(Simulation simulation);
   }

   public class CommitSimulationParametersPresenter : AbstractDisposablePresenter<ICommitSimulationParametersView, ICommitSimulationParametersPresenter>, ICommitSimulationParametersPresenter
   {
      private readonly ISimulationToCommitSimulationParametersDTOMapper _mapper;
      private CommitSimulationParametersDTO _dto;

      public CommitSimulationParametersPresenter(
         ICommitSimulationParametersView view,
         ISimulationToCommitSimulationParametersDTOMapper mapper) : base(view)
      {
         _mapper = mapper;
      }

      public IReadOnlyList<CompoundCommitInfo> ShowCommitDialog(Simulation simulation)
      {
         _dto = _mapper.MapFrom(simulation);

         if (!_dto.Compounds.Any())
            return null;

         _view.Caption = PKSimConstants.Command.CommitSimulationParametersDescription;
         _view.BindTo(_dto);
         _view.Display();

         if (_view.Canceled)
            return null;

         return commitInfosFrom(_dto);
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

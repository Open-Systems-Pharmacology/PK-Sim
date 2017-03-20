using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IEnzymaticCompoundProcessPresenter : IProcessPresenter<EnzymaticProcess>
   {
      /// <summary>
      ///    Method used to update the metabolite for the process
      /// </summary>
      /// <param name="newMetabolite">The new metabolite being attached to the process</param>
      void MetaboliteChanged(string newMetabolite);
   }

   public class EnzymaticCompoundProcessPresenter : BaseCompoundProcessPresenter<EnzymaticProcess, IEnzymaticCompoundProcessView, IEnzymaticCompoundProcessPresenter, EnzymaticProcessDTO>,
      IEnzymaticCompoundProcessPresenter
   {
      private EnzymaticProcess _process;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public EnzymaticCompoundProcessPresenter(
         IEnzymaticCompoundProcessView view,
         IParametersByGroupPresenter parametersPresenter,
         IEntityTask entityTask, ISpeciesRepository speciesRepository,
         IRepresentationInfoRepository representationInfoRepository,
         IEnzymaticProcessToEnzymaticProcessDTOMapper mapper,
         IBuildingBlockRepository buildingBlockRepository,
         ICompoundProcessTask processTask)
         : base(view, parametersPresenter, entityTask, speciesRepository, representationInfoRepository, mapper, processTask)
      {
         _buildingBlockRepository = buildingBlockRepository;
      }

      public override void Edit(EnzymaticProcess compoundProcess)
      {
         base.Edit(compoundProcess);
         _process = compoundProcess;
         _view.UpdateAvailableCompounds(allPossibleMetabolites());
      }

      private IEnumerable<string> allPossibleMetabolites()
      {
         return _buildingBlockRepository.All<Compound>()
            .Where(x => !Equals(_process.ParentCompound, x))
            .OrderBy(x => x.Name)
            .Select(x => x.Name);
      }

      public void MetaboliteChanged(string newMetabolite)
      {
         AddCommand(_processTask.SetMetaboliteForEnzymaticProcess(_process, newMetabolite));
      }
   }
}
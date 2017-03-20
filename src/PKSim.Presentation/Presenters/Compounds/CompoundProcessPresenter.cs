using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Core.Services;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICompoundProcessPresenter : IProcessPresenter<CompoundProcess>
   {
   }

   public class CompoundProcessPresenter : BaseCompoundProcessPresenter<CompoundProcess, ICompoundProcessView, ICompoundProcessPresenter, CompoundProcessDTO>, ICompoundProcessPresenter
   {
      public CompoundProcessPresenter(ICompoundProcessView view, IParametersByGroupPresenter parametersPresenter,
         IEntityTask entityTask, ISpeciesRepository speciesRepository,
         IRepresentationInfoRepository representationInfoRepository,
         ICompoundProcessToCompoundProcessDTOMapper compoundProcessDTOMapper,
         ICompoundProcessTask compoundProcessTask)
         : base(view, parametersPresenter, entityTask, speciesRepository, representationInfoRepository, compoundProcessDTOMapper, compoundProcessTask)
      {
      }


   }
}
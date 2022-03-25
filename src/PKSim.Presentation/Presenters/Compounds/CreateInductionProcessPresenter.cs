using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICreateInductionProcessPresenter : ICreatePartialProcessPresenter
   {
   }

   public class CreateInductionProcessPresenter : CreatePartialProcessPresenter<InductionProcess, ICreatePartialProcessView, ICreatePartialProcessPresenter>, ICreateInductionProcessPresenter
   {
      public CreateInductionProcessPresenter(ICreatePartialProcessView view, ICompoundProcessTask compoundProcessTask, IPartialProcessToPartialProcessDTOMapper partialProcessMapper, IParametersByGroupPresenter parameterEditPresenter, ICompoundProcessToCompoundProcessDTOMapper processMapper, IUsedMoleculeRepository usedMoleculeRepository, ISpeciesRepository speciesRepository)
         : base(view, compoundProcessTask, partialProcessMapper, parameterEditPresenter, processMapper, usedMoleculeRepository, speciesRepository)
      {
         view.ApplicationIcon = ApplicationIcons.Induction;
         view.MoleculeCaption = PKSimConstants.UI.AffectedEnzymeOrTransporter;
         view.Caption = PKSimConstants.UI.Induction;
      }
   }
}
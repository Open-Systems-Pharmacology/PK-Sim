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
   public interface ICreateSpecificBindingPartialProcessPresenter : ICreatePartialProcessPresenter
   {
   }

   public class CreateSpecificBindingPartialProcessPresenter : CreatePartialProcessPresenter<SpecificBindingPartialProcess, ICreatePartialProcessView, ICreatePartialProcessPresenter>, ICreateSpecificBindingPartialProcessPresenter
   {
      public CreateSpecificBindingPartialProcessPresenter(ICreatePartialProcessView view, ICompoundProcessTask compoundProcessTask, ICompoundProcessToCompoundProcessDTOMapper processMapper, IPartialProcessToPartialProcessDTOMapper partialProcessMapper, IParametersByGroupPresenter parameterEditPresenter, IUsedMoleculeRepository usedMoleculeRepository, ISpeciesRepository speciesRepository)
         : base(view, compoundProcessTask,  partialProcessMapper, parameterEditPresenter,processMapper, usedMoleculeRepository, speciesRepository)
      {
         view.ApplicationIcon = ApplicationIcons.SpecificBinding;
         view.MoleculeCaption = PKSimConstants.UI.ProteinBindingPartner;
         view.Caption = PKSimConstants.UI.CreateProteinBindingPartner;
      }
   }
}
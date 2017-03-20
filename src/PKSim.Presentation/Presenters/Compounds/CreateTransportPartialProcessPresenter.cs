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
   public interface ICreateTransportPartialProcessPresenter : ICreatePartialProcessPresenter
   {
   }

   public class CreateTransportPartialProcessPresenter : CreatePartialProcessPresenter<TransportPartialProcess, ICreatePartialProcessView, ICreatePartialProcessPresenter>, ICreateTransportPartialProcessPresenter
   {
      public CreateTransportPartialProcessPresenter(ICreatePartialProcessView view, ICompoundProcessTask compoundProcessTask, ICompoundProcessToCompoundProcessDTOMapper processMapper, IPartialProcessToPartialProcessDTOMapper partialProcessMapper, IParametersByGroupPresenter parameterEditPresenter, IUsedMoleculeRepository usedMoleculeRepository, ISpeciesRepository speciesRepository)
         : base(view, compoundProcessTask,  partialProcessMapper, parameterEditPresenter,processMapper, usedMoleculeRepository, speciesRepository)
      {
         view.SetIcon(ApplicationIcons.Transporter);
         view.MoleculeCaption = PKSimConstants.UI.TransportProteins;
         view.Caption = PKSimConstants.UI.CreateTransportProtein;
      }
   }
}
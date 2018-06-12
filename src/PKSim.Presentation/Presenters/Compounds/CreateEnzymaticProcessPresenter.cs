using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Assets;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICreateEnzymaticProcessPresenter : ICreatePartialProcessPresenter
   {
      void MetaboliteChanged(string newMetabolite);
   }

   public class CreateEnzymaticProcessPresenter : CreatePartialProcessPresenter<EnzymaticProcess, ICreateEnzymaticProcessView, ICreateEnzymaticProcessPresenter>, ICreateEnzymaticProcessPresenter
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private EnzymaticProcessDTO _enzymaticProcessDTO;

      public CreateEnzymaticProcessPresenter(
         ICreateEnzymaticProcessView view, 
         ICompoundProcessTask compoundProcessTask, 
         ICompoundProcessToCompoundProcessDTOMapper processMapper, 
         IPartialProcessToPartialProcessDTOMapper partialProcessMapper, 
         IParametersByGroupPresenter parameterEditPresenter, 
         IUsedMoleculeRepository usedMoleculeRepository, 
         ISpeciesRepository speciesRepository,
         IBuildingBlockRepository buildingBlockRepository)
         : base(view, compoundProcessTask, partialProcessMapper, parameterEditPresenter, processMapper, usedMoleculeRepository, speciesRepository)
      {
         view.SetIcon(ApplicationIcons.Metabolite);
         view.Caption = PKSimConstants.UI.CreateMetabolizingEnzyme;
         view.MoleculeCaption = PKSimConstants.UI.MetabolizingEnzyme;
         _buildingBlockRepository = buildingBlockRepository;
      }

      private IEnumerable<Compound> allPossibleMetabolites()
      {
         return _buildingBlockRepository.All<Compound>()
            .OrderBy(x => x.Name);
      }
      
      protected override void Rebind(EnzymaticProcess enzymaticProcess, Compound compound)
      {
         var previousMetaboliteName = _enzymaticProcessDTO.Metabolite;
         enzymaticProcess.MetaboliteName = previousMetaboliteName;
         _enzymaticProcessDTO = dtoFrom(enzymaticProcess);
         base.Rebind(enzymaticProcess, compound);
      }

      protected override void BindToView()
      {
         base.BindToView();
         _view.BindTo(_enzymaticProcessDTO);
      }

      protected override void EditProcess(EnzymaticProcess enzymaticProcess, Compound compound)
      {
         _view.UpdateAvailableCompounds(allPossibleMetabolites().Where(x => !Equals(x, compound)).Select(x => x.Name));
         _enzymaticProcessDTO = dtoFrom(enzymaticProcess);
         base.EditProcess(enzymaticProcess, compound);
      }

      public void MetaboliteChanged(string newMetabolite)
      {
         AddCommand(_compoundProcessTask.SetMetaboliteForEnzymaticProcess(_createdProcess, newMetabolite));
      }

      private EnzymaticProcessDTO dtoFrom(EnzymaticProcess enzymaticProcess) => new EnzymaticProcessDTO(enzymaticProcess) {Metabolite = enzymaticProcess.MetaboliteName};
   }
}
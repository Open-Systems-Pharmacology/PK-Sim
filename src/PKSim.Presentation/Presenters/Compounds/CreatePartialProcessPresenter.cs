using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICreatePartialProcessPresenter : ICreateProcessPresenter
   {
      string DefaultMoleculeName { get; set; }
   }

   public abstract class CreatePartialProcessPresenter<TPartialProcess, TView, TPresenter> :
      CreateProcessPresenter<TPartialProcess, TView, TPresenter>,
      ICreatePartialProcessPresenter, ILatchable
      where TPartialProcess : PartialProcess
      where TView : ICreatePartialProcessView<TPresenter> where TPresenter : IDisposablePresenter
   {
      private readonly IPartialProcessToPartialProcessDTOMapper _partialProcessMapper;
      private PartialProcessDTO _partialProcessDTO;
      public string DefaultMoleculeName { get; set; }

      protected CreatePartialProcessPresenter(TView view,
         ICompoundProcessTask compoundProcessTask,
         IPartialProcessToPartialProcessDTOMapper partialProcessMapper,
         IParametersByGroupPresenter parameterEditPresenter,
         ICompoundProcessToCompoundProcessDTOMapper processMapper,
         IUsedMoleculeRepository usedMoleculeRepository, ISpeciesRepository speciesRepository)
         : base(view, parameterEditPresenter, processMapper, compoundProcessTask, speciesRepository)
      {
         _partialProcessMapper = partialProcessMapper;
         _view.AllAvailableProteins = usedMoleculeRepository.All();
         DefaultMoleculeName = string.Empty;
      }

      protected override void Rebind(TPartialProcess partialProcess, Compound compound)
      {
         var oldProteinName = _partialProcessDTO.MoleculeName;
         var oldDataSource = _partialProcessDTO.DataSource;
         _partialProcessDTO = _partialProcessMapper.MapFrom(partialProcess, compound);
         _partialProcessDTO.MoleculeName = oldProteinName;
         _partialProcessDTO.DataSource = oldDataSource;
         BindToView();
      }

      protected override void EditProcess(TPartialProcess partialProcess, Compound compound)
      {
         if (string.IsNullOrEmpty(partialProcess.MoleculeName))
            partialProcess.MoleculeName = DefaultMoleculeName;

         _partialProcessDTO = _partialProcessMapper.MapFrom(partialProcess, compound);
         BindToView();
      }

      protected virtual void BindToView()
      {
         _view.BindTo(_partialProcessDTO);
      }

      protected override void UpdateProperties(TPartialProcess partialProcess)
      {
         _partialProcessMapper.UpdateProperties(partialProcess, _partialProcessDTO);
      }
   }
}
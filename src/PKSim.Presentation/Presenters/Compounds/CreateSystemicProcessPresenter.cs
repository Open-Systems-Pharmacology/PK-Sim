using OSPSuite.Assets;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICreateSystemicProcessPresenter : ICreateProcessPresenter
   {
   }

   public class CreateSystemicProcessPresenter : CreateProcessPresenter<SystemicProcess, ICreateSystemicProcessView, ICreateSystemicProcessPresenter>, ICreateSystemicProcessPresenter
   {
      private readonly ISystemicProcessToSystemicProcessDTOMapper _systemicProcessDTOMapper;
      private SystemicProcessDTO _systemicProcessDTO;

      public CreateSystemicProcessPresenter(ICreateSystemicProcessView view, IParametersByGroupPresenter parameterEditPresenter,
         ICompoundProcessToCompoundProcessDTOMapper processMapper, ICompoundProcessTask compoundProcessTask,
         ISpeciesRepository speciesRepository, ISystemicProcessToSystemicProcessDTOMapper systemicProcessDTOMapper) : base(view, parameterEditPresenter, processMapper, compoundProcessTask, speciesRepository)
      {
         _systemicProcessDTOMapper = systemicProcessDTOMapper;
      }

      protected override void EditProcess(SystemicProcess systemicProcess, Compound compound)
      {
         _view.ApplicationIcon = ApplicationIcons.IconByName(systemicProcess.SystemicProcessType.IconName);
         _view.Caption = PKSimConstants.UI.CreateSystemicProcess(systemicProcess.SystemicProcessType.DisplayName);
         _systemicProcessDTO = _systemicProcessDTOMapper.MapFrom(systemicProcess, compound);
         _view.BindTo(_systemicProcessDTO);
      }

      protected override void Rebind(SystemicProcess systemicProcess, Compound compound)
      {
         var oldDataSource = _systemicProcessDTO.DataSource;
         _systemicProcessDTO = _systemicProcessDTOMapper.MapFrom(systemicProcess, compound);
         _systemicProcessDTO.DataSource = oldDataSource;
         _view.BindTo(_systemicProcessDTO);
      }

      protected override void UpdateProperties(SystemicProcess systemicProcess)
      {
         _systemicProcessDTOMapper.UpdateProperties(systemicProcess, _systemicProcessDTO);
      }
   }
}
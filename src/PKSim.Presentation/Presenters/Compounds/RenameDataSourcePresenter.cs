using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IRenameDataSourcePresenter : IObjectBasePresenter<CompoundProcess>
   {
      string DataSource { get; }
   }

   public class RenameDataSourcePresenter : ObjectBasePresenter<CompoundProcess>, IRenameDataSourcePresenter
   {
      private readonly IPartialProcessToPartialProcessDTOMapper _partialProcessDTOMapper;
      private readonly ISystemicProcessToSystemicProcessDTOMapper _systemicProcessDTOMapper;
      private CompoundProcessDTO _compoundProcessDTO;

      public RenameDataSourcePresenter(IRenameDataSourceView view, IPartialProcessToPartialProcessDTOMapper partialProcessDTOMapper, ISystemicProcessToSystemicProcessDTOMapper systemicProcessDTOMapper) : base(view, false)
      {
         _partialProcessDTOMapper = partialProcessDTOMapper;
         _systemicProcessDTOMapper = systemicProcessDTOMapper;
      }

      protected override void InitializeResourcesFor(CompoundProcess compoundProcess)
      {
         _view.Caption = PKSimConstants.UI.Rename;
         _view.NameDescription = PKSimConstants.UI.RenameDataSourceCaption();
      }

      protected override ObjectBaseDTO CreateDTOFor(CompoundProcess process)
      {
         if (process.IsAnImplementationOf<PartialProcess>())
            _compoundProcessDTO = _partialProcessDTOMapper.MapFrom(process.DowncastTo<PartialProcess>(), process.ParentCompound);
         else
            _compoundProcessDTO = _systemicProcessDTOMapper.MapFrom(process.DowncastTo<SystemicProcess>(), process.ParentCompound);

         return _compoundProcessDTO;
      }

      public string DataSource
      {
         get { return _compoundProcessDTO.DataSource; }
      }
   }
}
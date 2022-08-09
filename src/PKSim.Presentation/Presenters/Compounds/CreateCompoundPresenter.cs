using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICreateCompoundPresenter : ICreateBuildingBlockPresenter<Compound>, IWizardPresenter
   {
      Compound Compound { get; }
   }

   public class CreateCompoundPresenter : PKSimWizardPresenter<ICreateCompoundView, ICreateCompoundPresenter, ICompoundItemPresenter>, ICreateCompoundPresenter
   {
      private ObjectBaseDTO _compoundPropertiesDTO;
      private readonly IObjectBaseDTOFactory _objectBaseDTOFactory;
      private readonly IBuildingBlockPropertiesMapper _propertiesMapper;
      private readonly ICompoundFactory _compoundFactory;
      public Compound Compound { get; private set; }

      public CreateCompoundPresenter(ICreateCompoundView view, ISubPresenterItemManager<ICompoundItemPresenter> subPresenterItemManager, IDialogCreator dialogCreator,
         ICompoundFactory compoundFactory, IObjectBaseDTOFactory objectBaseDTOFactory, IBuildingBlockPropertiesMapper propertiesMapper)
         : base(view, subPresenterItemManager, CompoundItems.All, dialogCreator)
      {
         _compoundFactory = compoundFactory;
         _objectBaseDTOFactory = objectBaseDTOFactory;
         _propertiesMapper = propertiesMapper;
         AllowQuickFinish = true;
      }

      protected override void UpdateControls(int indexThatWillHaveFocus)
      {
         UpdateViewStatus();

         //we are on the subject selection. Next only enable if the model /subject selection ok
         if (indexThatWillHaveFocus == CompoundItems.Parameters.Index)
            _view.NextEnabled = PresenterAt(CompoundItems.Parameters).CanClose;
         else
            _view.NextEnabled = true;

         _view.SetControlEnabled(CompoundItems.Processes, PresenterAt(CompoundItems.Parameters).CanClose);
         _view.SetControlEnabled(CompoundItems.AdvancedParameters, PresenterAt(CompoundItems.Parameters).CanClose);

         _view.OkEnabled = CanClose;
      }

      public IPKSimCommand Create()
      {
         _compoundPropertiesDTO = _objectBaseDTOFactory.CreateFor<Compound>();
         Compound = _compoundFactory.Create();
         _subPresenterItemManager.AllSubPresenters.Each(x => x.EditCompound(Compound));
         _view.BindToProperties(_compoundPropertiesDTO);
         _view.EnableControl(CompoundItems.Parameters);
         _view.EnableControl(CompoundItems.Processes);
         SetWizardButtonEnabled(CompoundItems.Parameters);
         _view.Display();

         if (_view.Canceled)
            return new PKSimEmptyCommand();

         updateCompoundProperties();

         return _macroCommand;
      }

      private void updateCompoundProperties()
      {
         _propertiesMapper.MapProperties(_compoundPropertiesDTO, Compound);
      }

      public Compound BuildingBlock
      {
         get { return Compound; }
      }
   }
}
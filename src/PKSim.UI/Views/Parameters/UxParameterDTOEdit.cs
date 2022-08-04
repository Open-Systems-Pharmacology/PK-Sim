using System;
using System.Linq;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.DTO;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.UI.Extensions;

namespace PKSim.UI.Views.Parameters
{
   public partial class UxParameterDTOEdit : BaseUserControl
   {
      private ScreenBinder<IParameterDTO> _screenBinder;
      private IElementBinder<IParameterDTO, double> _discreteValueElementBinder;
      private IElementBinder<IParameterDTO, double> _valueElementBinder;

      /// <summary>
      ///    Event is raised whenever a value is being changed in the user control
      /// </summary>
      public event Action Changing = delegate { };

      /// <summary>
      ///    Event is raised whenever a value has changed
      /// </summary>
      public event Action Changed = delegate { };

      public event Action<IParameterDTO, double> ValueChanged = delegate { };
      public event Action<IParameterDTO, Unit> UnitChanged = delegate { };

      public UxParameterDTOEdit()
      {
         InitializeComponent();
         InitializeBinding();
         InitializeResources();
      }

      public override void InitializeBinding()
      {
         _screenBinder = new ScreenBinder<IParameterDTO>();
         _valueElementBinder = _screenBinder.Bind(p => p.Value)
            .To(tbValue);

         _valueElementBinder.OnValueUpdating += (o, e) => ValueChanged(o, e.NewValue);

         _screenBinder.Bind(p => p.DisplayUnit)
            .To(cbUnit)
            .WithValues(p => p.AllUnits)
            .OnValueUpdating += (o, e) => UnitChanged(o, e.NewValue);

         _discreteValueElementBinder = _screenBinder.Bind(p => p.Value)
            .To(cbDiscreteValue)
            .WithValues(p => p.ListOfValues.Keys)
            .AndDisplays(p => p.ListOfValues);

         _discreteValueElementBinder.OnValueUpdating += (o, e) => ValueChanged(o, e.NewValue);

         _screenBinder.Changed += notifyChange;
         RegisterValidationFor(_screenBinder, () => Changing());

         tbValue.EnterMoveNextControl = true;
      }

      private void notifyChange()
      {
         Changed();
      }

      public void BindTo(IParameterDTO parameterDTO)
      {
         layoutItemDiscreteValue.Visibility = LayoutVisibilityConvertor.FromBoolean(parameterDTO.IsDiscrete);
         layoutControlItemValue.Visibility = LayoutVisibilityConvertor.FromBoolean(!parameterDTO.IsDiscrete);

         if (!parameterDTO.IsDiscrete)
         {
            _screenBinder.Remove(_discreteValueElementBinder);
            cbUnit.Enabled = parameterDTO.AllUnits.Count() > 1;
         }
         else
         {
            _screenBinder.Remove(_valueElementBinder);
            cbUnit.Enabled = false;
         }


         _screenBinder.BindToSource(parameterDTO);
         layoutControlItemUnit.AdjustControlWidth(layoutControl, OSPSuite.UI.UIConstants.Size.BUTTON_WIDTH);
      }

      public override bool HasError => _screenBinder.HasError;

      public void ValidateControl()
      {
         _screenBinder.Validate();
      }

      public string ToolTip
      {
         set => tbValue.ToolTip = value;
         get => tbValue.ToolTip;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutControl.InitializeDisabledColors();
         layoutItemDiscreteValue.Visibility = LayoutVisibilityConvertor.FromBoolean(false);
         Height = cbUnit.Height;
      }

      public void RegisterEditParameterEvents(IParameterValuePresenter editParameterPresenter)
      {
         ValueChanged += (o, e) => OnEvent(() => editParameterPresenter.SetParameterValue(o, e));
         UnitChanged += (o, e) => OnEvent(() => editParameterPresenter.SetParameterUnit(o, e));
      }
   }
}
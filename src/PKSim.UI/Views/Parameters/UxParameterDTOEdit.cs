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
using static OSPSuite.UI.UIConstants.Size;

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
         layoutControlItemUnit.AdjustControlWidth(BUTTON_WIDTH, layoutControl);

         //We remove the two elements that may be bound at run time and add them based on the parameter type
         _screenBinder.Remove(_discreteValueElementBinder);
         _screenBinder.Remove(_valueElementBinder);

         var unitCount = parameterDTO.AllUnits.Count();
         if (!parameterDTO.IsDiscrete)
         {
            _screenBinder.AddElement(_valueElementBinder);
            cbUnit.Enabled = unitCount > 1;
            layoutControlItemUnit.Visibility = LayoutVisibilityConvertor.FromBoolean(true);
         }
         else
         {
            _screenBinder.AddElement(_discreteValueElementBinder);
            cbUnit.Enabled = false;
            //we hide the unit for a parameter without dimension
            var unitVisible = !parameterDTO.Dimension.IsEquivalentTo(Constants.Dimension.NO_DIMENSION);
            layoutControlItemUnit.Visibility = LayoutVisibilityConvertor.FromBoolean(unitVisible);
            layoutItemDiscreteValue.Padding = new Padding(0, unitVisible ? 2 : 0, 0, 0);
         }

         _screenBinder.BindToSource(parameterDTO);
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
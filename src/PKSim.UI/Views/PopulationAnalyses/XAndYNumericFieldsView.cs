using System;
using System.Linq.Expressions;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using DevExpress.XtraEditors;
using PKSim.Assets;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class XAndYNumericFieldsView : BaseUserControl, IXAndYNumericFieldsView
   {
      private IXAndYNumericFieldsPresenter _presenter;
      private readonly ScreenBinder<XandYFieldsSelectionDTO> _screenBinder;

      public XAndYNumericFieldsView()
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<XandYFieldsSelectionDTO>();
      }

      public void AttachPresenter(IXAndYNumericFieldsPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         bind(dto => dto.X, cbXField);
         bind(dto => dto.Y, cbYField);

         _screenBinder.Changed += () => OnEvent(_presenter.FieldSelectionChanged);
         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      private void bind(Expression<Func<XandYFieldsSelectionDTO, IPopulationAnalysisField>> expression, ComboBoxEdit comboBoxEdit)
      {
         _screenBinder.Bind(expression)
            .To(comboBoxEdit)
            .WithValues(dto => _presenter.AllAvailableFields())
            .AndDisplays(field => _presenter.DisplayFor(field));
      }

      public void BindTo(XandYFieldsSelectionDTO fieldsSelectionDTO)
      {
         _screenBinder.BindToSource(fieldsSelectionDTO);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemXField.Text = PKSimConstants.UI.XField;
         layoutItemYField.Text = PKSimConstants.UI.YField;
         lblDescription.Text = PKSimConstants.UI.SelectedOutputs;
      }
   }
}
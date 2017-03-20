using System.Collections.Generic;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Presenters;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Individuals
{
   public partial class SimpleMoleculeView : BaseModalView, ISimpleMoleculeView
   {
      private IObjectBasePresenter _presenter;
      private ScreenBinder<ObjectBaseDTO> _screenBinder;
      public bool DescriptionVisible { get; set; }
      public bool NameVisible { get; set; }
      public bool NameEditable { get; set; }

      public SimpleMoleculeView(Shell shell)
         : base(shell)
      {
         InitializeComponent();
      }

      public override void InitializeBinding()
      {
         _screenBinder = new ScreenBinder<ObjectBaseDTO>();
         _screenBinder.Bind(x => x.Name).To(cbProteinName);
         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      public void AttachPresenter(IObjectBasePresenter presenter)
      {
         _presenter = presenter;
      }

      public string NameDescription
      {
         set { layoutItemName.Text = value.FormatForLabel(); }
         get { return layoutItemName.Text; }
      }

      public bool NameDescriptionVisible
      {
         get { return layoutItemName.TextVisible; }
         set { layoutItemName.TextVisible = value; }
      }

      public void BindTo(ObjectBaseDTO dto)
      {
         _screenBinder.BindToSource(dto);
      }

      public override bool HasError
      {
         get { return _screenBinder.HasError; }
      }

      public IEnumerable<string> AvailableProteins
      {
         set { cbProteinName.FillWith(value); }
      }

      protected override void SetActiveControl()
      {
         ActiveControl = cbProteinName;
      }
   }
}
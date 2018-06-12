using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Presentation.DTO;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Compounds
{
   public partial class RenameDataSourceView : ObjectBaseView, IRenameDataSourceView
   {
      private readonly ScreenBinder<CompoundProcessDTO> _compoundProcessBinder;

      public RenameDataSourceView(Shell shell): base(shell)
      {
         InitializeComponent();
         _compoundProcessBinder = new ScreenBinder<CompoundProcessDTO>();
      }

      public override void InitializeBinding()
      {
         _compoundProcessBinder.Bind(dto => dto.DataSource).To(tbName);
         _compoundProcessBinder.Bind(x => x.Description).To(tbDescription);
         RegisterValidationFor(_compoundProcessBinder);
      }

      public override void BindTo(ObjectBaseDTO objectBaseDTO)
      {
         _compoundProcessBinder.BindToSource(objectBaseDTO.DowncastTo<CompoundProcessDTO>());
         SetOkButtonEnable();
      }

      public override bool HasError => _compoundProcessBinder.HasError;
   }
}
using System.Collections.Generic;
using System.Windows.Forms;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.UI.Services;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Utils;
using OSPSuite.UI.Controls;
using PKSim.Assets;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Compounds
{
   public partial class CreateEnzymaticProcessView : CreatePartialProcessView, ICreateEnzymaticProcessView
   {
      private readonly UxMRUEdit _comboBox;
      private readonly ScreenBinder<EnzymaticProcessDTO> _screenBinder;
      private ICreateEnzymaticProcessPresenter _createEnzymaticProcessPresenter;

      public CreateEnzymaticProcessView(IImageListRetriever imageListRetriever, UserLookAndFeel lookAndFeel, Shell shell)
         : base(imageListRetriever, lookAndFeel, shell)
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<EnzymaticProcessDTO>();
         _comboBox = new UxMRUEdit();
      }

      public void AttachPresenter(ICreateEnzymaticProcessPresenter presenter)
      {
         _createEnzymaticProcessPresenter = presenter;
         base.AttachPresenter(presenter);
      }

      public void UpdateAvailableCompounds(IEnumerable<string> availableCompoundNames)
      {
         _comboBox.FillWith(availableCompoundNames);
      }

      public void BindTo(EnzymaticProcessDTO enzymaticProcessDTO)
      {
         _screenBinder.BindToSource(enzymaticProcessDTO);
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _screenBinder.Bind(x => x.Metabolite)
            .To(_comboBox)
            .OnValueUpdating += (o, e) => OnEvent(() => _createEnzymaticProcessPresenter.MetaboliteChanged(e.NewValue));
      }
      
      public override void InitializeResources()
      {
         base.InitializeResources();
         insertControlAtTop(_comboBox, PKSimConstants.UI.Metabolite);
      }

      private void insertControlAtTop(Control controlToAdd, string caption)
      {
         var item = layoutControl.Root.AddItem();
         item.Control = controlToAdd;
         item.InitializeAsHeader(_lookAndFeel, caption);
         item.Move(layoutItemSpecies, InsertType.Top);
      }
   }
}

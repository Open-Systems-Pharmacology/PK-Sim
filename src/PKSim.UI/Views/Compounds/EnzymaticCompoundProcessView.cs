using System.Collections.Generic;
using System.Windows.Forms;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using PKSim.UI.Views.Core;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Compounds
{
   public partial class EnzymaticCompoundProcessView : BaseCompoundProcessView<EnzymaticProcess, EnzymaticProcessDTO>, IEnzymaticCompoundProcessView
   {
      private readonly UxMRUEdit _comboBox;

      public EnzymaticCompoundProcessView(IImageListRetriever imageListRetriever, UserLookAndFeel lookAndFeel)
         : base(imageListRetriever, lookAndFeel)
      {
         InitializeComponent();
         _comboBox = new UxMRUEdit();
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _screenBinder.Bind(x => x.Metabolite)
            .To(_comboBox)
            .OnValueUpdating += (o, e) => OnEvent(() => enzymaticCompoundProcessPresenter.MetaboliteChanged(e.NewValue));
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         insertControlAtTop(_comboBox, PKSimConstants.UI.Metabolite);
      }

      public void AttachPresenter(IEnzymaticCompoundProcessPresenter presenter)
      {
         base.AttachPresenter(presenter);
      }

      private void insertControlAtTop(Control controlToAdd, string caption)
      {
         var item = layoutControl.Root.AddItem();
         item.Control = controlToAdd;
         item.InitializeAsHeader(_lookAndFeel, caption);
         item.Move(layoutItemSpecies, InsertType.Top);
      }

      public void UpdateAvailableCompounds(IEnumerable<string> availableCompoundNames)
      {
         _comboBox.FillWith(availableCompoundNames);
      }

      private IEnzymaticCompoundProcessPresenter enzymaticCompoundProcessPresenter => _presenter.DowncastTo<IEnzymaticCompoundProcessPresenter>();
   }
}
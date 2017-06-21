using System.Collections.Generic;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.UI.Services;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Utils;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Compounds
{
   public partial class CreatePartialProcessView : CreateProcessView, ICreatePartialProcessView
   {
      private ICreatePartialProcessPresenter _presenter;
      private ScreenBinder<PartialProcessDTO> _propertiesBinder;

      public CreatePartialProcessView(IImageListRetriever imageListRetriever, UserLookAndFeel lookAndFeel, Shell shell)
         : base(imageListRetriever, lookAndFeel, shell)
      {
         InitializeComponent();
      }

      public void AttachPresenter(ICreatePartialProcessPresenter presenter)
      {
         _presenter = presenter;
         _createProcessPresenter = _presenter;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();

         _propertiesBinder = new ScreenBinder<PartialProcessDTO>();

         _propertiesBinder.Bind(dto => dto.DataSource)
            .To(tbDataSource);

         _propertiesBinder.Bind(dto => dto.MoleculeName)
            .To(cbProteinName);

         _propertiesBinder.Bind(dto => dto.Species)
            .To(cbSpecies)
            .WithImages(species => _imageListRetriever.ImageIndex(species.Icon))
            .WithValues(dto => _presenter.AllSpecies())
            .AndDisplays(species => species.DisplayName)
            .OnValueUpdating += (o, e) => OnEvent(() => _presenter.SpeciesChanged(e.NewValue));

         RegisterValidationFor(_propertiesBinder);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemSystemicProcessType.Visibility = LayoutVisibilityConvertor.FromBoolean(false);
      }

      public string MoleculeCaption {
         set { ProteinCaption = value; }
      }

      public IEnumerable<string> AllAvailableProteins
      {
         set { cbProteinName.FillWith(value); }
      }

      protected override void SetActiveControl()
      {
         ActiveControl = cbProteinName;
      }

      public void BindTo(PartialProcessDTO partialProcessDTO)
      {
         _propertiesBinder.BindToSource(partialProcessDTO);
      }

      public override bool HasError
      {
         get { return _propertiesBinder.HasError || base.HasError; }
      }

   }
}
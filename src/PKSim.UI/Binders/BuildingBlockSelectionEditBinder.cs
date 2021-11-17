using System;
using System.Windows.Forms;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.UI.Views;

namespace PKSim.UI.Binders
{
   public class BuildingBlockSelectionEditBinder<TObjectType, TBuildingBlock> : ElementBinder<TObjectType, TBuildingBlock> where TBuildingBlock : IPKSimBuildingBlock
   {
      private readonly UxBuildingBlockSelection _buildingBlockSelection;

      public BuildingBlockSelectionEditBinder(IPropertyBinderNotifier<TObjectType, TBuildingBlock> propertyBinder, UxBuildingBlockSelection buildingBlockSelection)
         : base(propertyBinder)
      {
         _buildingBlockSelection = buildingBlockSelection;
         _buildingBlockSelection.BuildingBlockSelectionChanged += buildingBlockSelectionChanged;
         _buildingBlockSelection.InitializeFor<TBuildingBlock>();
      }

      private void buildingBlockSelectionChanged()
      {
         ValueInControlChanging();
         ValueInControlChanged();
      }

      public override TBuildingBlock GetValueFromControl()
      {
         return _buildingBlockSelection.BuildingBlock.DowncastTo<TBuildingBlock>();
      }

      public override void SetValueToControl(TBuildingBlock value)
      {
         _buildingBlockSelection.BindTo(value);
      }

      public override Control Control => _buildingBlockSelection;

      public override bool HasError => _buildingBlockSelection.HasError;

      public override void Bind(TObjectType source)
      {
         base.Bind(source);
         //necessary to trigger a refresh as soon as the object is bound
         buildingBlockSelectionChanged();
      }

      protected override bool HasChanged()
      {
         //always perform update to source
         return true;
      }

      public BuildingBlockSelectionEditBinder<TObjectType, TBuildingBlock> WithFilter(Func<TBuildingBlock, bool> filter)
      {
         _buildingBlockSelection.ExtraFilter = x => filter(x.DowncastTo<TBuildingBlock>());
         return this;
      }
   }
}
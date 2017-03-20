using OSPSuite.DataBinding;
using PKSim.Core.Model;
using PKSim.UI.Binders;
using PKSim.UI.Views;
using PKSim.UI.Views.Parameters;
using OSPSuite.Presentation.DTO;

namespace PKSim.UI.Extensions
{
   public static class ScreenBinderExtensions
   {
      public static ParameterDTOEditBinder<TObjectType> To<TObjectType>(this IScreenToElementBinder<TObjectType, IParameterDTO> screenToElementBinder, UxParameterDTOEdit parameterDTOEdit)
      {
         var element = new ParameterDTOEditBinder<TObjectType>(screenToElementBinder.PropertyBinder, parameterDTOEdit);
         screenToElementBinder.ScreenBinder.AddElement(element);
         return element;
      }

      public static BuildingBlockSelectionEditBinder<TObjectType, TBuildingBlock> To<TObjectType, TBuildingBlock>(this IScreenToElementBinder<TObjectType, TBuildingBlock> screenToElementBinder,
         UxBuildingBlockSelection buildingBlockSelection)
         where TBuildingBlock : IPKSimBuildingBlock
      {
         var element = new BuildingBlockSelectionEditBinder<TObjectType, TBuildingBlock>(screenToElementBinder.PropertyBinder, buildingBlockSelection);
         screenToElementBinder.ScreenBinder.AddElement(element);
         return element;
      }
   }
}
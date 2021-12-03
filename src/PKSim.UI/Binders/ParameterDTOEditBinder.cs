using System.Windows.Forms;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.Core;
using PKSim.UI.Views.Parameters;
using OSPSuite.Presentation.DTO;

namespace PKSim.UI.Binders
{
   public class ParameterDTOEditBinder<TObjectType> : ElementBinder<TObjectType, IParameterDTO>
   {
      private readonly UxParameterDTOEdit _parameterDTOEdit;

      public ParameterDTOEditBinder(IPropertyBinderNotifier<TObjectType, IParameterDTO> propertyBinder, UxParameterDTOEdit parameterDTOEdit) : base(propertyBinder)
      {
         _parameterDTOEdit = parameterDTOEdit;
         _parameterDTOEdit.Changing += ValueInControlChanging;
         _parameterDTOEdit.Changed += NotifyChange;
      }

      public override IParameterDTO GetValueFromControl()
      {
         return GetValueFromSource();
      }

      public override void SetValueToControl(IParameterDTO value)
      {
         _parameterDTOEdit.BindTo(value);
      }

      public override Control Control => _parameterDTOEdit;

      public override bool HasError => _parameterDTOEdit.HasError;

      public override void Validate()
      {
         _parameterDTOEdit.ValidateControl();
      }
   }
}
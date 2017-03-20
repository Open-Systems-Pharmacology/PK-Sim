using BTS.Utility.Reflection;
using BTS.Utility.Validation;
using DevExpress.XtraEditors.DXErrorProvider;

namespace PKSim.Presentation.DTO.Core
{
   public abstract class DxValidatableDTO : ValidatableDTO, IDXDataErrorInfo
   {
      public virtual void GetPropertyError(string propertyName, ErrorInfo info)
      {
         this.UpdatePropertyError(propertyName, info);
      }

      public virtual void GetError(ErrorInfo info)
      {
         this.UpdateError(info);
      }     
   }

   public abstract class DxValidatableDTO<T> : DxValidatableDTO where T : IValidatable, INotifier
   {
      protected DxValidatableDTO(T underlyingObject)
      {
         this.AddRulesFrom(underlyingObject);
         underlyingObject.PropertyChanged += (o, e) => OnPropertyChanged(e.PropertyName);
      }
   }
}
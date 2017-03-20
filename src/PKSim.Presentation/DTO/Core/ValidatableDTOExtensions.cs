using OSPSuite.Utility.Validation;
using DevExpress.XtraEditors.DXErrorProvider;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Core
{
   public static class ValidatableDTOExtensions
   {
      public static T AddRulesFrom<T>(this T validatableDTO, IValidatable validatable) where T : IValidatableDTO
      {
         validatableDTO.Rules.Add(new ValidatableBusinessRule(validatable));
         return validatableDTO;
      }

      public static void UpdatePropertyError<T>(this T dto, string propertyName, ErrorInfo info) where T : IDXDataErrorInfo, IValidatable
      {
         var brokenRules = dto.Validate(propertyName);
         if (brokenRules.IsEmpty) return;
         info.ErrorText = brokenRules.Message;
         info.ErrorType = ErrorType.Critical;
      }

      public static void UpdateError<T>(this T dto, ErrorInfo info) where T : IDXDataErrorInfo, IValidatable
      {
         var brokenRules = dto.Validate();
         if (brokenRules.IsEmpty) return;
         info.ErrorText = brokenRules.Message;
         info.ErrorType = ErrorType.Critical;
      }
   }
}
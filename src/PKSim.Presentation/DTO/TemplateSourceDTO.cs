using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO
{
   public class TemplateSourceDTO : ValidatableDTO
   {
      private string _name;
      private string _url;

      public TemplateSourceDTO()
      {
         Rules.AddRange(AllRules.All());
      }

      public TemplateSourceDTO(TemplateSource templateSource) : this()
      {
         _name = templateSource.Name;
         _url = templateSource.Url;
      }

      public virtual string Name
      {
         get => _name;
         set => SetProperty(ref _name, value);
      }

      public virtual string Url
      {
         get => _url;
         set => SetProperty(ref _url, value);
      }

      public TemplateSource ToTemplateSource() => new TemplateSource(Name?.Trim(), Url?.Trim());

      private static class AllRules
      {
         private static IBusinessRule nonEmpty(Expression<Func<TemplateSourceDTO, string>> expression) => GenericRules.NonEmptyRule(expression);

         private static IBusinessRule urlIsValid { get; } = CreateRule.For<TemplateSourceDTO>()
            .Property(x => x.Url)
            .WithRule((x, url) => TemplateSource.IsValidUrl(url))
            .WithError(PKSimConstants.Rules.TemplateSourceUrlIsInvalid);

         public static IEnumerable<IBusinessRule> All()
         {
            yield return nonEmpty(x => x.Name);
            yield return urlIsValid;
         }
      }
   }
}

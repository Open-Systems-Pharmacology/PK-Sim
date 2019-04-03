using MarkdownLog;
using OSPSuite.Core.Domain;
using PKSim.Infrastructure.Reporting.Markdown.Elements;

namespace PKSim.Infrastructure.Reporting.Markdown.Extensions
{
   public static class MarkdownElementExtensions
   {
      public static MarkdownElement ToMarkdownTitle(this string text) => ToMarkdownLevelElement(text, 1);

      public static MarkdownElement ToMarkdownSubTitle(this string text) => ToMarkdownLevelElement(text, 2);

      public static MarkdownElement ToMarkdownLevelElement(this string text, int indentationLevel) => new TitleBaseElement(text, indentationLevel);

      public static T MapFrom<T>(this T parameterElement, IParameter parameter) where T : IParameterElement
      {
         parameterElement.Name = parameter.Name;
         parameterElement.ValueOrigin = parameter.ValueOrigin.ToString();
         parameterElement.Value = $"{parameter.ValueInDisplayUnit} {parameter.DisplayUnit.Name}";
         return parameterElement;
      }

      public static T To<T>(this IParameter parameter) where T : IParameterElement, new() => new T().MapFrom(parameter);
   }
}
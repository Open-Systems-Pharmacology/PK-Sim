using System;
using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core.Mappers;
using PKSim.Infrastructure.Reporting.Markdown.Elements;

namespace PKSim.Infrastructure.Reporting.Markdown.Mappers
{
   public interface IParameterToParameterElementMapper
   {
      T MapFrom<T>(IParameter parameter, Action<T> mapConfig = null) where T : IParameterElement, new();
      ParameterElement MapFrom(IParameter parameter);
   }

   public class ParameterToParameterElementMapper : IParameterToParameterElementMapper
   {
      private readonly IParameterListOfValuesRetriever _listOfValuesRetriever;

      public ParameterToParameterElementMapper(IParameterListOfValuesRetriever listOfValuesRetriever)
      {
         _listOfValuesRetriever = listOfValuesRetriever;
      }

      public T MapFrom<T>(IParameter parameter, Action<T> mapConfig = null) where T : IParameterElement, new()
      {
         var parameterElement = new T
         {
            Name = parameter.Name,
            ValueOrigin = parameter.ValueOrigin,
            Value = displayValueFor(parameter)
         };


         mapConfig?.Invoke(parameterElement);
         return parameterElement;
      }

      public ParameterElement MapFrom(IParameter parameter) => MapFrom<ParameterElement>(parameter);

      private string displayValueFor(IParameter parameter)
      {
         var listOfValues = _listOfValuesRetriever.ListOfValuesFor(parameter);
         if (!listOfValues.Any())
            return $"{parameter.ValueInDisplayUnit} {parameter.DisplayUnit.Name}";

         return listOfValues[parameter.Value];
      }
   }
}
using System.Xml.Linq;
using PKSim.Core.Model;
using OSPSuite.Core.Converter.v5_2;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Infrastructure.ProjectConverter.v5_2
{
   public interface IFormulaAndDimensionConverter
   {
      void ConvertDimensionIn(XElement element);
      void ConvertDimensionIn(IPKSimBuildingBlock buildingBlock);
      void ConvertDimensionIn(DataRepository dataRepository);
      void ConvertDimensionIn(IContainer container);
   }

   public class FormulaAndDimensionConverter : IFormulaAndDimensionConverter
   {
      private readonly IDimensionConverter _dimensionConverter;
      private readonly IDimensionMapper _dimensionMapper;

      public FormulaAndDimensionConverter(IDimensionConverter dimensionConverter, IDimensionMapper dimensionMapper)
      {
         _dimensionConverter = dimensionConverter;
         _dimensionMapper = dimensionMapper;
      }

      public void ConvertDimensionIn(XElement element)
      {
         _dimensionConverter.ConvertDimensionIn(element);
      }

      public void ConvertDimensionIn(DataRepository dataRepository)
      {
         _dimensionConverter.ConvertDimensionIn(dataRepository);
      }

      public void ConvertDimensionIn(IContainer container)
      {
         convert(container);
      }

      public void ConvertDimensionIn(IPKSimBuildingBlock buildingBlock)
      {
         convert(buildingBlock.Root);

         var simulation = buildingBlock as Simulation;

         if (simulation != null)
         {
            convert(simulation.Model.Root);
         }
      }

      private void convert(IContainer container)
      {
         var allParameters = container.GetAllChildren<IParameter>();
         foreach (var parameter in allParameters)
         {
            if (parameter.IsNamed("P (plasma<->interstitial)"))
               parameter.Name = parameter.Name;

            var conversionFactor = _dimensionMapper.ConversionFactor(parameter);
            if (conversionFactor == 1) continue;
            parameter.DefaultValue = convert(parameter.DefaultValue, conversionFactor);
            parameter.Info.MinValue = convert(parameter.MinValue, conversionFactor);
            parameter.Info.MaxValue = convert(parameter.MaxValue, conversionFactor);
         }

         //This needs to be done AFTER so that the correct conversion factor is available
         _dimensionConverter.ConvertDimensionIn(container);
      }

      private double? convert(double? value, double conversionFactor)
      {
         if (value == null)
            return null;
         return value * conversionFactor;
      }
   }
}
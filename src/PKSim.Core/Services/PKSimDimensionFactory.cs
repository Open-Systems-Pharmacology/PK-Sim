using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Services
{
   public interface IPKSimDimensionFactory : IDimensionFactory
   {
      IDimensionRepository DimensionRepository { get; set; }
   }

   public class PKSimDimensionFactory : DimensionFactory, IPKSimDimensionFactory
   {
      public IDimensionRepository DimensionRepository { get; set; }

      protected override IDimensionConverterFor CreateConverterFor<T>(IDimension dimension, IDimension dimensionToMerge, T hasDimension)
      {
         var column = hasDimension as DataColumn;
         if (column != null)
            return createColumnConverterFor(column);

         var parameter = hasDimension as IParameter;
         if (parameter != null)
            return createParameterConverterFor(parameter);

         var fieldContext = hasDimension as NumericFieldContext;
         if (fieldContext != null)
            return createFieldConverterFor(fieldContext);

         var quantityField = hasDimension as IQuantityField;
         if (quantityField != null)
            return createFieldConverterFor(quantityField);

         return null;
      }

      private IDimensionConverterFor createFieldConverterFor(NumericFieldContext fieldContext)
      {
         var quantityField = fieldContext.NumericValueField as IQuantityField;
         if (quantityField == null)
            return null;

         return createFieldConverterFor(quantityField, fieldContext.PopulationDataCollector);
      }

      private IDimensionConverterFor createFieldConverterFor(IQuantityField quantityField, IPopulationDataCollector populationDataCollector = null)
      {
         var dimension = quantityField.Dimension;
         if (dimension == DimensionRepository.MolarConcentration)
            return new MolarToMassConcentrationDimensionForFieldConverter(quantityField, populationDataCollector, DimensionRepository);

         if (dimension == DimensionRepository.Amount)
            return new MolarToMassAmoutDimensionForFieldConverter(quantityField, populationDataCollector, DimensionRepository);

         if (dimension == DimensionRepository.AucMolar)
            return new AucMolarToAucMassDimensionForFieldConverter(quantityField, populationDataCollector, DimensionRepository);

         if (dimension == DimensionRepository.MassConcentration)
            return new MassToMolarConcentrationDimensionForFieldConverter(quantityField, populationDataCollector, DimensionRepository);

         return null;
      }

      public override string MergedDimensionNameFor(IDimension sourceDimension)
      {
         if (isConcentration(sourceDimension))
            return PKSimConstants.UI.Concentration;

         return base.MergedDimensionNameFor(sourceDimension);
      }

      private bool isConcentration(IDimension dimension)
      {
         return
            dimension == DimensionRepository.MassConcentration ||
            dimension == DimensionRepository.MolarConcentration;
      }

      private IDimensionConverterFor createParameterConverterFor(IParameter parameter)
      {
         if (parameter.Dimension == DimensionRepository.AucMolar)
            return new AucMolarToAucMassDimensionForParameterConverter(parameter, DimensionRepository);

         if (parameter.Dimension == DimensionRepository.MolarConcentration)
            return new MolarToMassConcentrationDimensionForParameterConverter(parameter, DimensionRepository);

         return null;
      }

      private IDimensionConverterFor createColumnConverterFor(DataColumn column)
      {
         if (column.Dimension == DimensionRepository.MolarConcentration)
            return new MolarToMassConcentrationDimensionConverter(column, DimensionRepository);

         if (column.Dimension == DimensionRepository.MassConcentration)
            return new MassToMolarConcentrationDimensionConverter(column, DimensionRepository);

         if (column.Dimension == DimensionRepository.Amount)
            return new MolarToMassAmoutDimensionConverter(column, DimensionRepository);

         return null;
      }
   }
}
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.PopulationAnalyses
{
   public class PopulationAnalysisFieldDTO : ValidatableDTO<IPopulationAnalysisField>
   {
      private static readonly NullNumericField _nullDataField = new NullNumericField();
      private static readonly NullOutputField _nullOutputField = new NullOutputField();
      public IPopulationAnalysisField Field { get; }
      public IDimension Dimension { get; set; }

      public PopulationAnalysisFieldDTO(IPopulationAnalysisField populationAnalysisField)
         : base(populationAnalysisField)
      {
         Field = populationAnalysisField;
      }

      public string Name
      {
         get { return Field.Name; }
         set { Field.Name = value; }
      }

      public Unit DisplayUnit
      {
         get { return numericField.DisplayUnit; }
         set { numericField.DisplayUnit = value; }
      }

      public Scalings Scaling
      {
         get { return numericField.Scaling; }
         set { numericField.Scaling = value; }
      }

      public Color Color
      {
         get { return outputField.Color; }
         set { outputField.Color = value; }
      }

      private INumericValueField numericField
      {
         get { return IsNumericField ? Field.DowncastTo<INumericValueField>() : _nullDataField; }
      }

      private PopulationAnalysisOutputField outputField
      {
         get { return IsOutputField ? Field.DowncastTo<PopulationAnalysisOutputField>() : _nullOutputField; }
      }

      public bool IsNumericField
      {
         get { return Field.IsAnImplementationOf<INumericValueField>(); }
      }

      public bool IsOutputField
      {
         get { return Field.IsAnImplementationOf<PopulationAnalysisOutputField>(); }
      }

      public IReadOnlyList<Unit> Units
      {
         get { return Dimension.Units.ToList(); }
      }
   }
}
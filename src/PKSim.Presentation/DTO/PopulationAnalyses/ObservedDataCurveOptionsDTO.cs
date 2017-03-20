using System.Drawing;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.PopulationAnalyses
{
   public class ObservedDataCurveOptionsDTO : DxValidatableDTO
   {
      private readonly ObservedDataCurveOptions _observedDataCurveOptions;
      private readonly CurveOptions _curveOptions;

      public ObservedDataCurveOptionsDTO(ObservedDataCurveOptions observedDataCurveOptions)
      {
         _observedDataCurveOptions = observedDataCurveOptions;
         _curveOptions = _observedDataCurveOptions.CurveOptions;
         Rules.Add(GenericRules.NonEmptyRule<ObservedDataCurveOptionsDTO>(x => x.Caption));
      }

      public string Caption
      {
         get { return _observedDataCurveOptions.Caption; }
         set { _observedDataCurveOptions.Caption = value; }
      }
      
      public LineStyles LineStyle
      {
         get { return _curveOptions.LineStyle; }
         set { _curveOptions.LineStyle = value; }
      }

      public Symbols Symbol
      {
         get { return _curveOptions.Symbol; }
         set { _curveOptions.Symbol = value; }
      }

      public Color Color
      {
         get { return _curveOptions.Color; }
         set { _curveOptions.Color = value; }
      }

      public bool Visible
      {
         get { return _curveOptions.Visible; }
         set { _curveOptions.Visible = value; }
      }
   }
}
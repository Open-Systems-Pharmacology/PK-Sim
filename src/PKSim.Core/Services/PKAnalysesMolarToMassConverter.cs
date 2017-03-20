using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Services
{
   public abstract class PKAnalysesMolarToMassConverter : DimensionConverterBase
   {
      private readonly IParameter _parameter;

      protected PKAnalysesMolarToMassConverter(IParameter parameter, IDimension molarDimension, IDimension massDimension)
         : base(molarDimension, massDimension)
      {
         _parameter = parameter;
      }

      public override bool CanResolveParameters()
      {
         var pkAnalysis = _parameter.ParentContainer as PKAnalysis;
         return pkAnalysis != null && pkAnalysis.MolWeight != null;
      }

      protected override double MolWeight
      {
         get
         {
            var pkAnalysis = _parameter.ParentContainer.DowncastTo<PKAnalysis>();
            return pkAnalysis.MolWeight.GetValueOrDefault(double.NaN);
         }
      }

      public override double ConvertToTargetBaseUnit(double molarConcentration)
      {
         return ConvertToMass(molarConcentration);
      }

      public override double ConvertToSourceBaseUnit(double massConcentration)
      {
         return ConvertToMolar(massConcentration);
      }
   }
}
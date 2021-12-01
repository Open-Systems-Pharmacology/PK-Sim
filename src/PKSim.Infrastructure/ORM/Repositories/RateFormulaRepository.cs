using System.Globalization;
using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class RateFormulaRepository : MetaDataRepository<RateFormula>, IRateFormulaRepository
   {
      private readonly ICache<RateKey, string> _allFormula;
      private readonly ICache<RateKey, string> _formulaDimensions;
      private readonly string _nanString;

      public RateFormulaRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<RateFormula> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_CALCULATION_METHOD_RATE_FORMULA)
      {
         _allFormula = new Cache<RateKey, string>();
         _formulaDimensions = new Cache<RateKey, string>();
         _nanString = double.NaN.ToString(NumberFormatInfo.InvariantInfo);
      }

      public string FormulaFor(RateKey rateKey)
      {
         Start();
         return _allFormula[rateKey];
      }

      public string DimensionNameFor(RateKey rateKey)
      {
         Start();
         return _formulaDimensions[rateKey];
      }

      protected override void PerformPostStartProcessing()
      {
         foreach (var rateFormula in AllElements())
         {
            var formula = string.IsNullOrEmpty(rateFormula.Formula) ? _nanString : rateFormula.Formula;
            _allFormula.Add(new RateKey(rateFormula.CalculationMethod, rateFormula.Rate), formula);
            _formulaDimensions.Add(new RateKey(rateFormula.CalculationMethod, rateFormula.Rate), rateFormula.Dimension);
         }
      }
   }
}
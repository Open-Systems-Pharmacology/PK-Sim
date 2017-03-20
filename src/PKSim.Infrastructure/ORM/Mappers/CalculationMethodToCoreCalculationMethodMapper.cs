using System.Linq;
using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Core.Domain.Formulas;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface ICalculationMethodToCoreCalculationMethodMapper : IMapper<string, ICoreCalculationMethod>
   {
   }

   public class CalculationMethodToCoreCalculationMethodMapper : ICalculationMethodToCoreCalculationMethodMapper
   {
      private readonly IFlatCalculationMethodParameterRateRepository _flatCalculationMethodParameterRateRepository;
      private readonly IFlatCalculationMethodParameterDescriptorConditionRepository _flatCalculationMethodParameterDescriptorConditionRepository;
      private readonly ICalculationMethodRepository _calculationMethodRepository;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IParameterFactory _parameterFactory;

      public CalculationMethodToCoreCalculationMethodMapper(IFlatCalculationMethodParameterRateRepository flatCalculationMethodParameterRateRepository,
         IFlatCalculationMethodParameterDescriptorConditionRepository flatCalculationMethodParameterDescriptorConditionRepository,
         ICalculationMethodRepository calculationMethodRepository,
         IFormulaFactory formulaFactory,
         IParameterFactory parameterFactory)
      {
         _flatCalculationMethodParameterRateRepository = flatCalculationMethodParameterRateRepository;
         _flatCalculationMethodParameterDescriptorConditionRepository = flatCalculationMethodParameterDescriptorConditionRepository;
         _calculationMethodRepository = calculationMethodRepository;
         _formulaFactory = formulaFactory;
         _parameterFactory = parameterFactory;
      }

      public ICoreCalculationMethod MapFrom(string calculationMethod)
      {
         var modelCalcMethod = new CoreCalculationMethod {Name = calculationMethod};

         modelCalcMethod.Category = _calculationMethodRepository.FindBy(calculationMethod).Category;

         var allRates = from cmr in _flatCalculationMethodParameterRateRepository.All()
            where cmr.CalculationMethod.Equals(calculationMethod)
            select cmr;

         foreach (var parameterRate in allRates)
         {
            var descriptor = descriptorCriteriaFrom(parameterRate.ParameterId);

            if (parameterRate.IsOutputParameter)
            {
               var paramDescriptor = new ParameterDescriptor(parameterRate.ParameterName, descriptor);
               modelCalcMethod.AddOutputFormula(_formulaFactory.RateFor(parameterRate.CalculationMethod, parameterRate.Rate, new FormulaCache()), paramDescriptor);
            }
            else
            {
               var parameterRateMetaData = new ParameterRateMetaData
               {
                  BuildingBlockType = PKSimBuildingBlockType.Simulation,
                  BuildMode = ParameterBuildMode.Local,
                  CalculationMethod = parameterRate.CalculationMethod,
                  CanBeVaried = parameterRate.CanBeVaried,
                  CanBeVariedInPopulation = parameterRate.CanBeVariedInPopulation,
                  Dimension = parameterRate.Dimension,
                  GroupName = parameterRate.GroupName,
                  ReadOnly = parameterRate.ReadOnly,
                  MinValue = parameterRate.MinValue,
                  MaxValue = parameterRate.MaxValue,
                  MinIsAllowed = parameterRate.MinIsAllowed,
                  MaxIsAllowed = parameterRate.MaxIsAllowed,
                  ParameterName = parameterRate.ParameterName,
                  Rate = parameterRate.Rate,
                  Visible = parameterRate.Visible,
                  Sequence = parameterRate.Sequence
               };

               var helpParameter = _parameterFactory.CreateFor(parameterRateMetaData, new FormulaCache());

               modelCalcMethod.AddHelpParameter(helpParameter, descriptor);
            }
         }

         return modelCalcMethod;
      }

      private DescriptorCriteria descriptorCriteriaFrom(int parameterId)
      {
         var descriptorCriteria = new DescriptorCriteria();

         var conditions = from paramDescr in _flatCalculationMethodParameterDescriptorConditionRepository.All()
            where paramDescr.ParameterId == parameterId
            select paramDescr;

         foreach (var condition in conditions)
         {
            if (condition.ShouldHave)
               descriptorCriteria.Add(new MatchTagCondition(condition.Tag));
            else
               descriptorCriteria.Add(new NotMatchTagCondition(condition.Tag));
         }

         return descriptorCriteria;
      }
   }
}
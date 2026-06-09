using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility;

namespace PKSim.Core.Mappers
{
   public interface IParameterToFomulaTypeMapper : IMapper<IParameter, FormulaType>
   {
   }

   public class ParameterToFomulaTypeMapper : IParameterToFomulaTypeMapper
   {
      private readonly IFormulaToFormulaTypeMapper _formulaTypeMapper;

      public ParameterToFomulaTypeMapper(IFormulaToFormulaTypeMapper formulaTypeMapper)
      {
         _formulaTypeMapper = formulaTypeMapper;
      }

      public FormulaType MapFrom(IParameter parameter)
      {
         return parameter == null ? FormulaType.Constant : _formulaTypeMapper.MapFrom(parameter.Formula);
      }
   }
}
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IOriginDataParameterToParameterDTOMapper : IMapper<OriginDataParameter, IParameterDTO>
   {
   }

   public class OriginDataParameterToParameterDTOMapper : IOriginDataParameterToParameterDTOMapper
   {
      private readonly IParameterToParameterDTOMapper _parameterDTOMapper;
      private readonly IParameterFactory _parameterFactory;
      private readonly IDimensionRepository _dimensionRepository;

      public OriginDataParameterToParameterDTOMapper(
         IParameterToParameterDTOMapper parameterDTOMapper,
         IParameterFactory parameterFactory,
         IDimensionRepository dimensionRepository
      )
      {
         _parameterDTOMapper = parameterDTOMapper;
         _parameterFactory = parameterFactory;
         _dimensionRepository = dimensionRepository;
      }

      public IParameterDTO MapFrom(OriginDataParameter originDataParameter)
      {
         return _parameterDTOMapper.MapAsReadWriteFrom(parameterFrom(originDataParameter));
      }

      private IParameter parameterFrom(OriginDataParameter originDataParameter)
      {
         if (originDataParameter == null)
            return null;

         var (value, unit, name) = originDataParameter;

         var dimension = _dimensionRepository.DimensionForUnit(unit) ?? Constants.Dimension.NO_DIMENSION;
         return _parameterFactory.CreateFor(name, value, dimension.Name, PKSimBuildingBlockType.Individual);
      }
   }
}
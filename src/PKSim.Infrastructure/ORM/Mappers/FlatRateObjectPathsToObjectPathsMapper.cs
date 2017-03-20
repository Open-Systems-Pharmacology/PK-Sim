using System.Linq;
using OSPSuite.Utility;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatRateObjectToFormulaUsablePathMapper : IMapper<FlatRateObjectPath, IFormulaUsablePath>
   {
   }

   public class FlatRateObjectToFormulaUsablePathMapper : IFlatRateObjectToFormulaUsablePathMapper
   {
      private readonly IFlatObjectPathToObjectPathMapper _objectPathMapper;
      private readonly IObjectPathFactory _objectPathFactory;
      private readonly IDimensionRepository _dimensionRepository;

      public FlatRateObjectToFormulaUsablePathMapper(IFlatObjectPathToObjectPathMapper objectPathMapper,
         IObjectPathFactory objectPathFactory,
         IDimensionRepository dimensionRepository)
      {
         _objectPathMapper = objectPathMapper;
         _objectPathFactory = objectPathFactory;
         _dimensionRepository = dimensionRepository;
      }

      public IFormulaUsablePath MapFrom(FlatRateObjectPath flatRateObjectPath)
      {
         var rateObjectContainerPath = _objectPathMapper.MapFrom(flatRateObjectPath);

         return _objectPathFactory.CreateFormulaUsablePathFrom(rateObjectContainerPath.ToArray())
            .WithAlias(flatRateObjectPath.Alias)
            .WithDimension(_dimensionRepository.DimensionByName(flatRateObjectPath.Dimension));
      }
   }
}
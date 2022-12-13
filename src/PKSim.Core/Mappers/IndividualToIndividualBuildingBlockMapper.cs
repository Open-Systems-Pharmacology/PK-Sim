using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility;
using PKSim.Core.Model;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Core.Mappers
{
   public class OriginDataToCoreOriginDataMapper : IOriginDataToCoreOriginDataMapper
   {
      public CoreOriginData MapFrom(OriginData input)
      {
         var coreOriginData = new CoreOriginData
         {
            Species = input.Species.DisplayName,
            Age = input.Age.Clone(),
            BMI = input.BMI.Clone(),
            Comment = input.Comment,
            Height = input.Height.Clone(),
            Weight = input.Weight.Clone()
         };
         coreOriginData.ValueOrigin.UpdateFrom(input.ValueOrigin);

         return coreOriginData;
      }
   }

   public interface IOriginDataToCoreOriginDataMapper : IMapper<OriginData, CoreOriginData>
   {
   }

   public class IndividualToIndividualBuildingBlockMapper : PathAndValueBuildingBlockMapper<Individual, IndividualBuildingBlock, IndividualParameter>, IIndividualToIndividualBuildingBlockMapper
   {
      private readonly IOriginDataToCoreOriginDataMapper _originDataToCoreOriginDataMapper;

      public IndividualToIndividualBuildingBlockMapper(IObjectBaseFactory objectBaseFactory, IEntityPathResolver entityPathResolver, IFormulaFactory formulaFactory, IApplicationConfiguration applicationConfiguration, 
         IOriginDataToCoreOriginDataMapper originDataToCoreOriginDataMapper) : base(objectBaseFactory, entityPathResolver, formulaFactory, applicationConfiguration)
      {
         _originDataToCoreOriginDataMapper = originDataToCoreOriginDataMapper;
      }

      public override IndividualBuildingBlock MapFrom(Individual input)
      {
         var buildingBlock = base.MapFrom(input);

         buildingBlock.OriginData = _originDataToCoreOriginDataMapper.MapFrom(input.OriginData);

         return buildingBlock;
      }
   }

   public interface IIndividualToIndividualBuildingBlockMapper : IMapper<Individual, IndividualBuildingBlock>
   {
   }
}
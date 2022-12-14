using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;

namespace PKSim.Core.Mappers
{
   public interface IIndividualToIndividualBuildingBlockMapper : IPathAndValueBuildingBlockMapper<Individual, IndividualBuildingBlock>
   {
   }

   public class IndividualToIndividualBuildingBlockMapper : PathAndValueBuildingBlockMapper<Individual, IndividualBuildingBlock, IndividualParameter>, IIndividualToIndividualBuildingBlockMapper
   {
      private readonly IOriginDataToCoreOriginDataMapper _originDataToCoreOriginDataMapper;
      private readonly ILazyLoadTask _lazyLoadTask;

      public IndividualToIndividualBuildingBlockMapper(IObjectBaseFactory objectBaseFactory, IEntityPathResolver entityPathResolver, IApplicationConfiguration applicationConfiguration, 
         IOriginDataToCoreOriginDataMapper originDataToCoreOriginDataMapper, ILazyLoadTask lazyLoadTask) : base(objectBaseFactory, entityPathResolver, applicationConfiguration)
      {
         _originDataToCoreOriginDataMapper = originDataToCoreOriginDataMapper;
         _lazyLoadTask = lazyLoadTask;
      }

      protected override IReadOnlyList<IParameter> AllParametersFor(Individual sourcePKSimBuildingBlock)
      {
         return sourcePKSimBuildingBlock.GetAllChildren<IParameter>().Where(x => x.GroupName != CoreConstants.Groups.RELATIVE_EXPRESSION).ToList();
      }

      public override IndividualBuildingBlock MapFrom(Individual input)
      {
         _lazyLoadTask.Load(input);
         var buildingBlock = base.MapFrom(input);

         buildingBlock.OriginData = _originDataToCoreOriginDataMapper.MapFrom(input.OriginData);
         return buildingBlock;
      }
   }
}
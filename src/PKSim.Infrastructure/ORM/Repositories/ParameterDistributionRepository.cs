using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatParameterDistributionRepository : IMetaDataRepository<ParameterDistributionMetaData>
   {
   }

   public class FlatParameterDistributionRepository : MetaDataRepository<ParameterDistributionMetaData>, IFlatParameterDistributionRepository
   {
      public FlatParameterDistributionRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<ParameterDistributionMetaData> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_PARAMETER_DISTRIBUTIONS)
      {
      }
   }

   public class ParameterDistributionRepository : ParameterMetaDataRepository<ParameterDistributionMetaData>, IParameterDistributionRepository
   {
      public ParameterDistributionRepository(
         IFlatParameterDistributionRepository flatParameterDistributionRepo,
         IFlatContainerRepository flatContainerRepository,
         IValueOriginRepository valueOriginRepository) :
         base(flatParameterDistributionRepo, flatContainerRepository, valueOriginRepository)
      {
      }
   }
}
using System.Linq;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatParameterRateRepository : IMetaDataRepository<ParameterRateMetaData>
   {
   }

   public class FlatParameterRateRepository : MetaDataRepository<ParameterRateMetaData>, IFlatParameterRateRepository
   {
      public FlatParameterRateRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<ParameterRateMetaData> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_PARAMETER_RATES)
      {
      }
   }

   public class ParameterRateRepository : ParameterMetaDataRepository<ParameterRateMetaData>, IParameterRateRepository
   {
      private readonly IFlatParameterRHSRepository _flatParameterRHSRepository;

      public ParameterRateRepository(
         IFlatParameterRateRepository flatParameterRateRepo,
         IFlatContainerRepository flatContainerRepo,
         IFlatParameterRHSRepository flatParameterRHSRepository,
         IValueOriginRepository valueOriginRepository,
         IFlatContainerParameterDescriptorConditionRepository flatContainerParameterDescriptorConditionRepository,
         ICriteriaConditionToDescriptorConditionMapper descriptorConditionMapper) :
         base(flatParameterRateRepo, flatContainerRepo, valueOriginRepository, flatContainerParameterDescriptorConditionRepository, descriptorConditionMapper)
      {
         _flatParameterRHSRepository = flatParameterRHSRepository;
      }

      protected override void PerformPostStartProcessing()
      {
         foreach (var parameterRateMetaData in _parameterMetaDataList)
         {
            setRHSFor(parameterRateMetaData);
         }
      }

      private void setRHSFor(ParameterRateMetaData parameterRateMetaData)
      {
         var rhsItems = (from rhs in _flatParameterRHSRepository.All()
            where rhs.Id == parameterRateMetaData.ContainerId
            where rhs.ParameterName.Equals(parameterRateMetaData.ParameterName)
            where rhs.CalculationMethod.Equals(parameterRateMetaData.CalculationMethod)
            select rhs).ToList();

         //nor RHS for given parameter available
         if (!rhsItems.Any())
            return;

         //set name of RHS rate
         parameterRateMetaData.RHSRate = rhsItems.ElementAt(0).Rate;
      }
   }
}
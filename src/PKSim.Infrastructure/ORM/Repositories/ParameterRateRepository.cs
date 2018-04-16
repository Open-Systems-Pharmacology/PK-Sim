using System.Linq;
using FluentNHibernate.Utils;
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
         : base(dbGateway, mapper, CoreConstants.ORM.ViewParameterRates)
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
         IValueOriginRepository valueOriginRepository) :
         base(flatParameterRateRepo, flatContainerRepo, valueOriginRepository)
      {
         _flatParameterRHSRepository = flatParameterRHSRepository;
      }

      protected override void PerformPostStartProcessing()
      {
         foreach (var parameterRateMetaData in _parameterMetaDataList)
         {
            setRHSFor(parameterRateMetaData);
         }

         //TODO ZTMSE DELETE
         foreach (var key in _parameterMetaDataCacheByContainer.Keys)
         {
            if (key.Contains("Lumen") && key.Contains("MoleculeProperties") && key.Contains("Organism"))
            {
               var list = _parameterMetaDataCacheByContainer[key];
               var solubilityParameter = list.Find(x => x.Rate == "PARAM_IntestinalSolubility");
               var solubilityTableParameter = solubilityParameter.DoClone();
               solubilityTableParameter.ParameterName = CoreConstants.Parameters.SOLUBILITY_TABLE;
               solubilityTableParameter.Rate = $"{CoreConstants.Rate.TableFormulaWithXArgumentPrefix}_PARAM_IntestinalSolubility";
               list.Add(solubilityTableParameter);
            }

         }

      }

      private void setRHSFor(ParameterRateMetaData parameterRateMetaData)
      {
         var rhsItems = from rhs in _flatParameterRHSRepository.All()
            where rhs.Id == parameterRateMetaData.ContainerId
            where rhs.ParameterName.Equals(parameterRateMetaData.ParameterName)
            where rhs.CalculationMethod.Equals(parameterRateMetaData.CalculationMethod)
            select rhs;

         rhsItems = rhsItems.ToList();
         if (!rhsItems.Any())
            return; //nor RHS for given parameter available

         //set name of RHS rate
         parameterRateMetaData.RHSRate = rhsItems.ElementAt(0).Rate;
      }
   }
}
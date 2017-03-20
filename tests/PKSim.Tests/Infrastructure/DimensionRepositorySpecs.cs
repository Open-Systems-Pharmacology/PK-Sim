using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_DimensionRepository : ContextSpecification<IDimensionRepository>
   {
      protected override void Context()
      {
         var pkSimConfiguration = new PKSimConfiguration
         {
            DimensionFilePath = DomainHelperForSpecs.DimensionFilePath,
            PKParametersFilePath = DomainHelperForSpecs.PKParametersFilePath
         };
         var serializerRepository = new UnitSystemXmlSerializerRepository();
         serializerRepository.PerformMapping();
         sut = new DimensionRepository(new PKSimDimensionFactory(), serializerRepository, pkSimConfiguration);
      }
   }

   public class When_retrieving_the_age_dimension : concern_for_DimensionRepository
   {
      private IDimension _ageDimension;

      protected override void Context()
      {
         base.Context();
         _ageDimension = sut.AgeInYears;
      }

      [Observation]
      public void should_be_able_to_convert_from_years_to_months()
      {
         var monthUnit = _ageDimension.Unit("month(s)");
         _ageDimension.BaseUnitValueToUnitValue(monthUnit, 1).ShouldBeEqualTo(12);
      }
   }
}
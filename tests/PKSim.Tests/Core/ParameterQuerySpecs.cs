using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using FakeItEasy;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterQuery : ContextSpecification<IParameterQuery> 
   {
      protected OriginData _originData;
      protected IEntityPathResolver _containerPathRetriever;
      protected Compartment _compartment;

      protected override void Context()
      {

         _compartment = A.Fake<Compartment>();
         A.CallTo(() => _compartment.OrganName).Returns("MyParentOrgan");
         _compartment.Name = "MyComp";
         var containerPath = new ObjectPath(new[] { _compartment.OrganName, _compartment.Name }).ToString();
         _containerPathRetriever = A.Fake<IEntityPathResolver>();
         A.CallTo(() => _containerPathRetriever.PathFor(_compartment)).Returns(containerPath);
         _originData = new OriginData();
         _originData.SpeciesPopulation = new SpeciesPopulation();
         var distributionDefinition = new ParameterDistributionMetaData();
         var rateDefinition = new ParameterRateMetaData();
         var valueDefinition = new ParameterValueMetaData();

         distributionDefinition.ParameterName = "tutu";
         var distributionRepository = A.Fake<IParameterDistributionRepository>();
         A.CallTo(() => distributionRepository.AllFor(containerPath)).Returns(new List<ParameterDistributionMetaData> { distributionDefinition });

         var rateRepository = A.Fake<IParameterRateRepository>();
         A.CallTo(() => rateRepository.AllFor(containerPath)).Returns(new List<ParameterRateMetaData> { rateDefinition });

         var valueRepository = A.Fake<IParameterValueRepository>();
         A.CallTo(() => valueRepository.AllFor(containerPath)).Returns(new List<ParameterValueMetaData> { valueDefinition });

         sut = new ParameterQuery(distributionRepository, valueRepository, rateRepository, _containerPathRetriever);
      }
   }

  
   
   public class When_retrieving_the_list_of_all_rates_parameters_for_a_compartment : concern_for_ParameterQuery
   {
      private IEnumerable<ParameterRateMetaData> _result;

      protected override void Because()
      {
         _result = sut.ParameterRatesFor(_compartment, new[] {"Individual"});
      }

      [Observation]
      public void should_return_the_list_from_the_database_for_that_compartment()
      {
         _result.ShouldNotBeNull();
      }
   }

   
   public class When_retrieving_the_list_of_all_distributions_parameters_for_a_compartment : concern_for_ParameterQuery
   {
      private IEnumerable<ParameterDistributionMetaData> _result;

      protected override void Because()
      {
         _result = sut.ParameterDistributionsFor(_compartment, _originData);
      }

      [Observation]
      public void should_return_the_list_from_the_database_for_that_compartment()
      {
         _result.ShouldNotBeNull();
      }
   }

   
   public class When_retrieving_the_list_of_all_parameter_values_for_a_compartment : concern_for_ParameterQuery
   {
      private IEnumerable<ParameterValueMetaData> _result;

      protected override void Because()
      {
         _result = sut.ParameterValuesFor(_compartment, _originData);
      }

      [Observation]
      public void should_return_the_list_from_the_database_for_that_compartment()
      {
         _result.ShouldNotBeNull();
      }
   }
}
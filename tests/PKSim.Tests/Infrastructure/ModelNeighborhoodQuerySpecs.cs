using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Queries;
using PKSim.Infrastructure.ORM.Repositories;
using FakeItEasy;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ModelNeighborhoodQuery : ContextSpecification<IModelNeighborhoodQuery>
   {
      protected IFlatNeighborhoodRepository _neighborhoodRepository;
      protected IFlatModelContainerRepository _modelContainerRepository;
      protected List<FlatModelContainer> _allFlatNeighborhoods;
      protected FlatModelContainer _flatNeighborhoodFor3Comp;
      protected FlatModelContainer _flatNeighborhoodFor4Comp;
      protected ModelProperties _modelProperties;
      protected INeighborhoodBuilderFactory _neighborhoodBuilderFactory;

      protected override void Context()
      {
         base.Context();
         _neighborhoodRepository = A.Fake<IFlatNeighborhoodRepository>();
         _modelContainerRepository = A.Fake<IFlatModelContainerRepository>();
         _neighborhoodBuilderFactory =A.Fake<INeighborhoodBuilderFactory>();
         _modelProperties = A.Fake<ModelProperties>();
         _modelProperties.ModelConfiguration = A.Fake<ModelConfiguration>();
         _modelProperties.ModelConfiguration.ModelName = "3Comp";
         _allFlatNeighborhoods = new List<FlatModelContainer>();
         A.CallTo(() => _modelContainerRepository.All()).Returns(_allFlatNeighborhoods);
         _flatNeighborhoodFor3Comp = new FlatModelContainer {Model = "3Comp", Type = CoreConstants.ContainerType.Neighborhood, Id = 1};
         _flatNeighborhoodFor4Comp = new FlatModelContainer {Model = "4Comp", Type = CoreConstants.ContainerType.Neighborhood, Id = 2};
         _allFlatNeighborhoods.Add(_flatNeighborhoodFor3Comp);
         _allFlatNeighborhoods.Add(_flatNeighborhoodFor4Comp);
         sut = new ModelNeighborhoodQuery(_modelContainerRepository,_neighborhoodBuilderFactory);
      }
   }

   
   public class When_retrieving_some_neighborhoods_for_a_given_model_properties_that_were_not_defined_in_the_individual_but_were_marked_as_required : concern_for_ModelNeighborhoodQuery
   {
      protected override void Context()
      {
         base.Context();
         _flatNeighborhoodFor3Comp.UsageInIndividual = CoreConstants.ORM.USAGE_IN_INDIVIDUAL_REQUIRED;
      }

      [Observation]
      public void should_throw_an_exception_stipulating_that_the_neighborhood_is_missing_in_the_individual()
      {
         The.Action(() => sut.NeighborhoodsFor(new Container(), _modelProperties)).ShouldThrowAn<ArgumentException>();
      }
   }

   
   public class When_retrieving_some_neighborhoods_for_a_given_model_properties_that_were_not_defined_in_the_individual_but_were_marked_as_unknown : concern_for_ModelNeighborhoodQuery
   {
      protected override void Context()
      {
         base.Context();
         _flatNeighborhoodFor3Comp.UsageInIndividual = string.Empty;
      }

      [Observation]
      public void should_throw_an_exception_()
      {
         The.Action(() => sut.NeighborhoodsFor(new Container(), _modelProperties)).ShouldThrowAn<ArgumentException>();
      }
   }


   
   public class When_retrieving_some_neighborhoods_for_a_given_model_properties_that_wwere_not_defined_in_the_individual_but_were_marked_as_optional : concern_for_ModelNeighborhoodQuery
   {
      private IEnumerable<INeighborhoodBuilder> _result;

      protected override void Context()
      {
         base.Context();
         _flatNeighborhoodFor3Comp.UsageInIndividual = CoreConstants.ORM.USAGE_IN_INDIVIDUAL_OPTIONAL;
      }

      protected override void Because()
      {
         _result = sut.NeighborhoodsFor(new Container(), _modelProperties);
      }

      [Observation]
      public void should_return_the_available_neighborhoods()
      {
         _result.ShouldNotBeNull();
      }
   }

  
   
   public class When_retrieving_some_neighborhoods_for_a_given_model_propertie_that_were_already_defined_in_the_individual : concern_for_ModelNeighborhoodQuery
   {
      private IEnumerable<INeighborhoodBuilder> _result;
      private FlatNeighborhood _flatNeighborhood;
      private INeighborhoodBuilder _neighborhoodBuilder;
      private IContainer _individualNeighborhoods;

      protected override void Context()
      {
         base.Context();
         _individualNeighborhoods = new Container();
         _individualNeighborhoods.Add(new Container().WithName(_flatNeighborhoodFor3Comp.Name));
         _neighborhoodBuilder = A.Fake<INeighborhoodBuilder>();
         _flatNeighborhood = new FlatNeighborhood { Name = "tralala" };
         _flatNeighborhoodFor3Comp.UsageInIndividual = CoreConstants.ORM.USAGE_IN_INDIVIDUAL_REQUIRED;
         A.CallTo(() => _neighborhoodRepository.NeighborhoodFrom(_flatNeighborhoodFor3Comp.Id)).Returns(_flatNeighborhood);
         A.CallTo(() => _neighborhoodBuilderFactory.Create()).Returns(_neighborhoodBuilder);
      }

      protected override void Because()
      {
         _result = sut.NeighborhoodsFor(_individualNeighborhoods, _modelProperties);
      }

      [Observation]
      public void should_return_the_new_neighborhoods()
      {
         _result.Contains(_neighborhoodBuilder).ShouldBeTrue();
      }
   }
}
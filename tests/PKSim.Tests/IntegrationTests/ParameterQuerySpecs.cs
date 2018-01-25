﻿using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ParameterQuery : ContextForIntegration<IParameterQuery>
   {
      protected IContainer _container;
      protected OriginData _originData;

      protected override void Context()
      {
         base.Context();
         _originData = new OriginData();
         _originData.Species = new Species().WithName(CoreConstants.Species.HUMAN);
         _originData.SpeciesPopulation = new SpeciesPopulation().WithName(CoreConstants.Population.ICRP);
         _originData.Age = 25;
         _originData.Gender = new Gender().WithName(CoreConstants.Gender.Male);
         _originData.SubPopulation=new SubPopulation();
         _originData.SubPopulation.AddParameterValueVersion(new ParameterValueVersion().WithName(CoreConstants.ParameterValueVersion.IndividualPKSim));
         _container = new Container().WithName("Liver").WithParentContainer(new Organism().WithParentContainer(new RootContainer()));
      }
   }

   
   public class When_returning_the_distributed_parameter_defined_for_an_origin_data_with_gestational_age_not_available_in_db : concern_for_ParameterQuery
   {
      private IEnumerable<ParameterDistributionMetaData> _results;

      protected override void Context()
      {
         base.Context();
         _originData.GestationalAge = 38;
      }

      protected override void Because()
      {
         _results = sut.ParameterDistributionsFor(_container, _originData);
      }
      
      [Observation]
      public void should_return_the_data_defined_in_the_database_for_gestational_age_40()
      {
         _results.Count().ShouldBeGreaterThan(0);
      }
   }

   
   public class When_returning_the_distributed_parameter_defined_for_an_origin_data_with_gestational_age_available_in_db : concern_for_ParameterQuery
   {
      private IEnumerable<ParameterDistributionMetaData> _results;

      protected override void Context()
      {
         base.Context();
         _originData.GestationalAge = 40;
      }

      protected override void Because()
      {
         _results = sut.ParameterDistributionsFor(_container, _originData);
      }

      [Observation]
      public void should_return_the_data_defined_in_the_database_for_that_age()
      {
         _results.Count().ShouldBeGreaterThan(0);
      }
   }
}	
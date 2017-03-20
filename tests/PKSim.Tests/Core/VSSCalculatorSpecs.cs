using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core
{
   public abstract class concern_for_VSSCalculator : ContextSpecification<IVSSCalculator>
   {
      protected IDefaultIndividualRetriever _defaultIndividualRetriever;
      protected ISpeciesRepository _speciesRepository;
      protected ISimulationFactory _simulationFactory;
      protected IParameterFactory _parameterFactory;
      protected IProtocolFactory _protocolFactory;
      protected Species _species1;
      protected Species _species2;
      protected Simulation _simulation;

      protected override void Context()
      {
         _defaultIndividualRetriever = A.Fake<IDefaultIndividualRetriever>();
         _speciesRepository = A.Fake<ISpeciesRepository>();
         _simulationFactory = A.Fake<ISimulationFactory>();
         _parameterFactory = A.Fake<IParameterFactory>();
         _protocolFactory = A.Fake<IProtocolFactory>();

         sut = new VSSCalculator(_defaultIndividualRetriever,_speciesRepository,_simulationFactory,_parameterFactory,_protocolFactory);
         _species1 = new Species().WithId("Human");
         _species2= new Species().WithId("Dog");
         A.CallTo(() => _speciesRepository.All()).Returns(new[] { _species1, _species2});

         _simulation = A.Fake<Simulation>();
         var organism = new Organism();
         organism.Add(DomainHelperForSpecs.ConstantParameterWithValue(0.5).WithName(CoreConstants.Parameter.HCT));
         A.CallTo(() => _simulation.Individual.Organism).Returns(organism);

      }
   }

   public class When_calculating_the_vss_for_all_available_species_for_a_given_compound : concern_for_VSSCalculator
   {
      private Compound _compound;
      private ICache<Species, IParameter> _result;

      protected override void Context()
      {
         base.Context();
         _compound= A.Fake<Compound>();
         A.CallTo(() => _simulationFactory.CreateForVSS(A<Protocol>._,A<Individual>._,_compound)).Returns(_simulation);
      }

      protected override void Because()
      {
         _result = sut.VSSPhysChemFor(_compound);
      }

      [Observation]
      public void should_return_a_cache_containing_one_entry_for_each_availalbe_species   ()
      {
         _result[_species1].ShouldNotBeNull();
         _result[_species2].ShouldNotBeNull();
      }
   }
   public class When_calculating_the_vss_for_a_given_simulation: concern_for_VSSCalculator
   {
      private ICache<string, IParameter> _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _simulation.CompoundNames).Returns(new[]{"DRUG", "METABOLITE"});
      }

      protected override void Because()
      {
         _result = sut.VSSPhysChemFor(_simulation);
      }

      [Observation]
      public void should_return_a_cache_containing_one_entry_for_each_availalbe_compound_in_the_simulation()
      {
         _result["DRUG"].ShouldNotBeNull();
         _result["METABOLITE"].ShouldNotBeNull();
      }
   }

   public class When_creating_a_vss_parameter_for_a_given_vss_value : concern_for_VSSCalculator
   {
      private IParameter _vssParameter;

      protected override void Context()
      {
         base.Context();
         _vssParameter= A.Fake<IParameter>();
         A.CallTo(() => _parameterFactory.CreateFor(CoreConstants.PKAnalysis.VssPhysChem, 10, CoreConstants.Dimension.VolumePerBodyWeight, PKSimBuildingBlockType.Simulation)).Returns(_vssParameter);
      }

      [Observation]
      public void should_leverage_the_parameter_factory_to_create_the_expected_parameter()
      {
         sut.VSSParameterWithValue(10).ShouldBeEqualTo(_vssParameter);
      }
   }
}	
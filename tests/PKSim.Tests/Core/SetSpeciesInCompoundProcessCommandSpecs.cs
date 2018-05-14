using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_SetSpeciesInCompoundProcessCommand : ContextSpecification<SetSpeciesInCompoundProcessCommand>
   {
      protected EnzymaticProcessWithSpecies _compoundProcess;
      protected Species _species;
      protected IExecutionContext _executionContext;
      protected IDefaultIndividualRetriever _defaultIndividualRetriever;
      protected Individual _individual;
      protected ICompoundProcessParameterMappingRepository _parameterMappingRepository;
      protected IParameter _matchingIndividualParameter;
      protected IParameter _nonMatchingIndividualParameter;
      protected Organism _organism;
      protected IParameter _individualParameter;
      protected IObjectPath _objectPath;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _compoundProcess = new EnzymaticProcessWithSpecies {InternalName = "CompoundProcess"};
         _species = new Species {Name = "NEW_SPECIES"};
         _compoundProcess.Species = new Species { Name = "OLD_SPECIES" };
         sut = new SetSpeciesInCompoundProcessCommand(_compoundProcess, _species);

         _defaultIndividualRetriever= A.Fake<IDefaultIndividualRetriever>();
         A.CallTo(() => _executionContext.Resolve<IDefaultIndividualRetriever>()).Returns(_defaultIndividualRetriever);

         _parameterMappingRepository= A.Fake<ICompoundProcessParameterMappingRepository>();
         A.CallTo(() => _executionContext.Resolve<ICompoundProcessParameterMappingRepository>()).Returns(_parameterMappingRepository);

         _individualParameter = DomainHelperForSpecs.ConstantParameterWithValue(value:25).WithName("Matching");
         _individualParameter.ValueOrigin.Method = ValueOriginDeterminationMethods.Assumption;
         _individualParameter.ValueOrigin.Source = ValueOriginSources.ParameterIdentification;

         _organism = new Organism { _individualParameter };
         _individual = new Individual {Root = new RootContainer {_organism}};

         A.CallTo(() => _defaultIndividualRetriever.DefaultIndividualFor(_species)).Returns(_individual);
         _matchingIndividualParameter = DomainHelperForSpecs.ConstantParameterWithValue(isDefault: true).WithName("Matching");
         _nonMatchingIndividualParameter = DomainHelperForSpecs.ConstantParameterWithValue(isDefault: true).WithName("NonMatching");
         _compoundProcess.Add(_matchingIndividualParameter);
         _compoundProcess.Add(_nonMatchingIndividualParameter);

         _objectPath =new ObjectPath(_organism.Name, _individualParameter.Name);
         A.CallTo(() => _parameterMappingRepository.HasMappedParameterFor(_compoundProcess.InternalName, _matchingIndividualParameter.Name)).Returns(true);
         A.CallTo(() => _parameterMappingRepository.MappedParameterPathFor(_compoundProcess.InternalName, _matchingIndividualParameter.Name)).Returns(_objectPath);
      }
   }

   public class When_executing_the_set_species_in_compound_process_command : concern_for_SetSpeciesInCompoundProcessCommand
   {
      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_update_the_used_species_in_the_compound_process()
      {
         _compoundProcess.Species.ShouldBeEqualTo(_species);
      }

      [Observation]
      public void should_retrieve_all_parameters_in_the_selected_process_that_have_a_matching_parameter_in_the_organism_to_update_their_value()
      {
         _matchingIndividualParameter.Value.ShouldBeEqualTo(_individualParameter.Value);
      }

      [Observation]
      public void should_have_updated_the_is_default_flag_to_false_for_those_parameters()
      {
         _matchingIndividualParameter.IsDefault.ShouldBeFalse();
      }

      [Observation]
      public void should_have_updated_the_value_origin_to_match_the_individual_parameter_value_origin()
      {
         _matchingIndividualParameter.ValueOrigin.ShouldBeEqualTo(_individualParameter.ValueOrigin);
      }
   }
}
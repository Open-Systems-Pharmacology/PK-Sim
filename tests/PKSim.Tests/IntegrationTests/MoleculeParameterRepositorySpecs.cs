using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_MoleculeParameterRepository : ContextForIntegration<IMoleculeParameterRepository>
   {
      protected const string MOLECULE_WITH_PARAMTERS = "CYP3A4";
   }

   public class When_retrieving_all_parameters_defined_for_a_molecule_containing_default_parameters_in_the_database : concern_for_MoleculeParameterRepository
   {
      [Observation]
      public void should_return_the_expected_parameters()
      {
         sut.AllParametersFor(MOLECULE_WITH_PARAMTERS).Count.ShouldBeEqualTo(3);
         sut.ParameterFor(MOLECULE_WITH_PARAMTERS, CoreConstants.Parameter.REFERENCE_CONCENTRATION).ShouldNotBeNull();
         sut.ParameterFor(MOLECULE_WITH_PARAMTERS, CoreConstants.Parameter.HALF_LIFE_LIVER).ShouldNotBeNull();
         sut.ParameterFor(MOLECULE_WITH_PARAMTERS, CoreConstants.Parameter.HALF_LIFE_INTESTINE).ShouldNotBeNull();
      }

      [Observation]
      public void should_return_the_expected_values()
      {
         sut.ParameterValueFor(MOLECULE_WITH_PARAMTERS, CoreConstants.Parameter.REFERENCE_CONCENTRATION).ShouldBeEqualTo(4.32);
         sut.ParameterValueFor(MOLECULE_WITH_PARAMTERS, CoreConstants.Parameter.HALF_LIFE_LIVER).ShouldBeEqualTo(37 * 60);
         sut.ParameterValueFor(MOLECULE_WITH_PARAMTERS, CoreConstants.Parameter.HALF_LIFE_INTESTINE).ShouldBeEqualTo(CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_INTESTINE_VALUE_IN_MIN);
      }

      [Observation]
      public void should_return_the_default_value_given_when_asked_for_a_parameter_value_that_does_not_exist()
      {
         sut.ParameterValueFor(MOLECULE_WITH_PARAMTERS, "PARAMETER_THAT_DOES_NOT_EXIST", defaultValue: 10).ShouldBeEqualTo(10);
      }
   }

   public class When_retrieving_all_parameters_defined_for_a_molecule_that_is_not_in_the_database : concern_for_MoleculeParameterRepository
   {
      [Observation]
      public void should_return_an_empty_enumeration()
      {
         sut.AllParametersFor("XXX").ShouldBeEmpty();
      }

      [Observation]
      public void should_return_the_default_value_given_when_asked_for_a_parameter_value()
      {
         sut.ParameterValueFor("XXX", CoreConstants.Parameter.REFERENCE_CONCENTRATION, defaultValue: 10).ShouldBeEqualTo(10);
      }
   }

   public class When_retrieving_all_parameters_defined_for_a_molecule_defined_in_the_database_using_a_different_case : concern_for_MoleculeParameterRepository
   {
      [Observation]
      public void should_be_able_to_retrieve_the_parameters()
      {
         sut.AllParametersFor(MOLECULE_WITH_PARAMTERS.ToLower()).Count.ShouldBeEqualTo(3);
      }
   }

   public class When_retrieving_a_parameter_that_does_not_exist : concern_for_MoleculeParameterRepository
   {
      [Observation]
      public void should_return_null_if_the_molecule_does_not_exist()
      {
         sut.ParameterFor("XXX", CoreConstants.Parameter.HALF_LIFE_LIVER).ShouldBeNull();
      }

      [Observation]
      public void should_return_null_if_the_parameter_does_not_exist_for_an_existing_molecule()
      {
         sut.ParameterFor(MOLECULE_WITH_PARAMTERS, "PARAM").ShouldBeNull();
      }
   }
}
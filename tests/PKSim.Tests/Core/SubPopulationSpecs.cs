using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
    public abstract class concern_for_SubPopulation : ContextSpecification<SubPopulation>
    {
        protected override void Context()
        {
            sut = new SubPopulation();
        }
    }

    
    public class When_adding_parameter_value_versions_to_the_sub_population : concern_for_SubPopulation
    {
        private ParameterValueVersion _parameterValueVersion1;
        private ParameterValueVersion _parameterValueVersion2;

        protected override void Context()
        {
            base.Context();
            _parameterValueVersion1=new ParameterValueVersion {Name = "pv1"};
           _parameterValueVersion2 = new ParameterValueVersion {Name = "pv2"};
        }

        protected override void Because()
        {
            sut.AddParameterValueVersion(_parameterValueVersion1);
            sut.AddParameterValueVersion(_parameterValueVersion2);
        }

        [Observation]
        public void should_be_able_to_retrieve_this_subpopulation()
        {
            sut.ParameterValueVersions.ShouldOnlyContain(_parameterValueVersion1, _parameterValueVersion2);
        }
    }

    
    public class When_a_sub_population_is_asked_if_it_contains_a_parameter_value_version : concern_for_SubPopulation
    {
        private ParameterValueVersion _existingParameterValueVersion;
        private ParameterValueVersion _parameterValueVersionThatDoesNotExist;

        protected override void Context()
        {
            base.Context();
            _parameterValueVersionThatDoesNotExist =new ParameterValueVersion {Name = "doesnotexist"};
           _existingParameterValueVersion =new ParameterValueVersion {Name = "exists"};
           sut.AddParameterValueVersion(_existingParameterValueVersion);
        }

        [Observation]
        public void should_return_true_if_the_sub_population_contains_the_parameter_value_version()
        {
            sut.Contains(_existingParameterValueVersion.Name).ShouldBeTrue();
        }

        [Observation]
        public void should_return_false_if_the_sub_population_does_not_contain_the_parameter_value_version()
        {
            sut.Contains(_parameterValueVersionThatDoesNotExist.Name).ShouldBeFalse();
        }

    }
}	
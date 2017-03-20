using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
    public abstract class concern_for_ModelProperties : ContextSpecification<ModelProperties>
    {
        protected override void Context()
        {
            sut = new ModelProperties();
        }
    }

    
    public class When_adding_a_calculation_method_to_the_model_properties : concern_for_ModelProperties
    {
        private CalculationMethod _calculationMethod;

        protected override void Context()
        {
            base.Context();
            _calculationMethod = new CalculationMethod {Name = "tralala"};
        }

        protected override void Because()
        {
            sut.AddCalculationMethod(_calculationMethod);
        }

        [Observation]
        public void should_be_able_to_retrieve_it_by_name()
        {
            sut.ContainsCalculationMethod(_calculationMethod.Name).ShouldBeTrue();
        }

        [Observation]
        public void should_be_able_to_retrieve_it_in_the_set_of_all_available_calculation_methods()
        {
            sut.AllCalculationMethods().ShouldOnlyContain(_calculationMethod);
        }
    }
}
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
    public abstract class concern_for_rate_key : ContextSpecification<RateKey>
    {
        protected string _rate;
        protected string _calculationMethod;

        protected override void Context()
        {
            _rate = "rate";
            _calculationMethod = "cm";
            sut = new RateKey(_calculationMethod,_rate);
        }
    }

    
    public class When_comparing_two_rate_keys_with_the_same_calculation_method_and_rate : concern_for_rate_key
    {
        [Observation]
        public void the_rate_key_should_be_equal()
        {
            sut.ShouldBeEqualTo(new RateKey(_calculationMethod,_rate));
            sut.ShouldBeEqualTo(sut);
        }
    }


    
    public class When_comparing_two_rate_keys_with_different_calculation_method_or_rate : concern_for_rate_key
    {
        [Observation]
        public void the_rate_key_should_not_be_equal()
        {
            sut.ShouldNotBeEqualTo(new RateKey("A", "b"));
            sut.ShouldNotBeEqualTo(new RateKey(_calculationMethod, "b"));
            sut.ShouldNotBeEqualTo(new RateKey("A", _rate));
            sut.ShouldNotBeEqualTo(new RateKey(_rate, _calculationMethod));
        }
    }

    
    public class When_comparing_the_hash_code_of_two_rate_keys_with_the_same_rate_and_calculation_method : concern_for_rate_key
    {
        private RateKey _otherKey;

        protected override void Context()
        {
            sut = new RateKey("GiTract_PKSim", "Param_VolumeGut");
            _otherKey = new RateKey("GiTract_PKSim", "Param_VolumeGut");
        }

        [Observation]
        public void the_hash_code_should_be_equal()
        {
            sut.ShouldBeEqualTo(_otherKey);
            sut.GetHashCode().ShouldBeEqualTo(_otherKey.GetHashCode());

        }
    }
}	
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_OriginData : ContextSpecification<OriginData>
    {
        protected override void Context()
        {
            sut = new OriginData
            {
               Age = new OriginDataParameter(10),
               GestationalAge = new OriginDataParameter(20),
               Comment = "tralala",
               Gender = new Gender{Name = "gender"},
               Height = new OriginDataParameter(25),
               SpeciesPopulation = new SpeciesPopulation { Name = "population" },
               Species = new Species { Name = "species" },
               SubPopulation = new SubPopulation(),
               Weight = new OriginDataParameter(50)
            };
        }
    }

    
    public class When_cloning_an_origin_data : concern_for_OriginData
    {
       private OriginData _result;

        protected override void Because()
        {
            _result = sut.Clone();
        }

        [Observation]
        public void should_return_an_origin_data_object_containing_the_same_properties()
        {
            _result.Age.Value.ShouldBeEqualTo(sut.Age.Value);
            _result.Comment.ShouldBeEqualTo(sut.Comment);
            _result.Gender.ShouldBeEqualTo(sut.Gender);
            _result.Height.Value.ShouldBeEqualTo(sut.Height.Value);
            _result.SpeciesPopulation.ShouldBeEqualTo(sut.SpeciesPopulation);
            _result.Species.ShouldBeEqualTo(sut.Species);
            _result.SubPopulation.ShouldBeEqualTo(sut.SubPopulation);
            _result.Weight.Value.ShouldBeEqualTo(sut.Weight.Value);
            _result.GestationalAge.Value.ShouldBeEqualTo(sut.GestationalAge.Value);
        }
    }

}
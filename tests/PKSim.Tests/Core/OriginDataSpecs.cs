using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_OriginData : ContextSpecification<OriginData>
    {
        protected override void Context()
        {
            sut = new OriginData();
            sut.Age = 10;
            sut.GestationalAge = 20;
            sut.Comment = "tralala";
            sut.Gender = new Gender{Name = "gender"};
            sut.Height = 25;
            sut.SpeciesPopulation = new SpeciesPopulation { Name = "population" };
            sut.Species = new Species { Name = "species" }; 
            sut.SubPopulation = new SubPopulation();
            sut.Weight = 50;
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
            _result.Age.ShouldBeEqualTo(sut.Age);
            _result.Comment.ShouldBeEqualTo(sut.Comment);
            _result.Gender.ShouldBeEqualTo(sut.Gender);
            _result.Height.ShouldBeEqualTo(sut.Height);
            _result.SpeciesPopulation.ShouldBeEqualTo(sut.SpeciesPopulation);
            _result.Species.ShouldBeEqualTo(sut.Species);
            _result.SubPopulation.ShouldBeEqualTo(sut.SubPopulation);
            _result.Weight.ShouldBeEqualTo(sut.Weight);
            _result.GestationalAge.ShouldBeEqualTo(sut.GestationalAge);
        }
    }

}
using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;

namespace PKSim.Core
{
    public abstract class concern_for_container_to_container_type_mapper : ContextSpecification<IContainerToContainerTypeMapper>
    {
        protected override void Context()
        {
            sut = new ContainerToContainerTypeMapper();
        }
    }

    
    public class when_getting_container_type : concern_for_container_to_container_type_mapper
    {
        [Observation]
        public void should_return_type_compartment()
        {
            sut.MapFrom(new Compartment()).ShouldBeEqualTo(PKSimContainerType.Compartment);
        }

        [Observation]
        public void should_return_type_compound()
        {
            sut.MapFrom(new Compound()).ShouldBeEqualTo(PKSimContainerType.Compound);
        }


        [Observation]
        public void should_return_type_organ()
        {
            sut.MapFrom(new Organ()).ShouldBeEqualTo(PKSimContainerType.Organ);
        }

        [Observation]
        public void should_return_type_organism()
        {
            sut.MapFrom(new Organism()).ShouldBeEqualTo(PKSimContainerType.Organism);
        }

        [Observation]
        public void should_throw_an_exception_for_an_unexpected_type()
        {
           The.Action(()=>sut.MapFrom(new IndividualSimulation())).ShouldThrowAn<Exception>();
        }

    }
}
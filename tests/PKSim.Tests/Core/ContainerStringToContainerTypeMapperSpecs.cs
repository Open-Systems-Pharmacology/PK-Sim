using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;

namespace PKSim.Core
{
    public abstract class concern_for_container_string_to_container_type_mapper : ContextSpecification<IContainerStringToContainerTypeMapper>
    {
        protected override void Context()
        {
            sut = new ContainerStringToContainerTypeMapper();
        }
    }

    
    public class When_mapping_a_string_to_a_container_type_with_the_container_type_mapper : concern_for_container_string_to_container_type_mapper
    {
        [Observation]
        public void should_return_the_accurate_link_type_for_a_valid_string()
        {
            sut.MapFrom("Organ").ShouldBeEqualTo(PKSimContainerType.Organ);
            sut.MapFrom("Compartment").ShouldBeEqualTo(PKSimContainerType.Compartment);
            sut.MapFrom("Simulation").ShouldBeEqualTo(PKSimContainerType.Root);
        }

        [Observation]
        public void should_throw_an_exception_if_the_string_is_not_recognized()
        {
            The.Action(() => sut.MapFrom("tralal")).ShouldThrowAn<Exception>();
        }
    }
}
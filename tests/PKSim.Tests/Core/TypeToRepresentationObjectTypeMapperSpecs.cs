using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core
{
   public abstract class concern_for_type_to_representation_object_type_mapper : ContextSpecification<ITypeToRepresentationObjectTypeMapper>
   {
      protected override void Context()
      {
         sut = new TypeToRepresentationObjectTypeMapper();
      }
   }

   public class When_mapping_a_type_with_a_matching_representation_object_type : concern_for_type_to_representation_object_type_mapper
   {
      [Observation]
      public void should_return_the_corresponding_type()
      {
         sut.MapFrom(typeof (Compartment)).ShouldBeEqualTo(RepresentationObjectType.CONTAINER);
         sut.MapFrom(typeof (IParameter)).ShouldBeEqualTo(RepresentationObjectType.PARAMETER);
         sut.MapFrom(typeof (PKSimDistributedParameter)).ShouldBeEqualTo(RepresentationObjectType.PARAMETER);
         sut.MapFrom(typeof (CalculationMethod)).ShouldBeEqualTo(RepresentationObjectType.CALCULATION_METHOD);
         sut.MapFrom(typeof (ParameterValueVersionCategory)).ShouldBeEqualTo(RepresentationObjectType.CATEGORY);
         sut.MapFrom(typeof (CalculationMethodCategory)).ShouldBeEqualTo(RepresentationObjectType.CATEGORY);
         sut.MapFrom(typeof (IDimension)).ShouldBeEqualTo(RepresentationObjectType.DIMENSION);
         sut.MapFrom(typeof (Gender)).ShouldBeEqualTo(RepresentationObjectType.GENDER);
         sut.MapFrom(typeof (IGroup)).ShouldBeEqualTo(RepresentationObjectType.GROUP);
         sut.MapFrom(typeof (ModelConfiguration)).ShouldBeEqualTo(RepresentationObjectType.MODEL);
         sut.MapFrom(typeof (IObserver)).ShouldBeEqualTo(RepresentationObjectType.OBSERVER);
         sut.MapFrom(typeof (ParameterValueVersion)).ShouldBeEqualTo(RepresentationObjectType.PARAMETER_VALUE_VERSION);
         sut.MapFrom(typeof (SpeciesPopulation)).ShouldBeEqualTo(RepresentationObjectType.POPULATION);
         sut.MapFrom(typeof (ParameterAlternativeGroup)).ShouldBeEqualTo(RepresentationObjectType.GROUP);
         sut.MapFrom(typeof (Species)).ShouldBeEqualTo(RepresentationObjectType.SPECIES);
      }
   }

   public class When_mapping_an_unknown_type : concern_for_type_to_representation_object_type_mapper
   {
      [Observation]
      public void should_return_an_empty_tyme()
      {
         sut.MapFrom(typeof (ICommand)).ShouldBeEqualTo(RepresentationObjectType.UNKNOWN);
      }
   }
}
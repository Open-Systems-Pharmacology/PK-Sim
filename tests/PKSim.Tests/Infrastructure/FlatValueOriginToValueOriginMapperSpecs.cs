using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_FlatValueOriginToValueOriginMapper : ContextSpecification<IFlatValueOriginToValueOriginMapper>
   {
      protected FlatValueOrigin _flatValueOrigin;

      protected override void Context()
      {
         sut = new FlatValueOriginToValueOriginMapper();

         _flatValueOrigin =new FlatValueOrigin();
      }
   }

   public class When_mapping_a_flat_value_origin_to_a_value_origin : concern_for_FlatValueOriginToValueOriginMapper
   {
      private ValueOrigin _valueOrigin;

      protected override void Context()
      {
         base.Context();
         _flatValueOrigin.Description = "Hello";
         _flatValueOrigin.Method = ValueOriginDeterminationMethodId.InVitro;
         _flatValueOrigin.Source = ValueOriginSourceId.Internet;
      }

      protected override void Because()
      {
         _valueOrigin = sut.MapFrom(_flatValueOrigin);
      }

      [Observation]
      public void should_return_a_value_origin_having_the_expected_properties()
      {
         _valueOrigin.Description.ShouldBeEqualTo(_flatValueOrigin.Description);
         _valueOrigin.Method.ShouldBeEqualTo(ValueOriginDeterminationMethods.InVitro);
         _valueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.Internet);
      }
   }
}	
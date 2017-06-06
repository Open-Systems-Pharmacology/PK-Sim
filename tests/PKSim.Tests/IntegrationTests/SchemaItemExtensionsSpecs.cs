using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_SchemaItemExtensions : StaticContextSpecification
   {
      protected ISchemaItem _schemaItem;
      protected Unit _unit;

      protected override void Context()
      {
         _schemaItem = A.Fake<ISchemaItem>();
         _unit= A.Fake<Unit>();
         _schemaItem.Dose.DisplayUnit = _unit;
      }
   }

   public class When_checking_if_a_schema_item_is_dosing_per_mass : concern_for_SchemaItemExtensions
   {
      [Observation]
      public void should_return_true_if_the_dose_is_in_mg()
      {
         A.CallTo(() => _unit.Name).Returns(CoreConstants.Units.mg);
         _schemaItem.DoseIsInMass().ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_otherwise()
      {
         A.CallTo(() => _unit.Name).Returns("AA");
         _schemaItem.DoseIsInMass().ShouldBeFalse();
      }
   }


   public class When_checking_if_a_schema_item_is_dosing_per_body_weight : concern_for_SchemaItemExtensions
   {
      [Observation]
      public void should_return_true_if_the_dose_is_in_mg_per_kg()
      {
         A.CallTo(() => _unit.Name).Returns(CoreConstants.Units.MgPerKg);
         _schemaItem.DoseIsPerBodyWeight().ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_otherwise()
      {
         A.CallTo(() => _unit.Name).Returns("AA");
         _schemaItem.DoseIsInMass().ShouldBeFalse();
      }
   }

   public class When_checking_if_a_schema_item_is_dosing_per_body_surface_area : concern_for_SchemaItemExtensions
   {
      [Observation]
      public void should_return_true_if_the_dose_is_in_mg_per_m2()
      {
         A.CallTo(() => _unit.Name).Returns(CoreConstants.Units.MgPerM2);
         _schemaItem.DoseIsPerBodySurfaceArea().ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_otherwise()
      {
         A.CallTo(() => _unit.Name).Returns("AA");
         _schemaItem.DoseIsInMass().ShouldBeFalse();
      }
   }
}
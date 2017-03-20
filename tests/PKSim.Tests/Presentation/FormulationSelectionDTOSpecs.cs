using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_FormulationSelectionDTO : ContextSpecification<FormulationSelectionDTO>
   {
      protected Formulation _formulation1;

      protected override void Context()
      {
         _formulation1 = new Formulation {Id = "TOTO"};
         sut = new FormulationSelectionDTO {BuildingBlock = _formulation1};
      }
   }

   public class When_comparing_two_formulation_selection_dto : concern_for_FormulationSelectionDTO
   {
      [Observation]
      public void should_return_equal_if_the_underlying_building_blocks_are_equal()
      {
         sut.Equals(new FormulationSelectionDTO {BuildingBlock = _formulation1}).ShouldBeTrue();
      }

      [Observation]
      public void should_return_unequal_otherwise()
      {
         sut.Equals(new FormulationSelectionDTO {BuildingBlock = new Formulation()}).ShouldBeFalse();
      }
   }
}
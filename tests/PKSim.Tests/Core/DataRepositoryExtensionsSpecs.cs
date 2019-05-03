using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core
{
   public class When_resolving_the_venous_blood_plasma_column : StaticContextSpecification
   {
      private DataColumn _venousBloodPlasma;
      private DataColumn _venousBloodUnboundPlasma;

      protected override void Context()
      {
         base.Context();
         _venousBloodPlasma = new DataColumn {DataInfo = new DataInfo(ColumnOrigins.Calculation)};
         _venousBloodPlasma.Name = CoreConstants.Observer.CONCENTRATION_IN_CONTAINER;
         _venousBloodUnboundPlasma = new DataColumn {DataInfo = new DataInfo(ColumnOrigins.Calculation)};
         _venousBloodUnboundPlasma.Name = CoreConstants.Compartment.Plasma;
         _venousBloodPlasma.QuantityInfo = new QuantityInfo("Concentration", new[] {CoreConstants.Organ.VenousBlood, CoreConstants.Compartment.Plasma, "Drug"}, QuantityType.Drug);
         _venousBloodUnboundPlasma.QuantityInfo = new QuantityInfo("Concentration", new[] {CoreConstants.Organ.VenousBlood, CoreConstants.Observer.PLASMA_UNBOUND}, QuantityType.Drug);
      }

      [Observation]
      public void should_not_return_venous_blood_unbound_plasma()
      {
         var dataRepository = new DataRepository("tralalal");
         dataRepository.Add(_venousBloodUnboundPlasma);
         dataRepository.Add(_venousBloodPlasma);
         dataRepository.VenousBloodColumn("Drug").ShouldBeEqualTo(_venousBloodPlasma);
      }
   }
}
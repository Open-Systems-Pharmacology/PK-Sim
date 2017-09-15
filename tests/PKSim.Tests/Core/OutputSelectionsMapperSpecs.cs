using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_OutputSelectionsMapper : ContextSpecificationAsync<OutputSelectionsMapper>
   {
      protected OutputSelections _outputSelections;
      protected QuantitySelection _quantitySelection1;
      protected QuantitySelection _quantitySelection2;
      protected Snapshots.OutputSelections _snapshot;

      protected override Task Context()
      {
         sut = new OutputSelectionsMapper();

         _quantitySelection1 = new QuantitySelection("PATH1", QuantityType.Drug);
         _quantitySelection2 = new QuantitySelection("PATH2", QuantityType.Observer);

         _outputSelections = new OutputSelections();
         _outputSelections.AddOutput(_quantitySelection1);
         _outputSelections.AddOutput(_quantitySelection2);


         return Task.FromResult(true);
      }
   }

   public class When_mapping_the_output_selections_to_snapshot : concern_for_OutputSelectionsMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_outputSelections);
      }

      [Observation]
      public void should_return_a_snapshot_with_one_entry_for_each_selected_output()
      {
         _snapshot.ShouldOnlyContain(_quantitySelection1.Path, _quantitySelection2.Path);
      }
   }
}
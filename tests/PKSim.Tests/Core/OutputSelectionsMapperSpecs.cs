using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using OutputSelections = OSPSuite.Core.Domain.OutputSelections;
using Parameter = OSPSuite.Core.Domain.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_OutputSelectionsMapper : ContextSpecificationAsync<OutputSelectionsMapper>
   {
      protected OutputSelections _outputSelections;
      protected QuantitySelection _quantitySelection1;
      protected QuantitySelection _quantitySelection2;
      protected Snapshots.OutputSelections _snapshot;
      protected IEntitiesInContainerRetriever _entitiesInContainerRetriever;
      protected IndividualSimulation _simulation;
      protected PathCache<IQuantity> _allQuantities;
      protected ILogger _logger;

      protected override Task Context()
      {
         _entitiesInContainerRetriever = A.Fake<IEntitiesInContainerRetriever>();
         _logger= A.Fake<ILogger>();
         sut = new OutputSelectionsMapper(_entitiesInContainerRetriever, _logger);

         _quantitySelection1 = new QuantitySelection("PATH1", QuantityType.Drug);
         _quantitySelection2 = new QuantitySelection("PATH2", QuantityType.Observer);

         _outputSelections = new OutputSelections();
         _outputSelections.AddOutput(_quantitySelection1);
         _outputSelections.AddOutput(_quantitySelection2);

         _simulation = new IndividualSimulation();
         _allQuantities = new PathCacheForSpecs<IQuantity>();

         A.CallTo(() => _entitiesInContainerRetriever.QuantitiesFrom(_simulation)).Returns(_allQuantities);

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

   public class When_mapping_a_output_selection_snapshot_to_output_selection : concern_for_OutputSelectionsMapper
   {
      private OutputSelections _newOutputSelections;
      private IQuantity _parameter1;
      private IQuantity _quantity1;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_outputSelections);
         _parameter1 = new Parameter();
         _quantity1 = A.Fake<IQuantity>();
         _quantity1.QuantityType = QuantityType.Protein;

         _allQuantities.Add("PATH1", _parameter1);
         _allQuantities.Add("PATH2", _quantity1);
      }

      protected override async Task Because()
      {
         _newOutputSelections = await sut.MapToModel(_snapshot,_simulation);
      }

      [Observation]
      public void should_retrieve_the_quantity_with_the_given_path_and_create_an_output_selection_with_the_expected_quantity_type()
      {
         _newOutputSelections.AllOutputs.Count().ShouldBeEqualTo(2);
         _newOutputSelections.AllOutputs.ElementAt(0).Path.ShouldBeEqualTo("PATH1");
         _newOutputSelections.AllOutputs.ElementAt(0).QuantityType.ShouldBeEqualTo(_parameter1.QuantityType);
         _newOutputSelections.AllOutputs.ElementAt(1).Path.ShouldBeEqualTo("PATH2");
         _newOutputSelections.AllOutputs.ElementAt(1).QuantityType.ShouldBeEqualTo(_quantity1.QuantityType);
      }
   }

   public class When_mapping_an_output_selection_containing_an_output_that_does_not_exist_anymore : concern_for_OutputSelectionsMapper
   {
      private IQuantity _parameter1;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_outputSelections);
         _parameter1 = new Parameter();

         _allQuantities.Add("PATH2", _parameter1);
      }

      protected override Task Because()
      {
         return sut.MapToModel(_snapshot, _simulation);
      }

      [Observation]
      public void should_warn_that_a_missing_output_was_not_found()
      {
         A.CallTo(() => _logger.AddToLog(A<string>.That.Contains("PATH1"), NotificationType.Warning)).MustHaveHappened();
      }
   }
}
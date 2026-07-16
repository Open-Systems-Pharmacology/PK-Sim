using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.R.Mappers;
using CoreAgingData = PKSim.Core.Model.AgingData;
using RAgingData = OSPSuite.R.Domain.AgingData;

namespace PKSim.R
{
   public abstract class concern_for_AgingDataMapper : ContextSpecification<IAgingDataMapper>
   {
      protected override void Context()
      {
         sut = new AgingDataMapper();
      }
   }

   public class When_mapping_core_aging_data_to_the_r_aging_data : concern_for_AgingDataMapper
   {
      private CoreAgingData _agingData;
      private RAgingData _result;

      protected override void Context()
      {
         base.Context();
         _agingData = new CoreAgingData();
         _agingData.Add(individualIndex: 0, parameterPath: "Organism|Liver|Volume", time: 10, value: 100);
         _agingData.Add(individualIndex: 0, parameterPath: "Organism|Liver|Volume", time: 20, value: 110);
         _agingData.Add(individualIndex: 5, parameterPath: "Organism|Liver|Volume", time: 10, value: 200);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_agingData);
      }

      [Observation]
      public void should_flatten_every_aging_row_into_the_parallel_arrays()
      {
         _result.IndividualIds.ShouldOnlyContainInOrder(0, 0, 5);
         _result.ParameterPaths.ShouldOnlyContainInOrder("Organism|Liver|Volume", "Organism|Liver|Volume", "Organism|Liver|Volume");
         _result.Times.ShouldOnlyContainInOrder(10.0, 20.0, 10.0);
         _result.Values.ShouldOnlyContainInOrder(100.0, 110.0, 200.0);
      }
   }

   public class When_mapping_core_aging_data_spanning_multiple_parameters : concern_for_AgingDataMapper
   {
      private CoreAgingData _agingData;
      private RAgingData _result;

      protected override void Context()
      {
         base.Context();
         _agingData = new CoreAgingData();
         _agingData.Add(individualIndex: 0, parameterPath: "PathA", time: 1, value: 10);
         _agingData.Add(individualIndex: 1, parameterPath: "PathB", time: 2, value: 20);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_agingData);
      }

      [Observation]
      public void should_include_the_rows_of_every_parameter()
      {
         _result.IndividualIds.Length.ShouldBeEqualTo(2);
         _result.ParameterPaths.ShouldContain("PathA", "PathB");
      }
   }
}

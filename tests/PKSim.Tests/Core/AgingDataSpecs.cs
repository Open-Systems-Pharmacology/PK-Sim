using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;


namespace PKSim.Core
{
   public abstract class concern_for_AgingData : ContextSpecification<AgingData>
   {
      protected override void Context()
      {
         sut = new AgingData();
      }
   }

   public class When_cloning_the_aging_data : concern_for_AgingData
   {
      private AgingData _clone;

      protected override void Context()
      {
         base.Context();
         sut.Add(1, "Path", 1, 10);
         sut.Add(2, "Path", 2, 20);
         sut.Add(1, "Path2", 3, 30);
         sut.Add(2, "Path2", 4, 40);
      }

      protected override void Because()
      {
         _clone = sut.Clone();
      }

      [Observation]
      public void should_return_aging_data_containing_the_values_definied_in_the_clone()
      {
         _clone.AllParameterData.Count().ShouldBeEqualTo(2);
         var parameterAgingDataPath = _clone.AllParameterData.First(x => x.ParameterPath == "Path");
         parameterAgingDataPath.Times.ShouldOnlyContain(1, 2);
         parameterAgingDataPath.Values.ShouldOnlyContain(10, 20);

         var parameterAgingDataPath2 = _clone.AllParameterData.First(x => x.ParameterPath == "Path2");
         parameterAgingDataPath2.Times.ShouldOnlyContain(3, 4);
         parameterAgingDataPath2.Values.ShouldOnlyContain(30, 40);
      }
   }
}
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_TablePart : ContextSpecification<TablePart>
   {
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected override void Context()
      {
         _representationInfoRepository= A.Fake<IRepresentationInfoRepository>();
         sut = new TablePart("Parameter");
      }
   }

   public class When_adding_a_paramter_to_a_table_part : concern_for_TablePart
   {
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName("PARAM");
         A.CallTo(() => _representationInfoRepository.DisplayNameFor(_parameter)).Returns(_parameter.Name);
      }
      protected override void Because()
      {
         sut.AddIs(_parameter,_representationInfoRepository);
      }

      [Observation]
      public void should_add_the_value_as_numbers_only()
      {
         sut.Rows.Find(x=>x.Key == _parameter.Name).Value[0].ShouldBeEqualTo(PKSim.Core.Model.ParameterMessages.DisplayValueFor(_parameter,true));
      }

      [Observation]
      public void should_add_the_display_unit_as_separate_entry()
      {
         sut.Rows.Find(x => x.Key == _parameter.Name).Value[1].ShouldBeEqualTo(PKSim.Core.Model.ParameterMessages.DisplayUnitFor(_parameter));
      }
   }

}	
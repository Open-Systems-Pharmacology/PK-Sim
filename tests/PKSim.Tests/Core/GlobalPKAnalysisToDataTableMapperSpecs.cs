using System.Data;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_GlobalPKAnalysisToDataTableMapper : ContextSpecification<IGlobalPKAnalysisToDataTableMapper>
   {
      private IRepresentationInfoRepository _representationInfoRepository;
      protected GlobalPKAnalysis _globalPKAnalysis;
      private IContainer _compound1;
      private IContainer _compound2;

      protected override void Context()
      {
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         sut = new GlobalPKAnalysisToDataTableMapper(_representationInfoRepository);

         _globalPKAnalysis = new GlobalPKAnalysis();
         _compound1 = new Container().WithName("Drug");
         _compound2 = new Container().WithName("Inhibitor");
         _globalPKAnalysis.Add(_compound1);
         _globalPKAnalysis.Add(_compound2);

         _compound1.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P1"));
         _compound1.Add(DomainHelperForSpecs.ConstantParameterWithValue(2).WithName("P2"));
         _compound2.Add(DomainHelperForSpecs.ConstantParameterWithValue(3).WithName("P1"));
         _compound2.Add(DomainHelperForSpecs.ConstantParameterWithValue(4).WithName("P3"));

         A.CallTo(() => _representationInfoRepository.InfoFor(A<IParameter>._))
            .ReturnsLazily(x => new RepresentationInfo {DisplayName = "Display for " + x.GetArgument<IParameter>(0).Name});
      }
   }

   public class When_mapping_a_global_pk_analysis_to_data_table : concern_for_GlobalPKAnalysisToDataTableMapper
   {
      private DataTable _dataTable;

      protected override void Because()
      {
         _dataTable = sut.MapFrom(_globalPKAnalysis);
      }

      [Observation]
      public void should_add_a_row_for_each_parameter_defined_in_the_global_pk_analysis_per_compound()
      {
         _dataTable.AllValuesInColumn<string>(PKSimConstants.PKAnalysis.Compound).ShouldOnlyContainInOrder("Drug", "Inhibitor", "Drug", "Inhibitor");
         _dataTable.AllValuesInColumn<string>(PKSimConstants.PKAnalysis.ParameterDisplayName).ShouldOnlyContainInOrder("Display for P1", "Display for P1", "Display for P2", "Display for P3");
         _dataTable.AllValuesInColumn<double>(PKSimConstants.PKAnalysis.Value).ShouldOnlyContainInOrder(1, 3, 2, 4);
      }
   }

   public class When_mapping_a_global_pk_analysis_to_data_table_and_adding_the_parameter_name : concern_for_GlobalPKAnalysisToDataTableMapper
   {
      private DataTable _dataTable;

      protected override void Because()
      {
         _dataTable = sut.MapFrom(_globalPKAnalysis, addMetaData: true);
      }

      [Observation]
      public void should_add_a_column_containing_the_parameter_name()
      {
         _dataTable.AllValuesInColumn<string>(PKSimConstants.PKAnalysis.ParameterName).ShouldOnlyContainInOrder("P1", "P1", "P2", "P3");
      }
   }
}
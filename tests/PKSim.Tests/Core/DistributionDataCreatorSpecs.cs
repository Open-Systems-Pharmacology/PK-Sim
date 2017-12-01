using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using DistributionSettings = PKSim.Core.Chart.DistributionSettings;

namespace PKSim.Core
{
   public abstract class concern_for_DistributionDataCreator : ContextSpecification<DistributionDataCreator>
   {
      protected ICoreUserSettings _userSettings;
      private IRepresentationInfoRepository _representationInfoRep;
      protected IEntityPathResolver _entityPathResolver;
      private IBinIntervalsCreator _binIntervalCreator;

      protected override void Context()
      {
         _userSettings = A.Fake<ICoreUserSettings>();
         _userSettings.NumberOfBins = 2;
         _userSettings.NumberOfIndividualsPerBin = 100;
         _representationInfoRep = A.Fake<IRepresentationInfoRepository>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _binIntervalCreator =new BinIntervalsCreator(_userSettings);
         sut = new DistributionDataCreator(_binIntervalCreator, _representationInfoRep, _entityPathResolver, _userSettings);
      }
   }

   public class When_retriving_the_distibution_data_for_a_vectorial_parameter_containers : concern_for_DistributionDataCreator
   {
      private IParameter _parameter;
      private IVectorialParametersContainer _vectorialParameterContainers;
      private ContinuousDistributionData _data;

      protected override void Context()
      {
         base.Context();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(5);
         A.CallTo(() => _entityPathResolver.PathFor(_parameter)).Returns("Path");
         _vectorialParameterContainers = A.Fake<IVectorialParametersContainer>();
         A.CallTo(() => _vectorialParameterContainers.AllValuesFor("Path")).Returns(new[] {0.77, 0.99, double.NaN});
         var gender = new Gender {Id = CoreConstants.Gender.Male, Name = CoreConstants.Gender.Male};
         A.CallTo(() => _vectorialParameterContainers.AllGenders).Returns(new[] {gender, gender, gender});
      }

      protected override void Because()
      {
         _data = sut.CreateFor(_vectorialParameterContainers, new DistributionSettings {AxisCountMode = AxisCountMode.Count, SelectedGender = CoreConstants.Gender.Male}, _parameter,_parameter.Dimension, _parameter.DisplayUnit);
      }

      [Observation]
      public void should_return_a_distribution_table_containing_one_entry_for_each_predefined_interval()
      {
         _data.DataTable.Rows.Count.ShouldBeEqualTo(_userSettings.NumberOfBins.ConvertedTo<int>());
      }

      [Observation]
      public void should_have_created_the_right_mean_values()
      {
         _data.DataTable.AllValuesInColumn<double>(_data.XAxisName).ShouldOnlyContain(0.825, 0.935);
      }

      [Observation]
      public void should_have_set_the_bar_width_equal_to_the_max_minus_min_divided_by_the_number_of_bins()
      {
         _data.BarWidth.ShouldBeEqualTo((0.99 - 0.77) / _userSettings.NumberOfBins);  
      }
   }

   public class When_retriving_the_distibution_data_for_a_skalar_parameter_containers : concern_for_DistributionDataCreator
   {
      private IParameter _parameter;
      private IVectorialParametersContainer _vectorialParameterContainers;
      private ContinuousDistributionData _data;

      protected override void Context()
      {
         base.Context();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(5);
         A.CallTo(() => _entityPathResolver.PathFor(_parameter)).Returns("Path");
         _vectorialParameterContainers = A.Fake<IVectorialParametersContainer>();
         A.CallTo(() => _vectorialParameterContainers.AllValuesFor("Path")).Returns(new[] { 0.5, 0.5, 0.5});
         var gender = new Gender { Id = CoreConstants.Gender.Male, Name = CoreConstants.Gender.Male };
         A.CallTo(() => _vectorialParameterContainers.AllGenders).Returns(new[] { gender, gender, gender });
      }

      protected override void Because()
      {
         _data = sut.CreateFor(_vectorialParameterContainers, new DistributionSettings { AxisCountMode = AxisCountMode.Count, SelectedGender = CoreConstants.Gender.Male }, _parameter,_parameter.Dimension, _parameter.DisplayUnit);
      }

      [Observation]
      public void should_return_a_distribution_table_containing_one_entry()
      {
         _data.DataTable.Rows.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_created_the_right_mean_values()
      {
         _data.DataTable.AllValuesInColumn<double>(_data.XAxisName).ShouldOnlyContain(0.5);
      }

      [Observation]
      public void should_create_a_bar_width_greater_than_zero()
      {
         _data.BarWidth.ShouldBeGreaterThan(0);
      }
   }


 
}
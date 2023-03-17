using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart.Simulations;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation;
using OSPSuite.Utility.Container;
using PKSim.Core.Chart;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Charts;

namespace PKSim.Presentation
{
   public abstract class concern_for_PKSimChartFactory : ContextSpecification<IPKSimChartFactory>
   {
      private IContainer _container;
      private IIdGenerator _idGenerator;
      private IPresentationUserSettings _presentationUserSettings;
      private IDimensionFactory _dimensionFactory;
      private ITableFormulaToDataRepositoryMapper _dataRepositoryMapper;
      private IChartTask _chartTask;

      protected override void Context()
      {
         _container = A.Fake<IContainer>();
         _idGenerator = A.Fake<IIdGenerator>();
         _presentationUserSettings = A.Fake<IPresentationUserSettings>();
         _dimensionFactory = A.Fake<IDimensionFactory>();
         _dataRepositoryMapper = A.Fake<ITableFormulaToDataRepositoryMapper>();
         _chartTask = A.Fake<IChartTask>();


         sut = new PKSimChartFactory(_container, _idGenerator, _presentationUserSettings, _dimensionFactory, _dataRepositoryMapper, _chartTask);
      }
   }

   public class When_creating_a_chart_by_type : concern_for_PKSimChartFactory
   {
      [Observation]
      public void should_be_able_to_create_for_time_profile_chart()
      {
         sut.Create(typeof(SimulationTimeProfileChart)).ShouldNotBeNull();
      }

      [Observation]
      public void should_be_able_to_create_for_simulation_predicted_vs_observed()
      {
         sut.Create(typeof(SimulationPredictedVsObservedChart)).ShouldNotBeNull();
      }

      [Observation]
      public void should_be_able_to_create_for_simulation_residual_vs_time()
      {
         sut.Create(typeof(SimulationResidualVsTimeChart)).ShouldNotBeNull();
      }

      [Observation]
      public void should_be_able_to_create_for_individual_simulation_comparison()
      {
         sut.Create(typeof(IndividualSimulationComparison)).ShouldNotBeNull();
      }
   }
}
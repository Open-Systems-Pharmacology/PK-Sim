using System;
using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.Utility.Format;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.Services;
using DataColumn = OSPSuite.Core.Domain.Data.DataColumn;

namespace PKSim.Presentation
{
   public abstract class concern_for_PKAnalysisPresenter : ContextSpecification<IIndividualPKAnalysisPresenter>
   {
      protected IIndividualPKAnalysisView _view;
      private IPKAnalysesTask _pkaAnalysesTask;
      protected IndividualSimulation _simulation;
      protected IList<Tuple<DataColumn, PKAnalysis>> _allPKAnalysis;
      protected PKAnalysis _pkAnalysis1;
      protected PKAnalysis _pkAnalysis2;
      private DataColumn _col1;
      private DataColumn _col2;
      private IDimension _concentrationDim;
      protected IFormatter<double> _doubleFormatter;
      protected IEnumerable<IParameter> _allGlobalPKParameters;
      private IPKAnalysisExportTask _exportTask;
      protected List<Simulation> _simulations;
      protected List<Curve> _curves;
      private DataColumn _col3;
      private IDimension _timeDim;
      private BaseGrid _baseGrid;
      protected IGlobalPKAnalysisPresenter _globalPKAnalysisPresenter;
      protected IIndividualPKAnalysisToPKAnalysisDTOMapper _individualPKAnalysisToDTOMapper;
      private IPKParameterRepository _pkParameterRepository;
      private IPresentationSettingsTask _presenterSettingsTask;

      protected override void Context()
      {
         _globalPKAnalysisPresenter= A.Fake<IGlobalPKAnalysisPresenter>();
         _concentrationDim = A.Fake<IDimension>();
         _timeDim = A.Fake<IDimension>();
         _view = A.Fake<IIndividualPKAnalysisView>();
         _pkaAnalysesTask = A.Fake<IPKAnalysesTask>();
         _simulation = A.Fake<IndividualSimulation>();
         _simulations = new List<Simulation>();
         _simulations.Add(_simulation);
         _exportTask = A.Fake<IPKAnalysisExportTask>();
         _individualPKAnalysisToDTOMapper = A.Fake<IIndividualPKAnalysisToPKAnalysisDTOMapper>();
         _pkParameterRepository= A.Fake<IPKParameterRepository>();
         ;
         var dataRepository = new DataRepository();

         _baseGrid = new BaseGrid("Base", _timeDim);
         _col1 = new DataColumn {DataInfo = new DataInfo(ColumnOrigins.Calculation, AuxiliaryType.Undefined, "mg", DateTime.Now, string.Empty, string.Empty, 5), Dimension = _concentrationDim};
         _col2 = new DataColumn {Dimension = _concentrationDim, DataInfo = new DataInfo(ColumnOrigins.Calculation, AuxiliaryType.Undefined, "mg", DateTime.Now, string.Empty, string.Empty, 5)};
         _col3 = new DataColumn {Dimension = _concentrationDim, DataInfo = new DataInfo(ColumnOrigins.Calculation, AuxiliaryType.Undefined, "mg", DateTime.Now, string.Empty, string.Empty, 5)};
         dataRepository.Add(_col1);
         dataRepository.Add(_col2);
         _simulation.DataRepository = dataRepository;
         var curve1 = A.Fake<Curve>();
         A.CallTo(() => curve1.yData).Returns(_col1);
         A.CallTo(() => curve1.xData).Returns(_baseGrid);

         var curve2 = A.Fake<Curve>();
         A.CallTo(() => curve2.yData).Returns(_col2);
         A.CallTo(() => curve2.xData).Returns(_baseGrid);

         var curve3 = A.Fake<Curve>();
         A.CallTo(() => curve3.yData).Returns(_col3);
         A.CallTo(() => curve3.xData).Returns(_col1);

         _curves = new List<Curve> {curve1, curve2, curve3};
         _allPKAnalysis = new List<Tuple<DataColumn, PKAnalysis>>();
         _pkAnalysis1 = createPKAnalysis();
         _pkAnalysis2 = createPKAnalysis();
         _allPKAnalysis.Add(new Tuple<DataColumn, PKAnalysis>(_col1, _pkAnalysis1));
         _allPKAnalysis.Add(new Tuple<DataColumn, PKAnalysis>(_col2, _pkAnalysis2));
         _allGlobalPKParameters = new List<IParameter>();
         A.CallTo(_pkaAnalysesTask).WithReturnType<IEnumerable<Tuple<DataColumn, PKAnalysis>>>().Returns(_allPKAnalysis);
         A.CallTo(_pkaAnalysesTask).WithReturnType<IEnumerable<IParameter>>().Returns(_allGlobalPKParameters);

         _presenterSettingsTask = A.Fake<IPresentationSettingsTask>();
         sut = new IndividualPKAnalysisPresenter(_view, _pkaAnalysesTask, _exportTask, _globalPKAnalysisPresenter,
            _individualPKAnalysisToDTOMapper, _pkParameterRepository, _presenterSettingsTask);
      }

      private PKAnalysis createPKAnalysis()
      {
         return PKAnalysisHelperForSpecs.GenerateIndividualPKAnalysis();
      }
   }

   public class when_the_pk_analysis_has_global_pk_parameters : concern_for_PKAnalysisPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _globalPKAnalysisPresenter.HasParameters()).Returns(true);
      }

      protected override void Because()
      {
         sut.ShowPKAnalysis(_simulations, _curves);
      }

      [Observation]
      public void the_pk_analysis_presenter_should_show_the_global_pk_parameters_view()
      {
         A.CallTo(_view).Where(x => x.Method.Name.Equals("set_GlobalPKVisible")).WhenArgumentsMatch(x => x.Get<bool>(0)).MustHaveHappened();
      }
   }

   public class when_the_pk_analysis_does_not_have_any_global_pk_parameters : concern_for_PKAnalysisPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _globalPKAnalysisPresenter.HasParameters()).Returns(false);
      }

      protected override void Because()
      {
         sut.ShowPKAnalysis(_simulations, _curves);
      }

      [Observation]
      public void the_pk_analysis_presenter_should_hide_the_global_pk_parameters_view()
      {
         A.CallTo(_view).Where(x => x.Method.Name.Equals("set_GlobalPKVisible")).WhenArgumentsMatch(x => x.Get<bool>(0) == false).MustHaveHappened();
      }
   }

   public class When_the_pk_analysis_presenter_is_told_to_display_the_pk_analysis_for_a_given_simulation_and_columns : concern_for_PKAnalysisPresenter
   {
      protected override void Because()
      {
         sut.ShowPKAnalysis(_simulations, _curves);
      }

      [Observation]
      public void should_display_the_global_pk_values_in_the_view()
      {
         A.CallTo(() => _globalPKAnalysisPresenter.CalculatePKAnalysis(A<IReadOnlyList<Simulation>>._)).MustHaveHappened();
      }
   }


}
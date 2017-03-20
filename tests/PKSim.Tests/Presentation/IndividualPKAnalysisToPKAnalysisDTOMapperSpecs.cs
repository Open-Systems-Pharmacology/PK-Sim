using System.Collections.Generic;
using System.Data;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using OSPSuite.Core.Chart;
using DataColumn = OSPSuite.Core.Domain.Data.DataColumn;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualPKAnalysisToPKAnalysisDTOMapper : ContextSpecification<IndividualPKAnalysisToPKAnalysisDTOMapper>
   {
      protected IIndividualPKAnalysisToDataTableMapper _individualPKAnalysisToDataTableMapper;
      protected IList<ICurve> _curves;
      private ICurve _curve;
      protected DataColumn _dataColumn;

      protected override void Context()
      {
         _individualPKAnalysisToDataTableMapper = A.Fake<IIndividualPKAnalysisToDataTableMapper>();
         sut = new IndividualPKAnalysisToPKAnalysisDTOMapper(_individualPKAnalysisToDataTableMapper);

         _curve = A.Fake<ICurve>();
         _curves = new List<ICurve> {_curve};
      }
   }

   public class when_mapping_dto_from_individualpkanalysis : concern_for_IndividualPKAnalysisToPKAnalysisDTOMapper
   {
      private PKAnalysisDTO _result;
      private PKAnalysis _pKAnalysis;
      private DataTable _dataTable;
      private List<IndividualPKAnalysis> _individualPKAnalyses;

      protected override void Context()
      {
         base.Context();
         _pKAnalysis = PKAnalysisHelperForSpecs.GenerateIndividualPKAnalysis();
         _dataTable = new DataTable();
         _individualPKAnalyses = new List<IndividualPKAnalysis> {new IndividualPKAnalysis(_dataColumn, _pKAnalysis)};
         A.CallTo(() => _individualPKAnalysisToDataTableMapper.MapFrom(_individualPKAnalyses, _curves, true)).Returns(_dataTable);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_individualPKAnalyses, _curves);
      }

      [Observation]
      public void resulting_table_must_have_a_row_per_pkparameter_being_calculated()
      {
         _result.DataTable.ShouldBeEqualTo(_dataTable);
      }
   }
}
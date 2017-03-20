using System;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;
using FakeItEasy;
using PKSim.Infrastructure.Reporting.Summary;

namespace PKSim.Core
{
   public abstract class concern_for_ReportGenerator : ContextSpecification<IReportGenerator>
   {
      protected IReportBuilderRepository _reportBuilderRepository;
      protected IRepresentationInfoRepository _representationInfoRepository;

      protected override void Context()
      {
         _reportBuilderRepository =A.Fake<IReportBuilderRepository>();
         _representationInfoRepository =A.Fake<IRepresentationInfoRepository>();
         sut = new ReportGenerator(_reportBuilderRepository,_representationInfoRepository);
      }
   }

   
   public class When_generating_a_report_for_an_object : concern_for_ReportGenerator
   {
      private IEntity _entity;
      private IReportBuilder _reportBuilder;
      private ReportPart _report;

      protected override void Context()
      {
         base.Context();
         _report =new ReportPart();
         _entity = new PKSimParameter();
         _reportBuilder = A.Fake<IReportBuilder<IEntity>>();
         A.CallTo(() => _reportBuilderRepository.BuilderFor(_entity)).Returns(_reportBuilder);
         A.CallTo(() => _reportBuilder.Report(_entity)).Returns(_report);
      }

      [Observation]
      public void should_return_the_report_document_build_for_the_given_object()
      {
         sut.ReportFor(_entity).ShouldBeEqualTo(_report);
      }
   }

   
   public class When_generating_a_report_for_an_object_for_which_no_report_builder_was_found : concern_for_ReportGenerator
   {
      private IEntity _entity;
      private string _description;

      protected override void Context()
      {
         base.Context();
         _description = "tralala";
         _entity = new PKSimParameter();
         A.CallTo(() => _representationInfoRepository.DescriptionFor(_entity)).Returns(_description);
         A.CallTo(() => _reportBuilderRepository.BuilderFor(_entity)).Throws(new Exception());
      }

      [Observation]
      public void should_return_a_report_part_containing_the_description_for_the_object_if_available()
      {
         sut.ReportFor(_entity).Content.Contains(_description).ShouldBeTrue();
      }
   }
}	
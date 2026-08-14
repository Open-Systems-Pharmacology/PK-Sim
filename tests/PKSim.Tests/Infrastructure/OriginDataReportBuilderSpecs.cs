using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Utility.Container;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Repositories;
using PKSim.Infrastructure.Reporting.Summary;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_OriginDataReportBuilder : ContextSpecification<OriginDataReportBuilder>
   {
      protected OriginData _originData;

      protected override void Context()
      {
         var serializerRepository = new UnitSystemXmlSerializerRepository();
         serializerRepository.PerformMapping();
         var realDimensions = new DimensionRepository(new PKSimDimensionFactory(), serializerRepository, new PKSimConfiguration(), A.Fake<IContainer>());

         var dimensionRepository = A.Fake<IDimensionRepository>();
         A.CallTo(() => dimensionRepository.AgeInWeeks).Returns(realDimensions.AgeInWeeks);
         A.CallTo(() => dimensionRepository.AgeInYears).Returns(realDimensions.AgeInYears);
         A.CallTo(() => dimensionRepository.Mass).Returns(realDimensions.Mass);
         A.CallTo(() => dimensionRepository.Length).Returns(realDimensions.Length);
         A.CallTo(() => dimensionRepository.BMI).Returns(realDimensions.BMI);

         //"week(s)" is defined in "Age in weeks", "Age in years" and "Time", so a lookup by unit
         //name alone is ambiguous and can return any of them. Pin it to the wrong one: the report
         //must not depend on it for parameters whose dimension is known statically.
         A.CallTo(() => dimensionRepository.DimensionForUnit(A<string>._)).Returns(realDimensions.AgeInYears);

         sut = new OriginDataReportBuilder(
            A.Fake<IReportGenerator>(),
            dimensionRepository,
            A.Fake<IRepresentationInfoRepository>(),
            A.Fake<IParameterListOfValuesRetriever>());

         _originData = new OriginData
         {
            Species = new Species {Name = "Human", DisplayName = "Human"},
            Population = new SpeciesPopulation
            {
               Name = CoreConstants.Population.PRETERM,
               DisplayName = "Preterm",
               IsAgeDependent = true,
               IsHeightDependent = true
            },
            Gender = new Gender {Name = "MALE", DisplayName = "Male"},
            //values are stored in the base unit of their own dimension
            Age = new OriginDataParameter(0.1, "year(s)"),
            GestationalAge = new OriginDataParameter(30, "week(s)"),
            Weight = new OriginDataParameter(1.5, "kg"),
            Height = new OriginDataParameter(4.454, "dm"),
            BMI = new OriginDataParameter(0.0756, "kg/m²")
         };
      }

      protected IReadOnlyList<string> valuesFor(string key)
      {
         var individualProperties = sut.Report(_originData).SubParts.OfType<TablePart>()
            .First(x => x.Title == PKSimConstants.UI.IndividualParameters);

         return individualProperties.Rows.First(x => x.Key == key).Value;
      }
   }

   public class When_reporting_the_origin_data_of_a_preterm_individual : concern_for_OriginDataReportBuilder
   {
      [Observation]
      public void should_format_gestational_age_using_the_age_in_weeks_dimension()
      {
         //formatting 30 through "Age in years" would report 30 * 52.1786 = 1565.36 week(s)
         valuesFor(PKSimConstants.UI.GestationalAge).ShouldOnlyContainInOrder("30.00", "week(s)");
      }

      [Observation]
      public void should_format_the_remaining_origin_data_parameters_with_their_own_dimension()
      {
         valuesFor(PKSimConstants.UI.Age).ShouldOnlyContainInOrder("0.10", "year(s)");
         valuesFor(PKSimConstants.UI.Weight).ShouldOnlyContainInOrder("1.50", "kg");
         valuesFor(PKSimConstants.UI.Height).ShouldOnlyContainInOrder("4.45", "dm");
         valuesFor(PKSimConstants.UI.BMI).ShouldOnlyContainInOrder("7.56", "kg/m²");
      }
   }
}

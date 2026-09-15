using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Snapshots;
using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Infrastructure;
using OriginData = PKSim.Core.Model.OriginData;

namespace PKSim.IntegrationTests
{
   public class When_mapping_the_origin_data_of_an_individual_to_snapshot_and_back : ContextForIntegration<OriginDataMapper>
   {
      private Individual _individual;
      private OriginData _mappedOriginData;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
      }

      protected override void Because()
      {
         var snapshot = sut.MapToSnapshot(_individual.OriginData).Result;
         _mappedOriginData = sut.MapToModel(snapshot, new SnapshotContext(new PKSimProject(), SnapshotVersions.Current)).Result;
      }

      [Observation]
      public void should_recompute_the_bmi_from_the_mapped_weight_and_height()
      {
         var expectedBMI = _mappedOriginData.Weight.Value / Math.Pow(_mappedOriginData.Height.Value, 2);
         _mappedOriginData.BMI.Value.ShouldBeEqualTo(expectedBMI, 1e-6);
      }

      [Observation]
      public void should_use_the_default_display_unit_of_the_bmi_parameter()
      {
         _mappedOriginData.BMI.Unit.ShouldBeEqualTo(_individual.Organism.Parameter(CoreConstants.Parameters.BMI).DisplayUnit.Name);
      }
   }
}
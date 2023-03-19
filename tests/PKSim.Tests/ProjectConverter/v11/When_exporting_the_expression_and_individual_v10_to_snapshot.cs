using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v11
{
   public class When_exporting_the_expression_and_individual_v10_to_snapshot : ContextWithLoadedProject<ProjectMapper>
   {
      private ProjectMapper _projectMapper;
      private Project _snapshot;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("expression_and_individual_V10");

         //do not load any building block
         _projectMapper = IoC.Resolve<ProjectMapper>();
      }

      protected override void Because()
      {
         _snapshot = _projectMapper.MapToSnapshot(_project).Result;
      }

      [Observation]
      public void should_have_exported_the_expression_for_the_individual()
      {
         _snapshot.ExpressionProfiles.ShouldNotBeNull();
         _snapshot.ExpressionProfiles.Length.ShouldBeGreaterThan(0);
      }
   }
}
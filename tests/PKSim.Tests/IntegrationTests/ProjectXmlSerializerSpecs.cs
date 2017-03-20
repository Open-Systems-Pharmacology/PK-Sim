using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.IntegrationTests
{
   public class When_serialization_a_project : ContextForSerialization<PKSimProject>
   {
      private PKSimProject _project;
      private PKSimProject _deserializedProject;

      protected override void Context()
      {
         base.Context();
         _project = new PKSimProject();
         _project.Favorites.Add("FAV1");
         _project.Favorites.Add("FAV2");
      }

      protected override void Because()
      {
         _deserializedProject = SerializeAndDeserialize(_project);
      }

      [Observation]
      public void should_have_deserialized_the_favorites()
      {
         _deserializedProject.Favorites.ShouldOnlyContain("FAV1", "FAV2");
      }

   }
}
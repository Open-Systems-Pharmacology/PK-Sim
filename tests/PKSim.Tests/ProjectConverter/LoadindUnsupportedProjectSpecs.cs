using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core;
using PKSim.Infrastructure.Serialization;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter
{
   public class When_loading_a_project_whose_version_is_not_supported_anymore : ContextWithLoadedProject<ProjectVersion>
   {
      [Observation]
      public void should_thrown_an_exception()
      {
         The.Action(() => LoadProject("oldProject")).ShouldThrowAn<InvalidProjectVersionException>();
      }
   }
}
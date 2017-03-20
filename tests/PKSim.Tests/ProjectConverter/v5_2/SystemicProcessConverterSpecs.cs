using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_2;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_2
{
   //Regressions test for http://byyncq.de.bayer.cnb:4711/issue/47-5637
   public class When_converting_the_Ex_PID_1_project : ContextWithLoadedProject<Converter514To521>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("Ex_PID_1");
      }


      [Observation]
      public void should_have_converted_the_systemic_processes()
      {
         var compound= First<Compound>();
         var liverProcess = compound.AllSystemicProcessesOfType(SystemicProcessTypes.Hepatic).First();
         //Should be able to calculate spec clearance
         liverProcess.Parameter(ConverterConstants.Parameter.SpecificClearance).Value.ShouldBeEqualTo(135,1e-2);

         //should have renamed lipo to lipo experiment 
         liverProcess.Parameter(CoreConstants.Parameter.LIPOPHILICITY_EXPERIMENT).Value.ShouldBeEqualTo(4.4);
         liverProcess.Parameter(ConverterConstants.Parameter.Lipophilicity).ShouldBeNull();
      }
   }
}	
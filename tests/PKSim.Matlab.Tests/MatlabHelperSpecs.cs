using System;
using System.IO;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using PKSim.Core.Snapshots;


namespace PKSim.Matlab
{
   [IntegrationTests]
   [Category("Matlab")]
   public abstract class concern_for_MatlabHelper : ContextSpecification<IMatlabHelper>
   {
      protected OriginData _matlabOriginData;

      public override void GlobalContext()
      {
         base.GlobalContext();
         ApplicationStartup.Initialize();
      }

      protected override void Context()
      {
         sut = new MatlabHelper();
      }
   }

   public class When_exporting_a_pkml_simulation_to_sim_model_xml : concern_for_MatlabHelper
   {
      private string _simulationFile;
      private string _simModelXmlFile;

      protected override void Context()
      {
         base.Context();
         _simulationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Data", "S1_concentrBased.pkml");
         _simModelXmlFile = FileHelper.GenerateTemporaryFileName();
      }

      protected override void Because()
      {
         sut.SaveToSimModelXmlFile(_simulationFile, _simModelXmlFile);
      }

      [Observation]
      public void should_have_created_a_file_containing_the_sim_model_representation()
      {
         FileHelper.FileExists(_simModelXmlFile).ShouldBeTrue();
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.DeleteFile(_simModelXmlFile);
      }
   }
}
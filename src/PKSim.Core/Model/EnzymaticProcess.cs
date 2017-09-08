using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class EnzymaticProcess : PartialProcess
   {
      /// <summary>
      ///    Metabolite information is free-form. It does not need to match an existing compound
      ///    It will be matched up during creation of a simulation
      /// </summary>
      public string MetaboliteName { set; get; }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceEnzymaticProcess = sourceObject as EnzymaticProcess;
         if (sourceEnzymaticProcess == null) return;
         MetaboliteName = sourceEnzymaticProcess.MetaboliteName;
      }

      public override string GetProcessClass()
      {
         return CoreConstants.ProcessClasses.ENZYMATIC;
      }
   }
   
   public class EnzymaticProcessWithSpecies : EnzymaticProcess, ISpeciesDependentCompoundProcess
   {
   }
}
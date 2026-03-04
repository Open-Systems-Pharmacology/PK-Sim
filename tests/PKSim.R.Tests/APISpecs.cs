using OSPSuite.BDDHelper;
using System;
using Castle.MicroKernel.ComponentActivator;
using NUnit.Framework;
using OSPSuite.BDDHelper.Extensions;
using PKSim.CLI.Core.RunOptions;

namespace PKSim.R
{
   [IntegrationTests]
   [Category("R")]
   public class APISpecs : StaticContextSpecification
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         Api.InitializeOnce();

         Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
      }

      private static Exception getExceptionFromPerforming(Action work)
      {
         try
         {
            work();
            return null;
         }
         catch (Exception ex)
         {
            return ex;
         }
      }

      protected static void ActionShouldNotThrowAn<TException>(Action workToPerform) where TException : Exception
      {
         Exception exceptionFromPerforming = getExceptionFromPerforming(workToPerform);
         exceptionFromPerforming.GetType().ShouldNotBeEqualTo(typeof(TException));
      }
   }

   public class When_resolving_tasks_after_initialization : APISpecs
   {
      [Observation]
      public void can_get_individual_factory()
      {
         Api.GetIndividualFactory().ShouldNotBeNull();
      }

      [Observation]
      public void can_get_population_factory()
      {
         Api.GetPopulationFactory().ShouldNotBeNull();
      }

      [Observation]
      public void can_resolve_snapshot_runner()
      {
         ActionShouldNotThrowAn<ComponentActivatorException>(() => Api.RunSnapshot(new SnapshotRunOptions()));
      }

      [Observation]
      public void can_resolve_export_runner()
      {
         ActionShouldNotThrowAn<ComponentActivatorException>(() => Api.RunExport(new ExportRunOptions()));
      }

      [Observation]
      public void can_resolve_json_runner()
      {
         ActionShouldNotThrowAn<ComponentActivatorException>(() => Api.RunJson(new JsonRunOptions()));
      }

      [Observation]
      public void can_resolve_qualification_runner()
      {
         ActionShouldNotThrowAn<ComponentActivatorException>(() => Api.RunQualification(new QualificationRunOptions()));
      }

      [Observation]
      public void can_resolve_simulation_export_runner()
      {
         ActionShouldNotThrowAn<ComponentActivatorException>(() => Api.RunSimulationExport(new ExportRunOptions()));
      }
   }
}

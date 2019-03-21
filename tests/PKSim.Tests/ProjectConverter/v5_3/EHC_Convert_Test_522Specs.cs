using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_3;
using PKSim.IntegrationTests;
using IContainer = OSPSuite.Core.Domain.IContainer;

namespace PKSim.ProjectConverter.v5_3
{
   public class When_converting_the_EHC_Convert_Test_522 : ContextWithLoadedProject<Converter52To531>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("EHC_Convert_Test_522");
         _project.All<Simulation>().Each(_lazyLoadTask.Load);
      }

      [Observation]
      public void should_have_renamed_the_parameter_gall_bladder_emptying_rate_in_the_organism()
      {
         foreach (var rootContainer in _project.All<Simulation>().Select(x => x.Model.Root))
         {
            var gallBladder = rootContainer.EntityAt<IContainer>(Constants.ORGANISM, CoreConstants.Organ.Gallbladder);
            if (gallBladder == null)
               continue;

            gallBladder.Parameter(ConverterConstants.Parameter.Gallbladder_emptying_rate).ShouldBeNull();
            gallBladder.Parameter(ConverterConstants.Parameter.Gallbladder_emptying_active).ShouldNotBeNull();
         }
      }

      [Observation]
      public void should_have_changed_the_gall_bladder_transport_rate_to_use_the_new_formula()
      {
         //Transport Nbhoods|Gbl_Lumen_duodenum|COMPOUND|GallbladderEmptying anpassen (neue formel)
         foreach (var simulation in _project.All<Simulation>())
         {
            var model = simulation.Model;
            var organism = model.Root.Container(Constants.ORGANISM);
            var gallBladder = organism.Container(CoreConstants.Organ.Gallbladder);
            if (gallBladder == null)
               continue;

            var neighborhood = model.Neighborhoods.GetSingleChildByName<IContainer>(ConverterConstants.Neighborhoods.GallbladderLumenDuo);
            var drugNeighborhood = neighborhood.Container(simulation.CompoundNames.First());
            var transport = drugNeighborhood.GetSingleChildByName<ITransport>(ConverterConstants.Events.GallbladderEmptying);
            var explicitFormula = transport.Formula.DowncastTo<ExplicitFormula>();
            explicitFormula.FormulaString.ShouldBeEqualTo("EHC_Active ? ln(2) / EHC_Halftime * M * EHC_EjectionFraction : 0");
            explicitFormula.ObjectPaths.Select(x => x.Alias).ShouldOnlyContain("M", "EHC_EjectionFraction", "EHC_Halftime", "EHC_Active");
         }
      }

      [Observation]
      public void should_have_changed_the_assignment_value_to_a_constant_1_formula_in_all_EHC_start_events()
      {
         //Transport Nbhoods|Gbl_Lumen_duodenum|COMPOUND|GallbladderEmptying anpassen (neue formel)
         foreach (var ehcStartEvent in _project.All<Simulation>().SelectMany(x => x.Model.Root.GetAllChildren<IEvent>(e => e.IsNamed(ConverterConstants.Events.EHCStartEvent))))
         {
            foreach (var assignment in ehcStartEvent.Assignments)
            {
               if (assignment.ObjectPath.Contains(ConverterConstants.Parameter.Gallbladder_emptying_rate))
               {
                  var explicitFormula = assignment.Formula.DowncastTo<ExplicitFormula>();
                  explicitFormula.FormulaString.ShouldBeEqualTo("1");
                  explicitFormula.ObjectPaths.Any().ShouldBeFalse();
               }
            }
         }
      }

      [Observation]
      public void should_be_able_to_export_the_simulation_to_pkml()
      {
         var simulation = FindByName<Simulation>("02_EHC");
         var tmpFile = FileHelper.GenerateTemporaryFileName();
         try
         {
            var exportTask = IoC.Resolve<IMoBiExportTask>();
            exportTask.SaveSimulationToFile(simulation, tmpFile);

            var simulationTransferLoader = IoC.Resolve<ICoreLoader>();
            var simulationTransfer = simulationTransferLoader.LoadSimulationTransfer(tmpFile);
            simulationTransfer.Simulation.ShouldNotBeNull();
         }
         finally
         {
            FileHelper.DeleteFile(tmpFile);
         }
      }
   }
}
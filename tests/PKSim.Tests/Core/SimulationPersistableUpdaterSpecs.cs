using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using NUnit.Framework;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationPersistableUpdater : ContextSpecification<ISimulationPersistableUpdater>
   {
      private IEntitiesInContainerRetriever _containerTask;

      protected override void Context()
      {
         _containerTask = A.Fake<IEntitiesInContainerRetriever>();
         sut = new SimulationPersistableUpdater(_containerTask);
      }
   }

   public class When_updating_the_simulation_settings_for_a_simulation : concern_for_SimulationPersistableUpdater
   {
      private IndividualSimulation _individualSimulation;
      private Observer _venousBloodPlasma;
      private Observer _fabsObserver;
      private Observer _peripheralVenousBloodObserver;

      protected override void Context()
      {
         base.Context();
         var organsim = new Container().WithName(Constants.ORGANISM);
         var venousBlood = new Container().WithName(CoreConstants.Organ.VENOUS_BLOOD).WithParentContainer(organsim);
         var peripheralVenousBlood = new Container().WithName(CoreConstants.Organ.PERIPHERAL_VENOUS_BLOOD).WithParentContainer(organsim);
         var lumen = new Container().WithName(CoreConstants.Organ.LUMEN).WithParentContainer(organsim);
         var plasma = new Container().WithName(CoreConstants.Compartment.PLASMA);
         var moleculeVenousBlood = new Container().WithName("DRUG")
            .WithParentContainer(plasma.WithParentContainer(venousBlood));

         var moleculePeripheral = new Container().WithName("DRUG")
            .WithParentContainer(peripheralVenousBlood);

         _peripheralVenousBloodObserver = new Observer().WithName(CoreConstants.Observer.PLASMA_PERIPHERAL_VENOUS_BLOOD);
         moleculePeripheral.Add(_peripheralVenousBloodObserver);

         _venousBloodPlasma = new Observer().WithName(CoreConstants.Observer.CONCENTRATION_IN_CONTAINER);
         moleculeVenousBlood.Add(_venousBloodPlasma);

         var moleculeLumen = new Container().WithName("DRUG")
            .WithParentContainer(lumen);

         _fabsObserver = new Observer().WithName(CoreConstants.Observer.FABS_ORAL);
         moleculeLumen.Add(_fabsObserver);

         var compound = new Compound().WithName("DRUG");

         _individualSimulation = new IndividualSimulation();
         _individualSimulation.SimulationSettings = new SimulationSettings();
         _individualSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("Id", PKSimBuildingBlockType.Compound) { BuildingBlock = compound });
         _individualSimulation.Model = new OSPSuite.Core.Domain.Model();
         _individualSimulation.Model.Root = new Container();
         _individualSimulation.Model.Root.Add(organsim);
      }

      protected override void Because()
      {
         sut.UpdatePersistableFromSettings(_individualSimulation);
      }

      [Observation]
      public void should_always_select_the_venous_blood_plasma_observer()
      {
         _venousBloodPlasma.Persistable.ShouldBeTrue();
      }

      [Observation]
      public void should_always_select_the_lumen_fabs_observer()
      {
         _fabsObserver.Persistable.ShouldBeTrue();
      }

      [Observation]
      public void should_always_select_the_peripheral_venous_blood_plasna()
      {
         _peripheralVenousBloodObserver.Persistable.ShouldBeTrue();
      }
   }

   public class When_updating_the_simulation_settings_for_a_population_simulation : concern_for_SimulationPersistableUpdater
   {
      private PopulationSimulation _populationSimulation;
      private Observer _venousBloodPlasma;
      private Observer _fabsObserver;
      private Observer _peripheralVenousBloodObserver;

      protected override void Context()
      {
         base.Context();
         var organsim = new Container().WithName(Constants.ORGANISM);
         var venousBlood = new Container().WithName(CoreConstants.Organ.VENOUS_BLOOD).WithParentContainer(organsim);
         var peripheralVenousBlood = new Container().WithName(CoreConstants.Organ.PERIPHERAL_VENOUS_BLOOD).WithParentContainer(organsim);
         var lumen = new Container().WithName(CoreConstants.Organ.LUMEN).WithParentContainer(organsim);
         var plasma = new Container().WithName(CoreConstants.Compartment.PLASMA);
         var moleculeVenousBlood = new Container().WithName("DRUG")
            .WithParentContainer(plasma.WithParentContainer(venousBlood));

         var moleculePeripheral = new Container().WithName("DRUG")
            .WithParentContainer(peripheralVenousBlood);

         _peripheralVenousBloodObserver = new Observer().WithName(CoreConstants.Observer.PLASMA_PERIPHERAL_VENOUS_BLOOD);
         moleculePeripheral.Add(_peripheralVenousBloodObserver);

         _venousBloodPlasma = new Observer().WithName(CoreConstants.Observer.CONCENTRATION_IN_CONTAINER);
         moleculeVenousBlood.Add(_venousBloodPlasma);

         var moleculeLumen = new Container().WithName("DRUG")
            .WithParentContainer(lumen);

         _fabsObserver = new Observer().WithName(CoreConstants.Observer.FABS_ORAL);
         moleculeLumen.Add(_fabsObserver);

         var compound = new Compound().WithName("DRUG");

         _populationSimulation = new PopulationSimulation();
         _populationSimulation.SimulationSettings = new SimulationSettings();
         _populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("Id", PKSimBuildingBlockType.Compound) { BuildingBlock = compound });
         _populationSimulation.Model = new OSPSuite.Core.Domain.Model();
         _populationSimulation.Model.Root = new Container();
         _populationSimulation.Model.Root.Add(organsim);
      }

      protected override void Because()
      {
         sut.UpdatePersistableFromSettings(_populationSimulation);
      }

      [Observation]
      public void should_always_select_the_venous_blood_plasma_observer()
      {
         _venousBloodPlasma.Persistable.ShouldBeTrue();
      }

      [Observation]
      public void should_always_select_the_lumen_fabs_observer()
      {
         _fabsObserver.Persistable.ShouldBeTrue();
      }

      [Observation]
      public void should_always_select_the_peripheral_venous_blood_plasna()
      {
         _peripheralVenousBloodObserver.Persistable.ShouldBeTrue();
      }
   }

   public class When_resetting_the_simulation_settings_for_a_given_simulation : concern_for_SimulationPersistableUpdater
   {
      private Observer _urineConcentrationObserver;
      private Observer _fecesConcentrationObserver;
      private MoleculeAmount _moleculeUrine;
      private MoleculeAmount _moleculeFeces;
      private MoleculeAmount _moleculeGallBladder;
      private IndividualSimulation _individualSimulation;

      protected override void Context()
      {
         base.Context();
         var organsim = new Container().WithName(Constants.ORGANISM);
         var kidney = new Container().WithName(CoreConstants.Organ.KIDNEY).WithParentContainer(organsim);
         var urine = new Container().WithName(CoreConstants.Compartment.URINE).WithParentContainer(kidney);
         var lumen = new Container().WithName(CoreConstants.Organ.LUMEN).WithParentContainer(organsim);
         var feces = new Container().WithName(CoreConstants.Compartment.FECES).WithParentContainer(lumen);
         var gallBladder = new Container().WithName(CoreConstants.Organ.GALLBLADDER).WithParentContainer(organsim);

         _moleculeUrine = new MoleculeAmount().WithName("DRUG")
            .WithParentContainer(urine);
         _moleculeUrine.Persistable = false;

         _moleculeFeces = new MoleculeAmount().WithName("DRUG")
            .WithParentContainer(feces);
         _moleculeFeces.Persistable = false;

         _moleculeGallBladder = new MoleculeAmount().WithName("DRUG")
            .WithParentContainer(gallBladder);
         _moleculeGallBladder.Persistable = false;

         _urineConcentrationObserver = new Observer().WithName(CoreConstants.Observer.CONCENTRATION_IN_CONTAINER);
         _urineConcentrationObserver.Persistable = true;
         _moleculeUrine.Add(_urineConcentrationObserver);

         _fecesConcentrationObserver = new Observer().WithName(CoreConstants.Observer.CONCENTRATION_IN_CONTAINER);
         _fecesConcentrationObserver.Persistable = true;
         _moleculeFeces.Add(_fecesConcentrationObserver);


         var compound = new Compound().WithName("DRUG");

         _individualSimulation = new IndividualSimulation();
         _individualSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("Id", PKSimBuildingBlockType.Compound) { BuildingBlock = compound });
         _individualSimulation.Model = new OSPSuite.Core.Domain.Model { Root = new Container { organsim } };
      }

      protected override void Because()
      {
         sut.ResetPersistable(_individualSimulation);
      }

      [Observation]
      public void should_set_the_urine_amount_as_persistable()
      {
         _moleculeUrine.Persistable.ShouldBeTrue();
      }

      [Observation]
      public void should_set_the_feces_amount_as_persistable()
      {
         _moleculeFeces.Persistable.ShouldBeTrue();
      }

      [Observation]
      public void should_set_the_gall_bladder_amount_as_persistable()
      {
         _moleculeGallBladder.Persistable.ShouldBeTrue();
      }

      [Observation]
      [Ignore("47-8079 Observed data Amount in Urine / Feces cannot be mapped to observers in PK Sim")]
      public void should_set_the_urine_concentration_as_non_persistable()
      {
         _urineConcentrationObserver.Persistable.ShouldBeFalse();
      }

      [Observation]
      [Ignore("47-8079 Observed data Amount in Urine / Feces cannot be mapped to observers in PK Sim")]
      public void should_set_the_feced_concentration_as_non_persistable()
      {
         _fecesConcentrationObserver.Persistable.ShouldBeFalse();
      }
   }
}
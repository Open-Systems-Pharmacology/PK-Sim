using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation;

namespace PKSim.Core
{
   public abstract class concern_for_PKSimPathToPathElementsMapper : ContextSpecification<PKSimPathToPathElementsMapper>
   {
      private IEntityPathResolver _entityPathResolver;
      private IRepresentationInfoRepository _representationInfoRepository;
      protected IEntity _entity;
      protected PathElements _pathElements;
      private Organism _organism;
      private Organ _venousBlood;
      private Compartment _plasma;
      protected MoleculeAmount _drug;
      protected string _drugName = "DRUG";
      protected IDimension _concentrationDimension;
      protected IDimension _fractionDimension;
      protected Container _drugContainerVenousBlood;
      private Container _peripheralVenousBlood;
      protected Container _drugContainerPeripheralVenousBlood;
      protected Organ _gallBladder;
      protected Container _drugContainerGallBladder;
      private Organ _lumen;
      protected Container _drugContainerInLumen;
      protected Neighborhood _liverCellToLiverIntNeighborhood;
      private Organ _liver;
      private Compartment _liverInt;
      private Compartment _liverCell;
      private Container _neighborhoods;
      protected Compartment _mucosa;
      protected Compartment _mucosa_duo;
      protected Compartment _mucosa_duo_interstitial;
      private Organ _smallIntestine;
      protected EventGroup _eventGroup;
      protected Container _events;
      protected IContainer _formulation;
      private Container _applications;
      protected EventGroup _application1;
      private Organ _kidney;
      private Compartment _kidneyUrine;
      protected MoleculeAmount _drugUrineKidney;
      protected Container _drugContainerInLiverCell;

      protected override void Context()
      {
         _entityPathResolver = new EntityPathResolverForSpecs();
         _representationInfoRepository = new RepresentationInfoRepositoryForSpecs();
         sut = new PKSimPathToPathElementsMapper(_representationInfoRepository, _entityPathResolver);

         _organism = new Organism();
         _venousBlood = new Organ().WithName(CoreConstants.Organ.VenousBlood).WithParentContainer(_organism);
         _liver = new Organ().WithName(CoreConstants.Organ.Liver).WithParentContainer(_organism);
         _kidney = new Organ().WithName(CoreConstants.Organ.Kidney).WithParentContainer(_organism);
         _liverInt = new Compartment().WithName(CoreConstants.Compartment.INTERSTITIAL).WithParentContainer(_liver);
         _liverCell = new Compartment().WithName(CoreConstants.Compartment.INTRACELLULAR).WithParentContainer(_liver);
         _kidneyUrine = new Compartment().WithName(CoreConstants.Compartment.URINE).WithParentContainer(_kidney);
         _gallBladder = new Organ().WithName(CoreConstants.Organ.Gallbladder).WithParentContainer(_organism);
         _lumen = new Organ().WithName(CoreConstants.Organ.Lumen).WithParentContainer(_organism);
         _peripheralVenousBlood = new Container().WithName(CoreConstants.Organ.PeripheralVenousBlood).WithParentContainer(_organism);
         _smallIntestine = new Organ().WithName(CoreConstants.Organ.SmallIntestine).WithParentContainer(_organism);
         _mucosa = new Compartment().WithName(CoreConstants.Compartment.MUCOSA).WithParentContainer(_smallIntestine);
         _mucosa_duo = new Compartment().WithName(CoreConstants.Compartment.Duodenum).WithParentContainer(_mucosa);
         _mucosa_duo_interstitial = new Compartment().WithName(CoreConstants.Compartment.INTERSTITIAL).WithParentContainer(_mucosa_duo);

         _events = new Container().WithName(Constants.EVENTS);
         _eventGroup = new EventGroup().WithName("E1").WithParentContainer(_events);
         _plasma = new Compartment().WithName(CoreConstants.Compartment.PLASMA).WithParentContainer(_venousBlood);
         _drugContainerVenousBlood = new Container().WithName(_drugName).WithContainerType(ContainerType.Molecule).WithParentContainer(_venousBlood);
         _drugContainerPeripheralVenousBlood = new Container().WithName(_drugName).WithContainerType(ContainerType.Molecule).WithParentContainer(_peripheralVenousBlood);
         _drugContainerGallBladder = new Container().WithName(_drugName).WithContainerType(ContainerType.Molecule).WithParentContainer(_gallBladder);
         _drugContainerInLumen = new Container().WithName(_drugName).WithContainerType(ContainerType.Molecule).WithParentContainer(_lumen);
         _drugContainerInLiverCell = new Container().WithName(_drugName).WithContainerType(ContainerType.Molecule).WithParentContainer(_liverCell);
         _drug = new MoleculeAmount().WithName(_drugName).WithParentContainer(_plasma);

         _drugUrineKidney = new MoleculeAmount().WithName(_drugName).WithParentContainer(_kidneyUrine);
         _applications = new Container().WithName(Constants.APPLICATIONS);
         _application1 = new EventGroup().WithName("App").WithParentContainer(_applications).WithContainerType(ContainerType.EventGroup);
         _formulation = new Container().WithName("F1").WithParentContainer(_application1);

         _neighborhoods = new Container().WithName(Constants.NEIGHBORHOODS);
         _liverCellToLiverIntNeighborhood = new Neighborhood {FirstNeighbor = _liverInt, SecondNeighbor = _liverCell}.WithParentContainer(_neighborhoods);
         _concentrationDimension = DomainHelperForSpecs.ConcentrationDimensionForSpecs();
         _fractionDimension = DomainHelperForSpecs.FractionDimensionForSpecs();
      }

      protected override void Because()
      {
         _pathElements = sut.MapFrom(_entity);
      }

      protected void ShouldReturnPathElementValues(string simulation, string topContainer, string container, string compartment, string molecule, string name)
      {
         _pathElements[PathElementId.Simulation].DisplayName.ShouldBeEqualTo(simulation);
         _pathElements[PathElementId.TopContainer].DisplayName.ShouldBeEqualTo(topContainer);
         _pathElements[PathElementId.Container].DisplayName.ShouldBeEqualTo(container);
         _pathElements[PathElementId.BottomCompartment].DisplayName.ShouldBeEqualTo(compartment);
         _pathElements[PathElementId.Molecule].DisplayName.ShouldBeEqualTo(molecule);
         _pathElements[PathElementId.Name].DisplayName.ShouldBeEqualTo(name);
      }
   }

   public class When_creating_the_path_elements_for_a_plasma_unbound_observer : concern_for_PKSimPathToPathElementsMapper
   {
      protected override void Context()
      {
         base.Context();
         _entity = new Observer().WithName(CoreConstants.Observer.PLASMA_UNBOUND)
            .WithParentContainer(_drug)
            .WithDimension(_concentrationDimension);
      }

      [Observation]
      public void should_set_the_name_of_the_bottom_compartment_to_the_name_of_the_observer()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.ORGANISM, CoreConstants.Organ.VenousBlood, CoreConstants.Observer.PLASMA_UNBOUND, _drugName, CoreConstants.Output.Concentration);
      }
   }

   public class When_creating_the_path_elements_for_an_concentration_observer : concern_for_PKSimPathToPathElementsMapper
   {
      protected override void Context()
      {
         base.Context();
         _entity = new Observer().WithName(CoreConstants.Observer.CONCENTRATION_IN_CONTAINER)
            .WithParentContainer(_drug)
            .WithDimension(_concentrationDimension);
      }

      [Observation]
      public void should_set_the_name_of_the_observer_to_concentration()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.ORGANISM, CoreConstants.Organ.VenousBlood, CoreConstants.Compartment.PLASMA, _drugName, CoreConstants.Output.Concentration);
      }
   }

   public class When_creating_the_path_elements_for_an_observer_that_is_not_a_default_observer : concern_for_PKSimPathToPathElementsMapper
   {
      protected override void Context()
      {
         base.Context();
         _entity = new Observer().WithName("TOTO")
            .WithParentContainer(_drug)
            .WithDimension(_concentrationDimension);
      }

      [Observation]
      public void should_not_rename_the_name_entry()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.ORGANISM, CoreConstants.Organ.VenousBlood, CoreConstants.Compartment.PLASMA, _drugName, _entity.Name);
      }
   }

   public class When_creating_the_path_elements_for_a_container_observer_such_as_whole_blood : concern_for_PKSimPathToPathElementsMapper
   {
      protected override void Context()
      {
         base.Context();
         _entity = new Observer().WithName(CoreConstants.Observer.WHOLE_BLOOD)
            .WithParentContainer(_drugContainerVenousBlood)
            .WithDimension(_concentrationDimension);
      }

      [Observation]
      public void should_set_the_name_of_the_observer_as_compartment_name()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.ORGANISM, CoreConstants.Organ.VenousBlood, CoreConstants.Observer.WHOLE_BLOOD, _drugName, CoreConstants.Output.Concentration);
      }
   }

   public class When_creating_the_path_elements_for_the_fraction_excreted_observer_into_urine : concern_for_PKSimPathToPathElementsMapper
   {
      protected override void Context()
      {
         base.Context();
         _entity = new Observer().WithName(CoreConstants.Observer.FRACTION_EXCRETED_TO_URINE)
            .WithParentContainer(_drugUrineKidney)
            .WithDimension(_fractionDimension);
      }

      [Observation]
      public void should_keep_the_name_of_the_observer_as_name()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.ORGANISM, CoreConstants.Organ.Kidney, CoreConstants.Compartment.URINE, _drugName, CoreConstants.Observer.FRACTION_EXCRETED_TO_URINE);
      }
   }

   public class When_creating_the_path_elements_for_the_fraction_of_dose_liver_observer : concern_for_PKSimPathToPathElementsMapper
   {
      protected override void Context()
      {
         base.Context();
         _entity = new Observer().WithName(CoreConstants.CompositeNameFor(CoreConstants.Observer.FRACTION_OF_DOSE, _drugName, CoreConstants.Organ.Liver, CoreConstants.Compartment.INTRACELLULAR))
            .WithParentContainer(_drugContainerInLiverCell)
            .WithDimension(_fractionDimension);
      }

      [Observation]
      public void should_keep_the_name_of_the_observer_as_name()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.ORGANISM, CoreConstants.Organ.Liver, CoreConstants.Compartment.INTRACELLULAR, _drugName, CoreConstants.CompositeNameFor(CoreConstants.Observer.FRACTION_OF_DOSE, _drugName));
      }
   }

   public class When_creating_the_path_elements_for_the_receptor_occupancy_observer_into_urine : concern_for_PKSimPathToPathElementsMapper
   {
      private string _observerName;

      protected override void Context()
      {
         base.Context();
         _observerName = CoreConstants.Observer.ObserverNameFrom(CoreConstants.Observer.RECEPTOR_OCCUPANCY, "Complex");
         _entity = new Observer().WithName(_observerName)
            .WithParentContainer(_drugUrineKidney)
            .WithDimension(_fractionDimension);
      }

      [Observation]
      public void should_keep_the_name_of_the_observer_as_name()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.ORGANISM, CoreConstants.Organ.Kidney, CoreConstants.Compartment.URINE, _drugName, _observerName);
      }
   }

   public class When_creating_the_path_elements_for_observers_defined_in_peripheral_venous_blood : concern_for_PKSimPathToPathElementsMapper
   {
      protected override void Context()
      {
         base.Context();
         _entity = new Observer().WithName(CoreConstants.Observer.PLASMA_PERIPHERAL_VENOUS_BLOOD)
            .WithParentContainer(_drugContainerPeripheralVenousBlood)
            .WithDimension(_concentrationDimension);
      }

      [Observation]
      public void should_use_the_display_name_of_the_observer_as_compartment_name()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.ORGANISM, CoreConstants.Organ.PeripheralVenousBlood, CoreConstants.Observer.PLASMA_PERIPHERAL_VENOUS_BLOOD, _drugName, CoreConstants.Output.Concentration);
      }
   }

   public class When_creating_the_path_elements_for_the_fraciton_excreted_to_bile_observer_defined_in_gall_bladder : concern_for_PKSimPathToPathElementsMapper
   {
      protected override void Context()
      {
         base.Context();
         _entity = new Observer().WithName(CoreConstants.Observer.FRACTION_EXCRETED_TO_BILE)
            .WithParentContainer(_drugContainerGallBladder)
            .WithDimension(_concentrationDimension);
      }

      [Observation]
      public void should_not_set_a_value_for_the_compartment()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.ORGANISM, CoreConstants.Organ.Gallbladder, string.Empty, _drugName, CoreConstants.Observer.FRACTION_EXCRETED_TO_BILE);
      }
   }

   public class When_creating_the_path_elements_for_any_observers_defined_in_gall_bladder_but_fraction_excreted_to_bile : concern_for_PKSimPathToPathElementsMapper
   {
      protected override void Context()
      {
         base.Context();
         _entity = new Observer().WithName(CoreConstants.Observer.PLASMA_UNBOUND)
            .WithParentContainer(_drugContainerGallBladder)
            .WithDimension(_concentrationDimension);
      }

      [Observation]
      public void should_use_the_display_name_of_the_observer_as_compartment_name()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.ORGANISM, CoreConstants.Organ.Gallbladder, CoreConstants.Observer.PLASMA_UNBOUND, _drugName, CoreConstants.Output.Concentration);
      }
   }

   public class When_creating_the_path_elements_for_the_concentration_observers_defined_in_lumen : concern_for_PKSimPathToPathElementsMapper
   {
      protected override void Context()
      {
         base.Context();
         _entity = new Observer().WithName(CoreConstants.Observer.PLASMA_UNBOUND)
            .WithParentContainer(_drugContainerInLumen)
            .WithDimension(_concentrationDimension);
      }

      [Observation]
      public void should_use_the_display_name_of_dimension()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.ORGANISM, CoreConstants.Organ.Lumen, string.Empty, _drugName, CoreConstants.Output.Concentration);
      }
   }

   public class When_creating_the_path_elements_for_the_fractions_observers_defined_in_lumen : concern_for_PKSimPathToPathElementsMapper
   {
      protected override void Context()
      {
         base.Context();
         _entity = new Observer().WithName(CoreConstants.Observer.PLASMA_UNBOUND)
            .WithParentContainer(_drugContainerInLumen)
            .WithDimension(_fractionDimension);
      }

      [Observation]
      public void should_use_the_display_name_of_the_observer()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.ORGANISM, CoreConstants.Organ.Lumen, string.Empty, _drugName, CoreConstants.Observer.PLASMA_UNBOUND);
      }
   }

   public class When_creating_the_path_elements_of_a_parameter_defined_in_a_neighborhood : concern_for_PKSimPathToPathElementsMapper
   {
      protected override void Context()
      {
         base.Context();
         _entity = new Parameter().WithName("Param")
            .WithParentContainer(_liverCellToLiverIntNeighborhood);
      }

      [Observation]
      public void should_use_the_parent_of_the_first_ancestor_as_container()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.NEIGHBORHOODS, CoreConstants.Organ.Liver, string.Empty, string.Empty, _entity.Name);
         _pathElements.Category.ShouldBeEqualTo(PKSimConstants.ObjectTypes.Organs);
      }
   }

   public class When_creating_the_path_elements_of_a_parameter_defined_in_the_mucosa : concern_for_PKSimPathToPathElementsMapper
   {
      protected override void Context()
      {
         base.Context();
         _entity = new Parameter().WithName("Param")
            .WithParentContainer(_mucosa_duo);
      }

      [Observation]
      public void should_set_the_category_to_mucosa()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.ORGANISM, CoreConstants.Compartment.Duodenum, string.Empty, string.Empty, _entity.Name);
         _pathElements.Category.ShouldBeEqualTo(PKSimConstants.ObjectTypes.Mucosa);
      }
   }

   public class When_creating_the_path_element_for_a_ph_parameter_defined_in_the_interstitial_space_of_the_small_intestine_mucosa_duodenum : concern_for_PKSimPathToPathElementsMapper
   {
      protected override void Context()
      {
         base.Context();
         _entity = new Parameter().WithName("pH")
            .WithParentContainer(_mucosa_duo_interstitial);
      }

      [Observation]
      public void should_set_the_category_to_mucosa()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.ORGANISM, CoreConstants.Compartment.Duodenum, CoreConstants.Compartment.INTERSTITIAL, string.Empty, _entity.Name);
         _pathElements.Category.ShouldBeEqualTo(PKSimConstants.ObjectTypes.Mucosa);
      }
   }

   public class When_creating_the_path_element_for_an_event_parameter : concern_for_PKSimPathToPathElementsMapper
   {
      protected override void Context()
      {
         base.Context();
         _entity = new Parameter().WithName("StartTime")
            .WithParentContainer(_eventGroup);
      }

      [Observation]
      public void should_set_the_container_to_the_name_of_the_parent_event_group()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.EVENTS, _eventGroup.Name, string.Empty, string.Empty, _entity.Name);
      }
   }

   public class When_creating_the_path_element_for_a_formulation_parameters : concern_for_PKSimPathToPathElementsMapper
   {
      protected override void Context()
      {
         base.Context();
         _entity = new Parameter().WithName("Lint80")
            .WithParentContainer(_formulation);
      }

      [Observation]
      public void should_set_the_container_to_the_name_of_the_parent_application()
      {
         ShouldReturnPathElementValues(string.Empty, Constants.APPLICATIONS, _application1.Name, string.Empty, string.Empty, _entity.Name);
      }
   }
}
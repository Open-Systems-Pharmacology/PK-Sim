using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;
using CompoundProcess = PKSim.Core.Snapshots.CompoundProcess;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_CompoundProcessMapper : ContextSpecificationAsync<CompoundProcessMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected EnzymaticProcess _enzymaticProcess;
      protected EnzymaticProcessWithSpecies _enzymaticProcessWithSpecies;
      protected CompoundProcess _snapshot;
      private IRepresentationInfoRepository _representationInfoRepository;
      protected ICompoundProcessRepository _compoundProcessRepository;
      protected ICloner _cloner;
      protected ISpeciesRepository _speciesRepository;
      protected ICompoundProcessTask _compoundProcessTask;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _compoundProcessRepository = A.Fake<ICompoundProcessRepository>();
         _cloner = A.Fake<ICloner>();
         _speciesRepository = A.Fake<ISpeciesRepository>();
         _compoundProcessTask = A.Fake<ICompoundProcessTask>();

         sut = new CompoundProcessMapper(_parameterMapper, _representationInfoRepository, _compoundProcessRepository, _cloner, _speciesRepository, _compoundProcessTask);

         _enzymaticProcess = new EnzymaticProcess
         {
            Name = "Enzymatic Process",
            Description = "Process description",
            InternalName = "A",
            DataSource = "B",
            MetaboliteName = "Meta"
         };

         //Same description as DB
         A.CallTo(() => _representationInfoRepository.DescriptionFor(RepresentationObjectType.PROCESS, _enzymaticProcess.InternalName)).Returns(_enzymaticProcess.Description);

         _enzymaticProcessWithSpecies = new EnzymaticProcessWithSpecies
         {
            Name = "Enzymatic process with species",
            Description = "toto",
            Species = new Species().WithName("Human"),
            InternalName = "C",
            DataSource = "D",
         };

         A.CallTo(() => _representationInfoRepository.DescriptionFor(RepresentationObjectType.PROCESS, _enzymaticProcessWithSpecies.InternalName)).Returns("Process description");

         return Task.FromResult(true);
      }
   }

   public class When_mapping_an_enzymatic_process_to_snapshot : concern_for_CompoundProcessMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_enzymaticProcess);
      }

      [Observation]
      public void should_create_the_snapshot_process_with_the_expected_properties()
      {
         _snapshot.Name.ShouldBeNull();
         _snapshot.Description.ShouldBeNull();
      }

      [Observation]
      public void should_save_the_partial_process_specifc_properites_to_snapshot()
      {
         _snapshot.InternalName.ShouldBeEqualTo(_enzymaticProcess.InternalName);
         _snapshot.DataSource.ShouldBeEqualTo(_enzymaticProcess.DataSource);
         _snapshot.Molecule.ShouldBeEqualTo(_enzymaticProcess.MoleculeName);
         _snapshot.Metabolite.ShouldBeEqualTo(_enzymaticProcess.MetaboliteName);
      }

      [Observation]
      public void should_set_the_unused_properties_to_null()
      {
         _snapshot.Species.ShouldBeNull();
      }
   }

   public class When_mapping_an_species_dependent_enzymatic_process_to_snapshot : concern_for_CompoundProcessMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_enzymaticProcessWithSpecies);
      }

      [Observation]
      public void should_create_the_snapshot_process_with_the_expected_properties()
      {
         _snapshot.Name.ShouldBeNull();
         _snapshot.Description.ShouldBeEqualTo(_enzymaticProcessWithSpecies.Description);
      }

      [Observation]
      public void should_save_the_partial_process_specifc_properites_to_snapshot()
      {
         _snapshot.InternalName.ShouldBeEqualTo(_enzymaticProcessWithSpecies.InternalName);
         _snapshot.DataSource.ShouldBeEqualTo(_enzymaticProcessWithSpecies.DataSource);
         _snapshot.Molecule.ShouldBeEqualTo(_enzymaticProcessWithSpecies.MoleculeName);
         _snapshot.Species.ShouldBeEqualTo(_enzymaticProcessWithSpecies.Species.Name);
      }
   }

   public class When_mapping_a_valid_systemic_process_snapshot_to_a_process : concern_for_CompoundProcessMapper
   {
      private EnzymaticProcess _templateProcess;
      private EnzymaticProcess _newEnzymaticProcess;
      private EnzymaticProcess _cloneOfTemplate;
      private Parameter _snapshotParameter;
      private IParameter _processParameter;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_enzymaticProcess);
         _templateProcess = new EnzymaticProcess();
         _templateProcess.InternalName = _snapshot.InternalName;
         A.CallTo(() => _compoundProcessRepository.ProcessByName(_snapshot.InternalName)).Returns(_templateProcess);
         _cloneOfTemplate = new EnzymaticProcess();
         _cloneOfTemplate.InternalName = _snapshot.InternalName;
         A.CallTo(() => _cloner.Clone((Model.CompoundProcess) _templateProcess)).Returns(_cloneOfTemplate);


         _snapshot.Description = null;
         _cloneOfTemplate.Description = "Description of template from database";
         _snapshot.Molecule = "CYP3A4";
         _snapshot.Metabolite = "Meta";
         _snapshot.DataSource = "Lab";

         _snapshotParameter = new Parameter().WithName("Km");
         _snapshot.Parameters = new []{_snapshotParameter};

         _processParameter = DomainHelperForSpecs.ConstantParameterWithValue(5).WithName("Km");
         _cloneOfTemplate.Add(_processParameter);
      }

      protected override async Task Because()
      {
         _newEnzymaticProcess = await sut.MapToModel(_snapshot) as EnzymaticProcess;
      }

      [Observation]
      public void should_load_the_template_from_the_database_and_return_a_clone_of_the_used_template()
      {
         _newEnzymaticProcess.ShouldBeEqualTo(_cloneOfTemplate);
      }

      [Observation]
      public void should_not_override_the_description_from_the_process_coming_from_the_database()
      {
         _newEnzymaticProcess.Description.ShouldBeEqualTo(_cloneOfTemplate.Description);
      }

      [Observation]
      public void should_have_set_the_enzymatic_process_specific_properties()
      {
         _newEnzymaticProcess.DataSource.ShouldBeEqualTo(_snapshot.DataSource);
         _newEnzymaticProcess.MoleculeName.ShouldBeEqualTo(_snapshot.Molecule);
         _newEnzymaticProcess.MetaboliteName.ShouldBeEqualTo(_snapshot.Metabolite);
         _newEnzymaticProcess.Name.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_update_the_parameters_from_snapshot()
      {
         A.CallTo(() => _parameterMapper.MapParameters(_snapshot.Parameters , _newEnzymaticProcess, _snapshot.InternalName)).MustHaveHappened();
      }
   }

   public class When_mapping_a_valid_species_dependent_snapshot_to_process : concern_for_CompoundProcessMapper
   {
      private EnzymaticProcessWithSpecies _newEnzymaticProcess;
      private EnzymaticProcessWithSpecies _templateProcess;
      private Species _species;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_enzymaticProcess);
         _templateProcess = new EnzymaticProcessWithSpecies();
         A.CallTo(() => _compoundProcessRepository.ProcessByName(_snapshot.InternalName)).Returns(_templateProcess);
         A.CallTo(() => _cloner.Clone((Model.CompoundProcess)_templateProcess)).Returns(_templateProcess);


         _snapshot.Molecule = "CYP3A4";
         _snapshot.DataSource = "Lab";
         _snapshot.Species = "Human";
         _species = new Species {Name = _snapshot.Species};
         A.CallTo(() => _speciesRepository.All()).Returns(new[] {_species});
      }

      protected override async Task Because()
      {
         _newEnzymaticProcess = await sut.MapToModel(_snapshot) as EnzymaticProcessWithSpecies;
      }

    [Observation]
      public void should_update_the_species_for_created_process()
      {
         A.CallTo(() => _compoundProcessTask.SetSpeciesForProcess(_newEnzymaticProcess, _species)).MustHaveHappened();
      }
   }
}
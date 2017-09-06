using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Mappers;
using CompoundProcess = PKSim.Core.Snapshots.CompoundProcess;

namespace PKSim.Core
{
   public abstract class concern_for_CompoundProcessMapper : ContextSpecification<CompoundProcessMapper>
   {
      private ParameterMapper _parameterMapper;
      protected EnzymaticProcess _enzymaticProcess;
      protected EnzymaticProcessWithSpecies _enzymaticProcessWithSpecies;
      protected CompoundProcess _snapshot;
      private IRepresentationInfoRepository _representationInfoRepository;

      protected override void Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _representationInfoRepository= A.Fake<IRepresentationInfoRepository>();
         sut = new CompoundProcessMapper(_parameterMapper, _representationInfoRepository);

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
      }
   }

   public class When_mapping_an_enzymatic_process_to_snapshot : concern_for_CompoundProcessMapper
   {
      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_enzymaticProcess);
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
      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_enzymaticProcessWithSpecies);
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
}
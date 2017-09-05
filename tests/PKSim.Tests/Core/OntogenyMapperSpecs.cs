using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_OntogenyMapper : ContextSpecification<OntogenyMapper>
   {
      protected DistributedTableFormulaMapper _distributedTableFormulaMapper;
      protected Ontogeny _ontogeny;
      protected DistributedTableFormula _distributedTableFormula;
      protected Snapshots.DistributedTableFormula _snapshotTable;
      protected ISimulationSubject _simulationSubject;
      private IOntogenyRepository _ontogenyRepository;

      protected override void Context()
      {
         _distributedTableFormulaMapper = A.Fake<DistributedTableFormulaMapper>();
         _ontogenyRepository= A.Fake<IOntogenyRepository>();

         sut = new OntogenyMapper(_distributedTableFormulaMapper, _ontogenyRepository);

         _distributedTableFormula = new DistributedTableFormula();
         _snapshotTable = new Snapshots.DistributedTableFormula();
         A.CallTo(() => _distributedTableFormulaMapper.MapToSnapshot(_distributedTableFormula)).Returns(_snapshotTable);
         
         _simulationSubject= A.Fake<ISimulationSubject>();
      }
   }

   public class When_mapping_an_undefined_ontogeny_to_snapshot : concern_for_OntogenyMapper
   {
      [Observation]
      public void should_return_null()
      {
         sut.MapToSnapshot(null).ShouldBeNull();
         sut.MapToSnapshot(new NullOntogeny()).ShouldBeNull();
      }
   }

   public class When_mapping_a_database_ontogeny_to_snapshot : concern_for_OntogenyMapper
   {
      private Snapshots.Ontogeny _snapshot;

      protected override void Context()
      {
         base.Context();
         _ontogeny = new DatabaseOntogeny
         {
            Name = "Database",
            Description = "Database description"
         };
      }

      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_ontogeny);
      }

      [Observation]
      public void should_have_saved_the_name_of_the_ontogeny()
      {
         _snapshot.Name.ShouldBeEqualTo(_ontogeny.Name);
      }

      [Observation]
      public void should_not_save_the_ontogeny_description()
      {
         _snapshot.Description.ShouldBeNull();
      }

      [Observation]
      public void the_ontogeny_table_should_be_null()
      {
         _snapshot.Table.ShouldBeNull();
      }
   }

   public class When_mapping_a_user_defined_ontogeny_to_snapshot : concern_for_OntogenyMapper
   {
      private Snapshots.Ontogeny _snapshot;

      protected override void Context()
      {
         base.Context();
         _ontogeny = new UserDefinedOntogeny
         {
            Name = "UserDefined",
            Description = "UserDefined description",
            Table = _distributedTableFormula
         };

      }

      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_ontogeny);
      }

      [Observation]
      public void should_have_saved_the_name_and_the_description_of_the_ontogeny()
      {
         _snapshot.Name.ShouldBeEqualTo(_ontogeny.Name);
         _snapshot.Description.ShouldBeEqualTo(_ontogeny.Description);
      }

      [Observation]
      public void should_save_the_ontogeny_table()
      {
         _snapshot.Table.ShouldBeEqualTo(_snapshotTable);
      }
   }

   public class When_mapping_a_user_defined_ontogeny_snapshot_to_model_ontogeny : concern_for_OntogenyMapper
   {
      private Snapshots.Ontogeny _snapshot;
      private Ontogeny _newOntogeny;

      protected override void Context()
      {
         base.Context();
         _ontogeny = new UserDefinedOntogeny
         {
            Name = "UserDefined",
            Description = "UserDefined description",
            Table = _distributedTableFormula
         };

         _snapshot = sut.MapToSnapshot(_ontogeny);
         A.CallTo(() => _distributedTableFormulaMapper.MapToModel(_snapshotTable)).Returns(_distributedTableFormula);
      }

      protected override void Because()
      {
         _newOntogeny = sut.MapToModel(_snapshot, _simulationSubject);
      }

      [Observation]
      public void should_return_the_expected_ontogeny()
      {
         _newOntogeny.IsUserDefined().ShouldBeTrue();
      }
   }
}
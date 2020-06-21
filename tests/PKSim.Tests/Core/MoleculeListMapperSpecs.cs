using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_MoleculeListMapper : ContextSpecificationAsync<MoleculeListMapper>
   {
      protected MoleculeList _moleculeList;
      protected override Task Context()
      {
         sut = new MoleculeListMapper();
         _moleculeList = new MoleculeList();
         return _completed;
      }
   }


   public class When_mapping_empty_molecule_list_to_molecule_list_snapshot : concern_for_MoleculeListMapper
   {
      private Snapshots.MoleculeList _result;

      protected override async Task Because()
      {
         _result = await sut.MapToSnapshot(_moleculeList);
      }

      [Observation]
      public void should_return_a_valid_snapshot()
      {
         _result.ShouldNotBeNull();
         _result.ForAll.ShouldBeEqualTo(_moleculeList.ForAll);
         _result.MoleculeNamesToInclude.ShouldBeNull();
         _result.MoleculeNamesToExclude.ShouldBeNull();
      }
   }

   public class When_mapping_valid_molecule_list_to_molecule_list_snapshot : concern_for_MoleculeListMapper
   {
      private Snapshots.MoleculeList _result;

      protected override async Task Context()
      {
         await base.Context();
         _moleculeList.ForAll = false;
         _moleculeList.AddMoleculeName("TOTO");
         _moleculeList.AddMoleculeName("TATA");
         _moleculeList.AddMoleculeNameToExclude("TUTU");
      }

      protected override async Task Because()
      {
         _result = await sut.MapToSnapshot(_moleculeList);
      }

      [Observation]
      public void should_return_a_valid_snapshot()
      {
         _result.ShouldNotBeNull();
         _result.ForAll.ShouldBeFalse();
         _result.MoleculeNamesToInclude.ShouldOnlyContain("TOTO", "TATA");
         _result.MoleculeNamesToExclude.ShouldOnlyContain("TUTU");
      }
   }

   public class When_mapping_valid_molecule_list_snapshot_to_molecule_list : concern_for_MoleculeListMapper
   {
      private Snapshots.MoleculeList _snapshot;
      private MoleculeList _result;

      protected override async Task Context()
      {
         await base.Context();
         _moleculeList.ForAll = true;
         _moleculeList.AddMoleculeName("TOTO");
         _moleculeList.AddMoleculeName("TATA");
         _moleculeList.AddMoleculeNameToExclude("TUTU");
         _snapshot = await sut.MapToSnapshot(_moleculeList);

      }

      protected override async Task Because()
      {
         _result = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_return_a_valid_molecule_list()
      {
         _result.ShouldNotBeNull();
         _result.ForAll.ShouldBeTrue();
         _result.MoleculeNames.ShouldOnlyContain("TOTO", "TATA");
         _result.MoleculeNamesToExclude.ShouldOnlyContain("TUTU");
      }
   }
}
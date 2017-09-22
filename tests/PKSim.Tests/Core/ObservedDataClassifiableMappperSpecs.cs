using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using DataRepository = OSPSuite.Core.Domain.Data.DataRepository;

namespace PKSim.Core
{
   public abstract class concern_for_ObservedDataClassifiableMappper : ContextSpecificationAsync<ObservedDataClassifiableMappper>
   {
      private ObservedDataMapper _observedDataMapper;
      protected ClassifiableObservedData _classifiable;
      private DataRepository _repository;
      protected Snapshots.DataRepository _dataRepositorySnapshot;

      protected override Task Context()
      {
         _observedDataMapper = A.Fake<ObservedDataMapper>();
         sut = new ObservedDataClassifiableMappper(_observedDataMapper);
         _repository = DomainHelperForSpecs.ObservedData().WithName("ObservedData");
         _dataRepositorySnapshot = new Snapshots.DataRepository();

         A.CallTo(() => _observedDataMapper.MapToSnapshot(_repository)).ReturnsAsync(_dataRepositorySnapshot);

         _classifiable = new ClassifiableObservedData { Subject = _repository };

         return Task.FromResult(true);
      }
   }

   public class When_mapping_classifiable_to_snapshot : concern_for_ObservedDataClassifiableMappper
   {
      private ObservedDataClassifiable _result;

      protected override async Task Because()
      {
         _result = await sut.MapToSnapshot(_classifiable);
      }

      [Observation]
      public void the_snapshot_should_contain_the_snapshot_for_the_repository()
      {
         _result.Repository.ShouldBeEqualTo(_dataRepositorySnapshot);
      }

      [Observation]
      public void the_snapshot_properties_should_be_set()
      {
         _result.Name.ShouldBeEqualTo(_classifiable.Name);
      }
   }
}

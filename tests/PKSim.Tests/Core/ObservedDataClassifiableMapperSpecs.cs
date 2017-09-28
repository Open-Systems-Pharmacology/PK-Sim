using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_ObservedDataClassifiableMapper : ContextSpecificationAsync<ObservedDataClassifiableMapper>
   {
      protected Classifiable _snapshot;
      protected ClassifiableObservedData _classifiable;
      protected override Task Context()
      {
         _snapshot = new Classifiable();
         _classifiable = new ClassifiableObservedData();
         _classifiable.UpdateSubject(DomainHelperForSpecs.ObservedData().WithName("subject"));
         _classifiable.Parent = new OSPSuite.Core.Domain.Classification{Name = "classification"};
         sut = new ObservedDataClassifiableMapper();
         return _completed;
      }
   }

   public class When_mapping_from_classifiable_to_snapshot : concern_for_ObservedDataClassifiableMapper
   {
      private Classifiable _result;

      protected override async Task Because()
      {
         _result = await sut.MapToSnapshot(_classifiable);
      }

      [Observation]
      public void the_snapshot_properties_should_be_set_correctly()
      {
         _result.Name.ShouldBeEqualTo(_classifiable.Name);
         _result.ClassificationPath.ShouldBeEqualTo(_classifiable.Parent.Path);
      }
   }
}

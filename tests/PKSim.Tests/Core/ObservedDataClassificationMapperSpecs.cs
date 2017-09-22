using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_ObservedDataClassificationMapper : ContextSpecificationAsync<ObservedDataClassificationMapper>
   {
      private ObservedDataClassifiableMappper _observedDataClassifiableMappper;
      protected Classification _classification, _subClassification, _subSubClassification;
      protected ClassifiableObservedData _classifiable;
      protected ObservedDataClassifiable _classifiableSnapshot;
      protected ObservedDataClassificationContext _context;

      protected override Task Context()
      {
         _classifiableSnapshot = new ObservedDataClassifiable();
         _observedDataClassifiableMappper = A.Fake<ObservedDataClassifiableMappper>();
         

         sut = new ObservedDataClassificationMapper(_observedDataClassifiableMappper);

         _classification = new Classification { ClassificationType = ClassificationType.ObservedData };
         _subClassification = new Classification { ClassificationType = ClassificationType.ObservedData, Parent = _classification };
         _subSubClassification = new Classification { ClassificationType = ClassificationType.ObservedData, Parent = _subClassification };
         _classifiable = new ClassifiableObservedData { Parent = _classification };

         _classification.Name = "A Name";
         _subClassification.Name = "Sub Name";
         _subSubClassification.Name = "Sub Sub Name";

         A.CallTo(() => _observedDataClassifiableMappper.MapToSnapshot(_classifiable)).ReturnsAsync(_classifiableSnapshot);

         _context = new ObservedDataClassificationContext
         {
            Classifiables = new List<ClassifiableObservedData> { _classifiable },
            Classifications = new List<Classification> { _classification, _subClassification, _subSubClassification }
         };

         return Task.FromResult(true);
      }
   }

   public class When_mapping_classification_to_snapshot : concern_for_ObservedDataClassificationMapper
   {
      private ObservedDataClassification _result;

      protected override async Task Because()
      {
         _result = await sut.MapToSnapshot(_classification, _context);
      }

      [Observation]
      public void the_snapshot_should_contain_classifiable_snapshots_from_the_original_classification()
      {
         _result.Classifiables.ShouldOnlyContain(_classifiableSnapshot);
      }

      [Observation]
      public void the_snapshot_should_include_the_entire_tree()
      {
         _result.Classifications[0].Classifications.Length.ShouldBeEqualTo(1);
         _result.Classifications[0].Classifications[0].Name.ShouldBeEqualTo(_subSubClassification.Name);
      }

      [Observation]
      public void the_snapshot_should_contain_classification_snapshots_from_the_original_classification()
      {
         _result.Classifications.Length.ShouldBeEqualTo(1);
         _result.Classifications[0].Name.ShouldBeEqualTo(_subClassification.Name);
      }

      [Observation]
      public void the_snapshot_should_have_properties_set()
      {
         _result.Name.ShouldBeEqualTo(_classification.Name);
      }
   }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots.Mappers;
using Classification = PKSim.Core.Snapshots.Classification;

namespace PKSim.Core
{
   public abstract class concern_for_ObservedDataClassificationMapper : ContextSpecificationAsync<ClassificationMapper>
   {
      protected OSPSuite.Core.Domain.Classification _classification, _subClassification, _subSubClassification;
      protected ClassificationContext _context;
      protected ClassifiableContext _classifiable;

      protected override Task Context()
      {
         sut = new ClassificationMapper();

         _classification = new OSPSuite.Core.Domain.Classification { ClassificationType = ClassificationType.ObservedData };
         _subClassification = new OSPSuite.Core.Domain.Classification { ClassificationType = ClassificationType.ObservedData, Parent = _classification };
         _subSubClassification = new OSPSuite.Core.Domain.Classification { ClassificationType = ClassificationType.ObservedData, Parent = _subClassification };
         _classifiable = new ClassifiableContext { Parent = _classification, Name = "classifiableName"};

         _classification.Name = "A Name";
         _subClassification.Name = "Sub Name";
         _subSubClassification.Name = "Sub Sub Name";

         _context = new ClassificationContext
         {
            Classifiables = new List<ClassifiableContext> { _classifiable },
            Classifications = new List<OSPSuite.Core.Domain.Classification> { _classification, _subClassification, _subSubClassification }
         };

         return Task.FromResult(true);
      }
   }

   public class When_mapping_classification_to_snapshot : concern_for_ObservedDataClassificationMapper
   {
      private Classification _result;

      protected override async Task Because()
      {
         _result = await sut.MapToSnapshot(_classification, _context);
      }

      [Observation]
      public void the_snapshot_should_contain_classifiable_snapshots_from_the_original_classification()
      {
         _result.Classifiables.ShouldOnlyContain(_classifiable.Name);
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

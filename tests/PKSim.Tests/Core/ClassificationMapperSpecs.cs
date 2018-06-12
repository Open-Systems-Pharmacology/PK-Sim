using System.Collections.Generic;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Snapshots.Mappers;
using Classification = PKSim.Core.Snapshots.Classification;

namespace PKSim.Core
{
   public abstract class concern_for_ClassificationMapper : ContextSpecificationAsync<ClassificationMapper>
   {
      protected OSPSuite.Core.Domain.Classification _classification, _subClassification, _subSubClassification;
      protected ClassificationContext _context;

      protected override Task Context()
      {
         sut = new ClassificationMapper();

         _classification = new OSPSuite.Core.Domain.Classification { ClassificationType = ClassificationType.ObservedData }.WithName("A Name");
         _subClassification = new OSPSuite.Core.Domain.Classification { ClassificationType = ClassificationType.ObservedData, Parent = _classification }.WithName("Sub NAme");
         _subSubClassification = new OSPSuite.Core.Domain.Classification { ClassificationType = ClassificationType.ObservedData, Parent = _subClassification }.WithName("Sub Sub Name");

         _context = new ClassificationContext
         {
            Classifications = new List<OSPSuite.Core.Domain.Classification> { _classification, _subClassification, _subSubClassification },
            Classifiables = new List<IClassifiableWrapper>()
         };

         return Task.FromResult(true);
      }
   }

   public class When_mapping_snapshot_to_classification : concern_for_ClassificationMapper
   {
      private OSPSuite.Core.Domain.Classification _result;
      private Classification _child;

      protected override async Task Context()
      {
         await base.Context();
         _child = new Classification().WithName("child");
      }

      protected override async Task Because()
      {
         _result = await sut.MapToModel(_child, ClassificationType.ObservedData);
      }

      [Observation]
      public void the_result_should_have_correct_properties()
      {
         _result.Name.ShouldBeEqualTo(_child.Name);
      }
   }

   public class When_mapping_classification_to_snapshot : concern_for_ClassificationMapper
   {
      private Classification _result;

      protected override async Task Context()
      {
         await base.Context();
         _context.Classifiables = new[] { new ClassifiableObservedData { Parent = _classification, Subject = new DataRepository().WithName("Classifiable") } };
      }

      protected override async Task Because()
      {
         _result = await sut.MapToSnapshot(_classification, _context);
      }

      [Observation]
      public void should_have_mapped_names_for_classifiables()
      {
         _result.Classifiables[0].ShouldBeEqualTo("Classifiable");
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

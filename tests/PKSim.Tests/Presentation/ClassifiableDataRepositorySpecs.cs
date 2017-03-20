using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Presentation
{
   public abstract class concern_for_ClassifiableDataRepository : ContextSpecification<ClassifiableObservedData>
   {
      protected DataRepository _fakeDataRepository;

      protected override void Context()
      {
         base.Context();
         _fakeDataRepository = A.Fake<DataRepository>();
         sut = new ClassifiableObservedData {Subject = _fakeDataRepository};
      }
   }

   public class when_retrieving_invalid_classification : concern_for_ClassifiableDataRepository
   {
      private IClassification _result;

      [Observation]
      public void classification_for_should_return_null()
      {
         _result.ShouldBeNull();
      }

      protected override void Because()
      {
         _result = sut.Parent;
      }
   }

   public class when_resolving_tree_without_compartment_metadata : concern_for_ClassifiableDataRepository
   {
      protected IClassification _result;
      protected ExtendedProperties _fakeExtendedInfo;

      protected override void Context()
      {
         base.Context();
         _fakeExtendedInfo = new ExtendedProperties();

         A.CallTo(() => _fakeDataRepository.ExtendedProperties).Returns(_fakeExtendedInfo);
         _fakeExtendedInfo.Add(new ExtendedProperty<string> {Name = PKSimConstants.UI.Compartment, Value = "Liver"});
      }

      protected override void Because()
      {
         _result = sut.Parent;
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }
   }
}
using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization;

namespace PKSim.Core
{
   public abstract class concern_for_ProjectClassifiableUpdaterAfterDeserialization : ContextSpecification<IProjectClassifiableUpdaterAfterDeserialization>
   {
      protected IWithIdRepository _repository;

      protected override void Context()
      {
         _repository = A.Fake<IWithIdRepository>();
         sut = new ProjectClassifiableUpdaterAfterDeserialization(_repository);
      }
   }

   public class When_updating_a_project_afer_deserialization : concern_for_ProjectClassifiableUpdaterAfterDeserialization
   {
      private IProject _project;
      private IClassifiableWrapper _classifiable1;
      private IClassifiableWrapper _classifiable2;
      private IWithId _subject1;

      protected override void Context()
      {
         base.Context();
         _project = A.Fake<IProject>();
         _subject1 = A.Fake<IWithId>();
         _classifiable1 = A.Fake<IClassifiableWrapper>().WithId("1");
         _classifiable2 = A.Fake<IClassifiableWrapper>().WithId("2");
         A.CallTo(() => _repository.Get(_classifiable1.Id)).Returns(_subject1);
         A.CallTo(() => _repository.Get(_classifiable2.Id)).Returns(null);
         A.CallTo(() => _project.AllClassifiablesByType<IClassifiableWrapper>()).Returns(new[] { _classifiable1, _classifiable2 });
      }

      protected override void Because()
      {
         sut.Update(_project);
      }

      [Observation]
      public void should_update_the_subject_for_each_available_classifiable()
      {
         A.CallTo(() => _classifiable1.UpdateSubject(_subject1)).MustHaveHappened();
      }

      [Observation]
      public void should_remove_the_classification_that_are_not_found()
      {
         A.CallTo(() => _project.RemoveClassifiable(_classifiable2)).MustHaveHappened();
      }
   }
}
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_Project : ContextSpecification<IProject>
   {
      protected override void Context()
      {
         sut = new PKSimProject();
      }
   }

   public class When_retrieving_classifiables_from_project_which_are_not_already_created : concern_for_Project
   {
      private Simulation _subject;

      protected override void Context()
      {
         base.Context();
         _subject = new IndividualSimulation();
      }

      protected override void Because()
      {
         sut.GetOrCreateClassifiableFor<ClassifiableSimulation, Simulation>(_subject);
      }

      [Observation]
      public void the_classifiable_should_be_created_and_added_to_the_project()
      {
         sut.AllClassifiables.FindById(_subject.Id).ShouldNotBeNull();
      }
   }

   public class When_retrieving_existing_classifiables_from_project : concern_for_Project
   {
      private IndividualSimulation _subject;
      private ClassifiableSimulation _result;
      private ClassifiableSimulation _classifiableSimulation;

      protected override void Context()
      {
         base.Context();
         const string id = "myId";
         _classifiableSimulation = new ClassifiableSimulation{Id = id};
         sut.AddClassifiable(_classifiableSimulation);
         _subject = new IndividualSimulation{Id = id, Name = "name"};
         
      }

      protected override void Because()
      {
         _result = sut.GetOrCreateClassifiableFor<ClassifiableSimulation, Simulation>(_subject);
      }

      [Observation]
      public void the_returned_classifiable_should_be_the_existing_classifiable_from_project()
      {
         _result.ShouldBeEqualTo(_classifiableSimulation);
      }
   }

   public class When_retrieving_classifications_with_same_path : concern_for_Project
   {
      private IClassification _result, _parent, _firstResult;

      [Observation]
      public void returns_existing_object()
      {
         _result.ShouldBeEqualTo(_firstResult);
      }

      protected override void Because()
      {
         _result = sut.GetOrCreateByPath(_parent, "NewPath",ClassificationType.ObservedData);
      }

      protected override void Context()
      {
         base.Context();
         _parent = new Classification{Name = "Parent"};
         sut.AddClassification(_parent);
         _firstResult = sut.GetOrCreateByPath(_parent, "NewPath", ClassificationType.ObservedData);
         _firstResult.Parent = _parent;
      }
   }

   public class When_removing_a_classification_to_the_project_that_does_not_exist : concern_for_Project
   {
      private IClassification _classification1;

      protected override void Context()
      {
         base.Context();
         _classification1 = A.Fake<IClassification>();
         A.CallTo(() => _classification1.Path).Returns("Path");
      }

      [Observation]
      public void should_not_crash()
      {
         sut.RemoveClassification(_classification1);
      }
   }

   public class When_removing_an_existing_classification_from_the_project : concern_for_Project
   {
      private IClassification _classification1;

      protected override void Context()
      {
         base.Context();
         _classification1 = A.Fake<IClassification>();
         sut.AddClassification(_classification1);
      }

      protected override void Because()
      {
         sut.RemoveClassification(_classification1);
      }

      [Observation]
      public void should_remove_the_classification()
      {
         sut.AllClassifications.ShouldBeEmpty();
      }
   }
}
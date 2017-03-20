using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_Container : ContextSpecification<IContainer>
   {
      protected IParameter _param1;
      protected IParameter _param2;
      protected IEntity _oneEntityWhichIsNotAParameter;

      protected override void Context()
      {
         _param1 = A.Fake<IParameter>();
         _param1.Id = "_param1";
         _param1.Name = "_param1";
         _param2 = A.Fake<IParameter>();
         _param2.Id = "_param2";
         _param2.Name = "_param2";
         _oneEntityWhichIsNotAParameter = A.Fake<IEntity>();
         _oneEntityWhichIsNotAParameter.Id = "_oneEntityWhichIsNotAParameter";
         _oneEntityWhichIsNotAParameter.Name = "_oneEntityWhichIsNotAParameter";
         sut = new Container();
         sut.Name = "toto";
         sut.Add(_param1);
         sut.Add(_oneEntityWhichIsNotAParameter);
         sut.Add(_param2);
      }
   }

   
   public class When_resolving_all_parameters_for_a_container : concern_for_Container
   {
      [Observation]
      public void should_return_all_parameters_defined_as_children_in_the_container()
      {
         sut.AllParameters().ShouldOnlyContain(_param1, _param2);
      }
   }

   
   public class When_resolving_all_parameters_names_for_a_container : concern_for_Container
   {
      [Observation]
      public void should_return_all_parameters_defined_as_children_in_the_container()
      {
         sut.AllParameterNames().ShouldOnlyContain(_param1.Name, _param2.Name);
      }
   }

   
   public class When_resolving_a_parameter_by_name : concern_for_Container
   {
      [Observation]
      public void should_return_the_parameter_if_this_parameter_exits()
      {
         sut.Parameter(_param1.Name).ShouldBeEqualTo(_param1);
         sut.Parameter("trakaka").ShouldBeNull();
      }
   }
}
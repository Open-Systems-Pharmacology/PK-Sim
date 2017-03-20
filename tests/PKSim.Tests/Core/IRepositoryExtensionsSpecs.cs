using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;
using PKSim.Core.Repositories;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;


namespace PKSim.Core
{
    public abstract class concern_for_RepositoryExtensions : StaticContextSpecification
    {
        protected IRepository<IObjectBase> _repository;
        protected IObjectBase _object1;
        protected IObjectBase _object2;

        protected override void Context()
        {
            _repository = A.Fake<IRepository<IObjectBase>>();
            _object1 = A.Fake<IObjectBase>();
            _object1.Name = "tutu";
            _object2 = A.Fake<IObjectBase>();
            _object2.Name = "titi";
            A.CallTo(() => _repository.All()).Returns(new[] { _object1, _object2 });
        }
    }

    
    public class When_selecting_the_first_item_fullfiling_a_criteria : concern_for_RepositoryExtensions
    {
        private IObjectBase _result;

        protected override void Because()
        {
            _result = _repository.SelectFirst(item => item.Name.Equals("tutu")); 
        }

        [Observation]
        public void should_return_a_valid_object()
        {
            _result.ShouldBeEqualTo(_object1);
        }

    }

    
    public class When_selecting_the_items_fullfiling_a_criteria : concern_for_RepositoryExtensions
    {
        private IEnumerable<IObjectBase> _result;

        protected override void Because()
        {
            _result = _repository.Where(item => item.Name.Equals("tutu")); 
        }

        [Observation]
        public void should_return_a_valid_object()
        {
            _result.ShouldOnlyContainInOrder(_object1);
        }

    }
}
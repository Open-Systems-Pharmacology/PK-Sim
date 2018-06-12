using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
    public abstract class concern_for_RepresentationInfoRepository : ContextForIntegration<IRepresentationInfoRepository>
    {
    }

    
    public class when_retrieving_all_info_from_repository : concern_for_RepresentationInfoRepository
    {
        protected IEnumerable<RepresentationInfo> _result;

        protected override void Because()
        {
            _result = sut.All();
        }

        [Observation]
        public void should_return_at_least_one_element()
        {
            _result.Count().ShouldBeGreaterThan(0);
        }

        [Observation]
        public void should_return_object_by_type_and_name()
        {
            var firstObject = _result.FirstOrDefault();
            var objectByTypeAndName = sut.InfoFor(firstObject.ObjectType, firstObject.Name);

            firstObject.ShouldBeEqualTo(objectByTypeAndName);
        }

        [Observation]
        public void should_return_null_for_invalid_name()
        {
            var firstObject = _result.FirstOrDefault();
            sut.InfoFor(firstObject.ObjectType, "fgfjgsjfgsjfgsjfg").Name.ShouldBeNull();
        }
    }

    
    public class when_retrieving_info_of_type_from_repository : concern_for_RepresentationInfoRepository
    {
        protected IList<RepresentationObjectType> _objectTypes = new List<RepresentationObjectType>();
        protected IEnumerable<RepresentationInfo> _result;

        protected override void Because()
        {
            _result = sut.All();

            //get types of all objects that are used in repository
            foreach (var obj in _result)
                if (!_objectTypes.Contains(obj.ObjectType))
                    _objectTypes.Add(obj.ObjectType);
        }

        [Observation]
        public void number_of_elements_of_all_types_must_be_equal_total_elemenst_count()
        {
            int typedObjectsCount = 0;

            foreach (var objectType in _objectTypes)
                typedObjectsCount += sut.AllOfType(objectType).Count();

            typedObjectsCount.ShouldBeEqualTo(_result.Count());
        }
    }

    
    public class when_retrieving_info_for_objectbase_from_repository : concern_for_RepresentationInfoRepository
    {
        protected Compartment _comp = new Compartment();
        protected Organ _organ = new Organ();
        protected Species _species = new Species { Name = "Human" };
        protected IParameter _param = new PKSimParameter();
        protected IDistributedParameter _distrParam = new PKSimDistributedParameter();
        protected IEnumerable<RepresentationInfo> _result;

        protected override void Because()
        {
            _result = sut.All();

            _comp.Name = "Plasma";
            _organ.Name = "Liver";
            _species.Name = "Human";
            _param.Name = "Q";
            _distrParam.Name = "Volume";
        }

        [Observation]
        public void should_return_reprinfo_for_compartment()
        {
            sut.InfoFor(_comp).DisplayName.ShouldNotBeNull();
        }

        [Observation]
        public void should_return_reprinfo_for_organ()
        {
            sut.InfoFor(_organ).DisplayName.ShouldNotBeNull();
        }

        [Observation]
        public void should_return_reprinfo_for_species()
        {
            sut.InfoFor(_species).DisplayName.ShouldNotBeNull();
        }


        [Observation]
        public void should_return_reprinfo_for_parameter()
        {
            sut.InfoFor(_param).DisplayName.ShouldNotBeNull();
        }

        [Observation]
        public void should_return_reprinfo_for_distributed_parameter()
        {
            sut.InfoFor(_distrParam).DisplayName.ShouldNotBeNull();
        }
    }
}
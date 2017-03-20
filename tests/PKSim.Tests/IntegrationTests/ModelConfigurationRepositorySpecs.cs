using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Mappers;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.IntegrationTests
{
    public abstract class concern_for_ModelConfigurationRepository : ContextForIntegration<IModelConfigurationRepository>
    {
    }

    
    public class when_getting_models_from_repository : concern_for_ModelConfigurationRepository
    {
       private IEnumerable<ModelConfiguration> _allModels;

        protected override void Because()
        {
            _allModels = sut.All();
            _allModels = sut.All();
        }

        [Observation]
        public void should_return_at_least_one_model()
        {
            _allModels.Count().ShouldBeGreaterThan(0);
        }
    }

    
    public class when_getting_models_for_species : concern_for_ModelConfigurationRepository
    {
       protected IEnumerable<ModelConfiguration> _modelsForHuman;
       protected IEnumerable<ModelConfiguration> _modelsForInvalidSpecies;
       protected ModelConfiguration _defaultForHuman;
        private IEnumerable<ModelConfiguration> _allModels;

        protected override void Because()
        {
           _allModels = sut.All().Where(mc => mc.SpeciesName.Equals("Human"));

            var human = new Species().WithName("Human");
            _modelsForHuman = sut.AllFor(human);
            _defaultForHuman = sut.DefaultFor(human);

            _modelsForInvalidSpecies = sut.AllFor(new Species().WithName("ET"));
        }

        [Observation]
        public void should_retrieve_all_models_for_human()
        {
            _modelsForHuman.Count().ShouldBeEqualTo(_allModels.Count());
        }

        [Observation]
        public void should_retrieve_no_models_for_invalid_species()
        {
            _modelsForInvalidSpecies.Count().ShouldBeEqualTo(0);
        }

        [Observation]
        public void should_retrieve_default_model()
        {
            _modelsForHuman.ShouldContain(_defaultForHuman);
        }
    }
}	
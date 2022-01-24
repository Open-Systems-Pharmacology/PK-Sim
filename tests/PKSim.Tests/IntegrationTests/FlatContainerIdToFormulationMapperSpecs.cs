using System.Linq;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_FlatContainerIdToFormulationMapper : ContextForIntegration<IFlatContainerIdToFormulationMapper>
   {
      protected IFlatContainerRepository _flatContainerRepository;

      protected override void Context()
      {
         sut = IoC.Resolve<IFlatContainerIdToFormulationMapper>();
         _flatContainerRepository = IoC.Resolve<IFlatContainerRepository>();
      }
   }

   
   public class When_mapping_a_formulation_container_id_to_a_formulation : concern_for_FlatContainerIdToFormulationMapper
   {
      private FlatContainer _formulationFlatContainer;
      private  PKSim.Core.Model.Formulation _formulation;

      protected override void Context()
      {
         base.Context();
         _formulationFlatContainer = _flatContainerRepository.All().First(x => x.Type == CoreConstants.ContainerType.FORMULATION);
      }
      protected override void Because()
      {
         _formulation = sut.MapFrom(_formulationFlatContainer);
      }

      [Observation]
      public void should_return_a_formulation_whose_type_was_set_to_the_flat_container_name()
      {
         _formulation.FormulationType.ShouldBeEqualTo(_formulationFlatContainer.Name);
      }
   }
}	
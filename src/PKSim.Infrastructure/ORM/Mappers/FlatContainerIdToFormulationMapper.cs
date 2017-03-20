using System.Linq;
using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatContainerIdToFormulationMapper : IFlatContainerIdToContainerMapperSpecification, IMapper<FlatContainer, Formulation>
   {
   }

   public class FlatContainerIdToFormulationMapper : IFlatContainerIdToFormulationMapper
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IFormulationRouteRepository _formulationRouteRepo;
      private readonly IFlatContainerRepository _flatContainerRepository;

      public FlatContainerIdToFormulationMapper(IObjectBaseFactory objectBaseFactory, IFormulationRouteRepository formulationRouteRepo, IFlatContainerRepository flatContainerRepository)
      {
         _objectBaseFactory = objectBaseFactory;
         _formulationRouteRepo = formulationRouteRepo;
         _flatContainerRepository = flatContainerRepository;
      }

      public IContainer MapFrom(FlatContainerId flatContainerId)
      {
         return MapFrom(_flatContainerRepository.ContainerFrom(flatContainerId.Id));
      }

      public bool IsSatisfiedBy(PKSimContainerType item)
      {
         return item == PKSimContainerType.Formulation;
      }

      public Formulation MapFrom(FlatContainer flatContainer)
      {
         var formulation = _objectBaseFactory.Create<Formulation>();
         formulation.Root = _objectBaseFactory.Create<IRootContainer>();
         formulation.Name = flatContainer.Name;

         //save formulation name as formulation type too,
         //because name can be overwritten by user
         formulation.FormulationType = formulation.Name;
         foreach (var formulationRoute in _formulationRouteRepo.All().Where(fr => fr.Formulation.Equals(formulation.Name)))
         {
            formulation.AddRoute(formulationRoute.Route);
         }
         return formulation;
      }
   }
}
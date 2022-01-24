using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class FormulationRepository : StartableRepository<Formulation>, IFormulationRepository
   {
      private readonly IFlatContainerRepository _flatContainerRepository;
      private readonly IFlatContainerToFormulationMapper _formulationMapper;
      private readonly IFormulationRouteRepository _formulationRouteRepository;

      private readonly ICache<string, Formulation> _formulations;

      public FormulationRepository(IFlatContainerRepository flatContainerRepository,
                                   IFlatContainerToFormulationMapper formulationMapper,
                                   IFormulationRouteRepository formulationRouteRepository)
      {
         _flatContainerRepository = flatContainerRepository;
         _formulationMapper = formulationMapper;
         _formulationRouteRepository = formulationRouteRepository;
         _formulations = new Cache<string, Formulation>(f => f.FormulationType);
      }

      public override IEnumerable<Formulation> All()
      {
         Start();
         return _formulations;
      }

      protected override void DoStart()
      {
         var allFormulationsContainer = _flatContainerRepository.All().Where(ContainerIsFormulation).ToList();

         // loop over FORMULATION ROUTES in order to get formulations in proper sequence
         foreach (var formulationRoute in _formulationRouteRepository.All())
         {
            string formulationName = formulationRoute.Formulation;
            var flatFormulations = allFormulationsContainer.Where(f => f.Name.Equals(formulationName)).ToList();

            if (!flatFormulations.Any())
               continue;

            var flatFormulation = flatFormulations.First();

            var formulation = _formulationMapper.MapFrom(flatFormulation);

            // formulation may appear more than once
            if (_formulations.Contains(formulation.FormulationType))
               continue;

            _formulations.Add(formulation);
         }
      }

      public bool ContainerIsFormulation(FlatContainer container)
      {
         return string.Equals(container.Type, CoreConstants.ContainerType.FORMULATION) && container.Visible;
      }

      public IEnumerable<Formulation> AllFor(string applicationRoute)
      {
         return string.IsNullOrEmpty(applicationRoute)
                   ? All()
                   : All().Where(f => f.HasRoute(applicationRoute));
      }

      public Formulation DefaultFormulationFor(string applicationRoute)
      {
         return AllFor(applicationRoute).FirstOrDefault();
      }

      public Formulation FormulationBy(string formulationType)
      {
         Start();
         return _formulations[formulationType];
      }
   }
}
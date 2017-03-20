using System.Linq;
using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatSpeciesToSpeciesMapper : IMapper<FlatSpecies, Species>
   {
   }

   public class FlatSpeciesToSpeciesMapper : IFlatSpeciesToSpeciesMapper
   {
      private readonly IPopulationRepository _populationRepository;
      private readonly IFlatParameterValueVersionRepository _flatParameterValueVersionRepository;
      private readonly IFlatPVVListToPVVCategoryListMapper _pvvCategoriesMapper;

      public FlatSpeciesToSpeciesMapper(IPopulationRepository populationRepository,
                                        IFlatParameterValueVersionRepository flatParameterValueVersionRepository,
                                        IFlatPVVListToPVVCategoryListMapper pvvCategoriesMapper)
      {
         _populationRepository = populationRepository;
         _flatParameterValueVersionRepository = flatParameterValueVersionRepository;
         _pvvCategoriesMapper = pvvCategoriesMapper;
      }

      public Species MapFrom(FlatSpecies flatSpecies)
      {
         var species = new Species { Id=flatSpecies.Id, Name = flatSpecies.Id, Icon = flatSpecies.IconName };

         foreach (var population in _populationRepository.All().Where(pop => pop.Species == flatSpecies.Id).OrderBy(x => x.Sequence))
            species.AddPopulation(population);

         foreach (var pvvCategory in _pvvCategoriesMapper.MapFrom(_flatParameterValueVersionRepository.All(), flatSpecies.Id))
            species.AddPVVCategory(pvvCategory);

         return species;
      }
   }
}
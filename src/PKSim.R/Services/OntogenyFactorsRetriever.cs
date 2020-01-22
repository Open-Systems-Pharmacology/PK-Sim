using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Maths.Interpolations;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.R.Domain;

namespace PKSim.R.Services
{
   public interface IOntogenyFactorsRetriever
   {
      /// <summary>
      ///    Returns the ontogeny factor for each molecule
      ///    For instance, for CYP3A4, the call will return CYP3A4|Ontogeny Factor and CYP3A4|Ontogeny Factor GI
      /// </summary>
      IReadOnlyList<ParameterValue> FactorsFor(OriginData originData, IEnumerable<MoleculeOntogeny> moleculeOntogenies);

      IReadOnlyList<DistributedParameterValue> DistributionFactorsFor(OriginData originData, IEnumerable<MoleculeOntogeny> moleculeOntogenies);
   }

   public class OntogenyFactorsRetriever : IOntogenyFactorsRetriever
   {
      private readonly IOntogenyRepository _ontogenyRepository;

      public OntogenyFactorsRetriever(IOntogenyRepository ontogenyRepository)
      {
         _ontogenyRepository = ontogenyRepository;
      }

      public IReadOnlyList<ParameterValue> FactorsFor(OriginData originData, IEnumerable<MoleculeOntogeny> moleculeOntogenies)
      {
         var allOntogenyFactors = new List<ParameterValue>();

         var allOntogeniesForSpecies = _ontogenyRepository.AllFor(originData.Species.Name);
         if (!allOntogeniesForSpecies.Any())
            return allOntogenyFactors;

         //we have ontogeny for the given species! Let's now iterate over the containers to retrieve the possible ontogeny factor
         foreach (var moleculeOntogeny in moleculeOntogenies)
         {
            var ontogeny = allOntogeniesForSpecies.FindByName(moleculeOntogeny.Ontogeny);
            if (ontogeny == null) continue;

            allOntogenyFactors.Add(ontogenyFactorFor(ontogeny, moleculeOntogeny.Molecule, originData, CoreConstants.Parameters.ONTOGENY_FACTOR, CoreConstants.Groups.ONTOGENY_LIVER));
            allOntogenyFactors.Add(ontogenyFactorFor(ontogeny, moleculeOntogeny.Molecule, originData, CoreConstants.Parameters.ONTOGENY_FACTOR_GI, CoreConstants.Groups.ONTOGENY_DUODENUM));
         }

         return allOntogenyFactors;
      }

      public IReadOnlyList<DistributedParameterValue> DistributionFactorsFor(OriginData originData, IEnumerable<MoleculeOntogeny> moleculeOntogenies)
      {
         var allOntogenyDistributions = new List<DistributedParameterValue>();

         var allOntogeniesForSpecies = _ontogenyRepository.AllFor(originData.Species.Name);
         if (!allOntogeniesForSpecies.Any())
            return allOntogenyDistributions;

         //we have ontogeny for the given species! Let's now iterate over the containers to retrieve the possible ontogeny factor
         foreach (var moleculeOntogeny in moleculeOntogenies)
         {
            var ontogeny = allOntogeniesForSpecies.FindByName(moleculeOntogeny.Ontogeny);
            if (ontogeny == null) continue;

            allOntogenyDistributions.Add(_ontogenyRepository.OntogenyParameterDistributionFor(ontogeny, originData, CoreConstants.Groups.ONTOGENY_LIVER, new ObjectPath {moleculeOntogeny.Molecule, CoreConstants.Parameters.ONTOGENY_FACTOR}));
            allOntogenyDistributions.Add(_ontogenyRepository.OntogenyParameterDistributionFor(ontogeny, originData, CoreConstants.Groups.ONTOGENY_DUODENUM, new ObjectPath {moleculeOntogeny.Molecule, CoreConstants.Parameters.ONTOGENY_FACTOR_GI}));
         }

         return allOntogenyDistributions;
      }

      private ParameterValue ontogenyFactorFor(Ontogeny ontogeny, string moleculeName, OriginData originData, string parameterName, string ontogenyLocation)
      {
         var path = new ObjectPath {moleculeName, parameterName};
         double factor = _ontogenyRepository.OntogenyFactorFor(ontogeny, ontogenyLocation, originData);
         return new ParameterValue(path, factor, CoreConstants.DEFAULT_PERCENTILE);
      }
   }
}
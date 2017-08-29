using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Matlab
{
   public class MoleculeOntogeny
   {
      /// <summary>
      ///    Name of the molecule being used in the model
      /// </summary>
      public string Molecule { get; private set; }

      /// <summary>
      ///    Name of the ontogeny associated with the molecule
      /// </summary>
      public string Ontogeny { get; private set; }

      public MoleculeOntogeny(string moleculeName, string ontogenyName)
      {
         Molecule = moleculeName;
         Ontogeny = ontogenyName;
      }
   }

   public interface IOntogenyFactorsRetriever
   {
      /// <summary>
      ///    returns an enumeration containing the ontogeny factor for each molecule
      ///    For instance, for CYP3A4, the call will return CYP3A4|Ontogeny Factor and CYP3A4|Ontogeny Factor GI
      /// </summary>
      IEnumerable<ParameterValue> FactorsFor(Core.Model.OriginData originData, IEnumerable<MoleculeOntogeny> moleculeOntogenies);
   }

   public class OntogenyFactorsRetriever : IOntogenyFactorsRetriever
   {
      private readonly IOntogenyRepository _ontogenyRepository;

      public OntogenyFactorsRetriever(IOntogenyRepository ontogenyRepository)
      {
         _ontogenyRepository = ontogenyRepository;
      }

      public IEnumerable<ParameterValue> FactorsFor(Core.Model.OriginData originData, IEnumerable<MoleculeOntogeny> moleculeOntogenies)
      {
         var allOntogenyFactors = new List<ParameterValue>();

         var allOntogeniesForSpecies = _ontogenyRepository.AllFor(originData.Species.Name).ToList();
         if (!allOntogeniesForSpecies.Any())
            return allOntogenyFactors;

         //we have ontogeny for the given species! Let's now iterate over the containers to retrieve the possible ontogeny factor
         foreach (var moleculeOntogeny in moleculeOntogenies)
         {
            var ontogeny = allOntogeniesForSpecies.FindByName(moleculeOntogeny.Ontogeny);
            if (ontogeny == null) continue;

            allOntogenyFactors.Add(ontogenyFactorFor(ontogeny, moleculeOntogeny.Molecule, originData, CoreConstants.Parameter.ONTOGENY_FACTOR, CoreConstants.Groups.ONTOGENY_LIVER));
            allOntogenyFactors.Add(ontogenyFactorFor(ontogeny, moleculeOntogeny.Molecule, originData, CoreConstants.Parameter.ONTOGENY_FACTOR_GI, CoreConstants.Groups.ONTOGENY_DUODENUM));
         }

         return allOntogenyFactors;
      }

      private ParameterValue ontogenyFactorFor(Ontogeny ontogeny, string moleculeName, Core.Model.OriginData originData, string parameterName, string ontogenyLocation)
      {
         var path = new ObjectPath {moleculeName, parameterName};
         double factor = _ontogenyRepository.OntogenyFactorFor(ontogeny, ontogenyLocation, originData);
         return new ParameterValue(path, factor, CoreConstants.DEFAULT_PERCENTILE);
      }
   }
}
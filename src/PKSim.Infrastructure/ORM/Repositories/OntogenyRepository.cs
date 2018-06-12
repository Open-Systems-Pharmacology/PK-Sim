using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Maths.Interpolations;
using OSPSuite.Core.Maths.Random;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class OntogenyRepository : StartableRepository<Ontogeny>, IOntogenyRepository
   {
      private readonly IFlatOntogenyRepository _flatOntogenyRepository;
      private readonly IInterpolation _interpolation;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly List<Ontogeny> _allOntogenies;
      private readonly ICache<CompositeKey, IReadOnlyList<OntogenyMetaData>> _ontogenyValues;
      private readonly IDimension _ageInWeeksDimension;
      public ICache<string, string> SupportedProteins { get; private set; }

      public OntogenyRepository(IFlatOntogenyRepository flatOntogenyRepository, IInterpolation interpolation, IObjectBaseFactory objectBaseFactory, IDimensionRepository dimensionRepository)
      {
         _flatOntogenyRepository = flatOntogenyRepository;
         _interpolation = interpolation;
         _objectBaseFactory = objectBaseFactory;
         _allOntogenies = new List<Ontogeny>();
         _ontogenyValues = new Cache<CompositeKey, IReadOnlyList<OntogenyMetaData>>(onMissingKey: x => new List<OntogenyMetaData>());
         _ageInWeeksDimension = dimensionRepository.AgeInWeeks;
         SupportedProteins = new Cache<string, string>
         {
            {CoreConstants.Parameters.ONTOGENY_FACTOR_AGP, CoreConstants.Molecule.AGP},
            {CoreConstants.Parameters.ONTOGENY_FACTOR_ALBUMIN, CoreConstants.Molecule.Albumin}
         };
      }


      public override IEnumerable<Ontogeny> All()
      {
         Start();
         return _allOntogenies;
      }

      protected override void DoStart()
      {
         var moleculeWithDefinedOntogenies = (from flatOntogeny in _flatOntogenyRepository.All()
            select new
            {
               flatOntogeny.MoleculeName,
               flatOntogeny.SpeciesName,
               flatOntogeny.DisplayName,
            }
            ).Distinct();

         foreach (var moleculeWithOntogeny in moleculeWithDefinedOntogenies)
         {
            var newOntogeny = _objectBaseFactory.Create<DatabaseOntogeny>().WithName(moleculeWithOntogeny.MoleculeName);
            newOntogeny.SpeciesName = moleculeWithOntogeny.SpeciesName;
            newOntogeny.DisplayName = moleculeWithOntogeny.DisplayName;
            _allOntogenies.Add(newOntogeny);

            //cache ontogeny values using {Enzyme name, species name} as key
            var ontogeny = moleculeWithOntogeny;

            var allOntogenies = from flatOntogeny in _flatOntogenyRepository.All()
               where flatOntogeny.MoleculeName.Equals(ontogeny.MoleculeName)
               where flatOntogeny.SpeciesName.Equals(ontogeny.SpeciesName)
               select flatOntogeny;

            _ontogenyValues.Add(ontogenyKey(ontogeny.MoleculeName, ontogeny.SpeciesName), allOntogenies.ToList());
         }
      }

      public IReadOnlyList<Ontogeny> AllFor(string speciesName)
      {
         return allFor(speciesName).Where(x => !x.NameIsOneOf(SupportedProteins)).ToList();
      }

      private IReadOnlyList<Ontogeny> allFor(string speciesName)
      {
         return All().Where(o => o.SpeciesName.Equals(speciesName)).ToList();
      }

      public double OntogenyFactorFor(Ontogeny ontogeny, string containerName, OriginData originData, RandomGenerator randomGenerator = null)
      {
         return OntogenyFactorFor(ontogeny, containerName, originData.Age, originData.GestationalAge, randomGenerator);
      }

      public double OntogenyFactorFor(Ontogeny ontogeny, string containerName, double? age, double? gestationalAge, RandomGenerator randomGenerator)
      {
         var allOntogenies = AllValuesFor(ontogeny, containerName);
         if (!allOntogenies.Any())
            return CoreConstants.DEFAULT_ONTOGENY_FACTOR;

         var factorRetriever = ontogenyFactorRetriever(randomGenerator, allOntogenies);
         return _interpolation.Interpolate(allOntogenies.Select(x => new Sample(x.PostmenstrualAge, factorRetriever(x))), postmenstrualAgeInYearsFor(age, gestationalAge));
      }

      public double PlasmaProteinOntogenyFactor(string protein, OriginData originData, RandomGenerator randomGenerator = null)
      {
         return PlasmaProteinOntogenyFactor(protein, originData.Age, originData.GestationalAge, originData.Species.Name, randomGenerator);
      }

      public double PlasmaProteinOntogenyFactor(string protein, double? age, double? gestationalAge, string species, RandomGenerator randomGenerator)
      {
         var ontogeny = allFor(species).FindByName(protein) ?? new NullOntogeny();
         return OntogenyFactorFor(ontogeny, CoreConstants.Groups.ONTOGENY_PLASMA, age, gestationalAge, randomGenerator);
      }

      private Ontogeny forMolecule(OriginData originData, string molecule)
      {
         return allFor(originData.Species.Name).FindByName(molecule) ?? new NullOntogeny();
      }

      public IReadOnlyList<Sample> AllOntogenyFactorForStrictBiggerThanPMA(Ontogeny ontogeny, OriginData originData, string containerName, RandomGenerator randomGenerator = null)
      {
         Start();

         if (ontogeny.IsUndefined())
            return new List<Sample>();

         var pma = postmenstrualAgeInYearsFor(originData);
         var gaInYears = inYears(originData.GestationalAge.GetValueOrDefault(CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS));

         var ontogenies = AllValuesFor(ontogeny, containerName).Where(x => x.PostmenstrualAge > pma).ToList();
         if (!ontogenies.Any())
            return new List<Sample>();

         var factorRetriever = ontogenyFactorRetriever(randomGenerator, ontogenies);
         return ontogenies.Select(x => new Sample(x.PostmenstrualAge - gaInYears, factorRetriever(x))).ToList();
      }

      public IReadOnlyList<Sample> AllPlasmaProteinOntogenyFactorForStrictBiggerThanPMA(string parameterName, OriginData originData, RandomGenerator randomGenerator = null)
      {
         return AllOntogenyFactorForStrictBiggerThanPMA(forMolecule(originData, SupportedProteins[parameterName]), originData, CoreConstants.Groups.ONTOGENY_PLASMA, randomGenerator);
      }

      private Func<OntogenyMetaData, double> ontogenyFactorRetriever(RandomGenerator randomGenerator, IReadOnlyList<OntogenyMetaData> allOntogenies)
      {
         if (randomGenerator == null || !allOntogenies.Any())
            return x => x.OntogenyFactor;

         var percentile = allOntogenies.Select(x => x.RandomizedPercentile(randomGenerator))
            .FirstOrDefault(p => !double.IsNaN(p));

         return x => x.RandomizedFactor(percentile);
      }

      private double postmenstrualAgeInYearsFor(OriginData originData)
      {
         return postmenstrualAgeInYearsFor(originData.Age, originData.GestationalAge);
      }

      private double postmenstrualAgeInYearsFor(double? ageInYears, double? gestationalAge)
      {
         //get interpolated value for age. Age is given in year. 
         var gaOffset = gestationalAge.GetValueOrDefault(CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS);
         return ageInYears.Value + inYears(gaOffset);
      }

      private double inYears(double valueInWeeks)
      {
         return _ageInWeeksDimension.BaseUnitValueToUnitValue(_ageInWeeksDimension.Unit(CoreConstants.Units.Years), valueInWeeks);
      }

      private CompositeKey ontogenyKey(string moleculeName, string speciesName)
      {
         return new CompositeKey(moleculeName, speciesName);
      }

      public IReadOnlyList<OntogenyMetaData> AllValuesFor(Ontogeny ontogeny)
      {
         Start();

         if (ontogeny.IsUndefined())
            return new List<OntogenyMetaData>();

         if (ontogeny.IsUserDefined())
            return userDefinedOntogenyValues(ontogeny);

         return _ontogenyValues[ontogenyKey(ontogeny.Name, ontogeny.SpeciesName)];
      }

      private IReadOnlyList<OntogenyMetaData> userDefinedOntogenyValues(Ontogeny ontogeny)
      {
         var userDefinedOntogeny = ontogeny.DowncastTo<UserDefinedOntogeny>();
         var pmas = userDefinedOntogeny.PostmenstrualAges();
         var ontogenyFactors = userDefinedOntogeny.OntogenyFactors();
         var deviations = userDefinedOntogeny.Deviations();

         return pmas.Select((v, i) => new OntogenyMetaData
         {
            PostmenstrualAge = v,
            OntogenyFactor = ontogenyFactors[i],
            Deviation = deviations[i],
            GroupName = Constants.Groups.UNDEFINED
         }).ToList();
      }

      public IReadOnlyList<OntogenyMetaData> AllValuesFor(Ontogeny ontogeny, string containerName)
      {
         var allValues = AllValuesFor(ontogeny);
         var allGroups = allValues.Select(x => x.GroupName).Distinct().ToList();

         var groupName = ontogenyGroupFor(containerName, allGroups);
         return allValues.Where(x => x.GroupName == groupName).ToList();
      }

      private static string ontogenyGroupFor(string containerName, List<string> allGroups)
      {
         if (allGroups.Count == 1)
            return allGroups[0];

         if (allGroups.Contains(containerName))
            return containerName;

         return CoreConstants.Groups.ONTOGENY_LIVER;
      }
   }
}
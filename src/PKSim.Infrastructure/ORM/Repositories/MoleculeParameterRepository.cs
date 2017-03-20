using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Maths.Random;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;
using OSPSuite.Core.Domain;
using IPKSimParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class MoleculeParameterRepository : StartableRepository<MoleculeParameter>, IMoleculeParameterRepository
   {
      private readonly IFlatMoleculeParameterRepository _flatMoleculeParameterRepository;
      private readonly IPKSimParameterFactory _parameterFactory;
      private readonly IFlatProteinSynonymRepository _proteinSynonymRepository;
      private readonly Cache<string, IReadOnlyList<IDistributedParameter>> _allParametersByMolecule;
      private readonly List<MoleculeParameter> _allMoleculeParameters;

      public MoleculeParameterRepository(IFlatMoleculeParameterRepository flatMoleculeParameterRepository, IPKSimParameterFactory parameterFactory, IFlatProteinSynonymRepository proteinSynonymRepository)
      {
         _flatMoleculeParameterRepository = flatMoleculeParameterRepository;
         _parameterFactory = parameterFactory;
         _proteinSynonymRepository = proteinSynonymRepository;
         _allParametersByMolecule = new Cache<string, IReadOnlyList<IDistributedParameter>>();
         _allMoleculeParameters = new List<MoleculeParameter>();
      }

      protected override void DoStart()
      {
         _flatMoleculeParameterRepository.All().GroupBy(x => x.MoleculeName)
            .Each(addMoleculeParameters);
      }

      private void addMoleculeParameters(IGrouping<string, FlatMoleculeParameter> allParametersForMolecule)
      {
         var allParameters = new List<MoleculeParameter>();
         var moleculeName = allParametersForMolecule.Key;
         allParameters.AddRange(allParametersForMolecule.Select(x => moleculeParameterFrom(moleculeName, x)));
         _allMoleculeParameters.AddRange(allParameters);
         _allParametersByMolecule.Add(moleculeName, allParameters.Select(x => x.Parameter).ToList());
      }

      private MoleculeParameter moleculeParameterFrom(string moleculeName, FlatMoleculeParameter flatMoleculeParameter)
      {
         var moleculeParameter = new MoleculeParameter
         {
            MoleculeName = moleculeName,
            Parameter = _parameterFactory.CreateFor(flatMoleculeParameter)
         };
         _proteinSynonymRepository.AddSynonymsTo(moleculeParameter);
         return moleculeParameter;
      }

      public override IEnumerable<MoleculeParameter> All()
      {
         Start();
         return _allMoleculeParameters;
      }

      public IReadOnlyList<IDistributedParameter> AllParametersFor(string moleculeName)
      {
         Start();
         var moleculeParameter = _allMoleculeParameters.FirstOrDefault(x => x.IsTemplateFor(moleculeName));
         if (moleculeParameter == null)
            return new List<IDistributedParameter>();

         return _allParametersByMolecule[moleculeParameter.MoleculeName];
      }

      public IDistributedParameter ParameterFor(string moleculeName, string parameterName)
      {
         return AllParametersFor(moleculeName).FindByName(parameterName);
      }

      public double ParameterValueFor(string moleculeName, string parameterName, double? defaultValue = double.NaN, RandomGenerator randomGenerator = null)
      {
         var parameter = ParameterFor(moleculeName, parameterName);
         if (parameter == null)
            return defaultValue.GetValueOrDefault(double.NaN);

         return randomGenerator == null ? parameter.Value : parameter.RandomDeviateIn(randomGenerator);
      }
   }
}
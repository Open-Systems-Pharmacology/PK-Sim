using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using static PKSim.Core.CoreConstants.Groups;
using static PKSim.Core.CoreConstants.Parameter;
using SnapshotCompound = PKSim.Core.Snapshots.Compound;
using ModelCompound = PKSim.Core.Model.Compound;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CompoundMapper : ParameterContainerSnapshotMapperBase<ModelCompound, SnapshotCompound>
   {
      private readonly AlternativeMapper _alternativeMapper;
      private readonly CalculationMethodCacheMapper _calculationMethodCacheMapper;
      private readonly CompoundProcessMapper _processMapper;
      private readonly ICompoundFactory _compoundFactory;

      public CompoundMapper(
         ParameterMapper parameterMapper,
         AlternativeMapper alternativeMapper,
         CalculationMethodCacheMapper calculationMethodCacheMapper,
         CompoundProcessMapper processMapper,
         ICompoundFactory compoundFactory) : base(parameterMapper)
      {
         _alternativeMapper = alternativeMapper;
         _calculationMethodCacheMapper = calculationMethodCacheMapper;
         _processMapper = processMapper;
         _compoundFactory = compoundFactory;
      }

      public override async Task<SnapshotCompound> MapToSnapshot(ModelCompound compound)
      {
         var snapshot = await SnapshotFrom(compound);
         snapshot.CalculationMethods = await _calculationMethodCacheMapper.MapToSnapshot(compound.CalculationMethodCache);
         snapshot.Lipophilicity = await mapAlternatives(compound, COMPOUND_LIPOPHILICITY);
         snapshot.FractionUnbound = await mapAlternatives(compound, COMPOUND_FRACTION_UNBOUND);
         snapshot.Solubility = await mapAlternatives(compound, COMPOUND_SOLUBILITY);
         snapshot.IntestinalPermeability = await mapAlternatives(compound, COMPOUND_INTESTINAL_PERMEABILITY);
         snapshot.Permeability = await mapAlternatives(compound, COMPOUND_PERMEABILITY);
         snapshot.PkaTypes = mapPkaTypes(compound);
         snapshot.Processes = await mapProcesses(compound);
         snapshot.IsSmallMolecule = compound.IsSmallMolecule;
         snapshot.PlasmaProteinBindingPartner = compound.PlasmaProteinBindingPartner.ToString();
         return snapshot;
      }

      public override async Task<ModelCompound> MapToModel(SnapshotCompound snapshot)
      {
         var compound = _compoundFactory.Create();
         MapSnapshotPropertiesToModel(snapshot, compound);
         _calculationMethodCacheMapper.UpdateCalculationMethodCache(compound, snapshot.CalculationMethods);

         await updateAlternatives(compound, snapshot.Lipophilicity, COMPOUND_LIPOPHILICITY);
         await updateAlternatives(compound, snapshot.FractionUnbound, COMPOUND_FRACTION_UNBOUND);
         await updateAlternatives(compound, snapshot.Solubility, COMPOUND_SOLUBILITY);
         await updateAlternatives(compound, snapshot.IntestinalPermeability, COMPOUND_INTESTINAL_PERMEABILITY);
         await updateAlternatives(compound, snapshot.Permeability, COMPOUND_PERMEABILITY);

         updatePkaTypes(compound, snapshot);

         var tasks = snapshot.Processes.Select(_processMapper.MapToModel);
         compound.AddChildren(await Task.WhenAll(tasks));
         compound.IsSmallMolecule = snapshot.IsSmallMolecule;
         compound.PlasmaProteinBindingPartner = EnumHelper.ParseValue<PlasmaProteinBindingPartner>(snapshot.PlasmaProteinBindingPartner);
         await UpdateParametersFromSnapshot(snapshot, compound, PKSimConstants.ObjectTypes.Compound);

         return compound;
      }

      private async Task updateAlternatives(ModelCompound compound, Alternative[] snapshotAlternatives, string alternativeGroupName)
      {
         var alternativeGroup = compound.ParameterAlternativeGroup(alternativeGroupName);

         //Remove all alternatives except calculated ones
         alternativeGroup.AllAlternatives.ToList().Where(x => !x.IsCalculated).Each(alternativeGroup.RemoveAlternative);

         var tasks = snapshotAlternatives.Select(x => _alternativeMapper.MapToModel(x, alternativeGroup));

         alternativeGroup.AddChildren(await Task.WhenAll(tasks));
      }

      private void updatePkaTypes(ModelCompound compound, SnapshotCompound snapshot)
      {
         snapshot.PkaTypes.Each((pkaType, i) =>
         {
            var compoundType = EnumHelper.ParseValue<CompoundType>(pkaType.Type);
            compound.Parameter(ParameterCompoundType(i)).Value = (int) compoundType;
            compound.Parameter(ParameterPKa(i)).Value = pkaType.Pka;
         });
      }

      private Task<CompoundProcess[]> mapProcesses(ModelCompound compound)
      {
         var tasks= compound.AllProcesses().Select(_processMapper.MapToSnapshot);
         return Task.WhenAll(tasks);
      }

      private PkaType[] mapPkaTypes(ModelCompound compound)
      {
         var pkaTypes = new List<PkaType>();

         for (int i = 0; i < CoreConstants.NUMBER_OF_PKA_PARAMETERS; i++)
         {
            var compoundTypeParameter = compound.Parameter(ParameterCompoundType(i));
            var pkA = compound.Parameter(ParameterPKa(i)).Value;
            var compoundType = (CompoundType) compoundTypeParameter.Value;
            if (compoundType == CompoundType.Neutral)
               continue;

            pkaTypes.Add(new PkaType {Pka = pkA, Type = compoundType.ToString()});
         }

         return pkaTypes.ToArray();
      }

      protected override Task AddModelParametersToSnapshot(ModelCompound compound, SnapshotCompound snapshot)
      {
         var parameters = parameterOverwrittenByUserIn(compound);
         return AddParametersToSnapshot(parameters, snapshot);
      }

      private IReadOnlyList<IParameter> parameterOverwrittenByUserIn(ModelCompound compound)
      {
         var parameters = new List<IParameter>();
         //Molecular Weight
         parameters.AddRange(changedGroupParameters(compound, COMPOUND_MW));

         //advanced parameters
         parameters.AddRange(changedGroupParameters(compound, COMPOUND_TWO_PORE));
         parameters.AddRange(changedGroupParameters(compound, COMPOUND_DISSOLUTION));

         return parameters;
      }

      private IEnumerable<IParameter> changedGroupParameters(IContainer container, string groupName)
      {
         return container.AllParameters(x => string.Equals(x.GroupName, groupName))
            .Where(ParameterHasChanged);
      }

      private async Task<Alternative[]> mapAlternatives(ModelCompound compound, string alternativeGroupName)
      {
         var alternativeGroup = compound.ParameterAlternativeGroup(alternativeGroupName);
         var tasks = alternativeGroup.AllAlternatives.Select(_alternativeMapper.MapToSnapshot);
         var alteratives = await Task.WhenAll(tasks);
         return alteratives.Where(x => x != null).ToArray();
      }
   }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using static PKSim.Core.CoreConstants.Groups;
using static PKSim.Core.CoreConstants.Parameters;
using SnapshotCompound = PKSim.Core.Snapshots.Compound;
using ModelCompound = PKSim.Core.Model.Compound;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CompoundMapper : ParameterContainerSnapshotMapperBase<ModelCompound, SnapshotCompound>
   {
      private readonly AlternativeMapper _alternativeMapper;
      private readonly CalculationMethodCacheMapper _calculationMethodCacheMapper;
      private readonly CompoundProcessMapper _processMapper;
      private readonly ValueOriginMapper _valueOriginMapper;
      private readonly ICompoundFactory _compoundFactory;

      public CompoundMapper(
         ParameterMapper parameterMapper,
         AlternativeMapper alternativeMapper,
         CalculationMethodCacheMapper calculationMethodCacheMapper,
         CompoundProcessMapper processMapper,
         ValueOriginMapper valueOriginMapper,
         ICompoundFactory compoundFactory) : base(parameterMapper)
      {
         _alternativeMapper = alternativeMapper;
         _calculationMethodCacheMapper = calculationMethodCacheMapper;
         _processMapper = processMapper;
         _valueOriginMapper = valueOriginMapper;
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
         snapshot.PkaTypes = await mapPkaTypes(compound);
         snapshot.Processes = await mapProcesses(compound);
         snapshot.IsSmallMolecule = compound.IsSmallMolecule;
         snapshot.PlasmaProteinBindingPartner = SnapshotValueFor(compound.PlasmaProteinBindingPartner, PlasmaProteinBindingPartner.Unknown);
         return snapshot;
      }

      public override async Task<ModelCompound> MapToModel(SnapshotCompound snapshot, SnapshotContext snapshotContext)
      {
         var compound = _compoundFactory.Create();
         MapSnapshotPropertiesToModel(snapshot, compound);
         _calculationMethodCacheMapper.UpdateCalculationMethodCache(compound, snapshot.CalculationMethods);

         await updateAlternatives(compound, snapshot.Lipophilicity, COMPOUND_LIPOPHILICITY, snapshotContext);
         await updateAlternatives(compound, snapshot.FractionUnbound, COMPOUND_FRACTION_UNBOUND, snapshotContext);
         await updateAlternatives(compound, snapshot.Solubility, COMPOUND_SOLUBILITY, snapshotContext);
         await updateAlternatives(compound, snapshot.IntestinalPermeability, COMPOUND_INTESTINAL_PERMEABILITY, snapshotContext);
         await updateAlternatives(compound, snapshot.Permeability, COMPOUND_PERMEABILITY, snapshotContext);

         updatePkaTypes(compound, snapshot);

         await updateProcesses(snapshot, compound, snapshotContext);
         await UpdateParametersFromSnapshot(snapshot, compound, snapshotContext);

         synchronizeMolWeightValueOrigins(compound);
         return compound;
      }

      protected override void MapSnapshotPropertiesToModel(SnapshotCompound snapshot, ModelCompound compound)
      {
         base.MapSnapshotPropertiesToModel(snapshot, compound);
         compound.IsSmallMolecule = ModelValueFor(snapshot.IsSmallMolecule, true);
         compound.PlasmaProteinBindingPartner = ModelValueFor(snapshot.PlasmaProteinBindingPartner, PlasmaProteinBindingPartner.Unknown);
      }

      private async Task updateProcesses(SnapshotCompound snapshot, ModelCompound compound, SnapshotContext snapshotContext)
      {
         var processes = await _processMapper.MapToModels(snapshot.Processes, snapshotContext);
         processes?.Each(compound.AddProcess);
      }

      private async Task updateAlternatives(ModelCompound compound, Alternative[] snapshotAlternatives, string alternativeGroupName, SnapshotContext snapshotContext)
      {
         if (snapshotAlternatives == null)
            return;

         var alternativeGroup = compound.ParameterAlternativeGroup(alternativeGroupName);

         //Remove all alternatives except calculated ones
         alternativeGroup.AllAlternatives.ToList().Where(x => !x.IsCalculated).Each(alternativeGroup.RemoveAlternative);

         //Reset the default flag that will be read from snapshot
         alternativeGroup.AllAlternatives.Each(x => x.IsDefault = false);

         var alternativeSnapshotContext = new AlternativeMapperSnapshotContext(alternativeGroup, snapshotContext);
         var alternatives = await _alternativeMapper.MapToModels(snapshotAlternatives, alternativeSnapshotContext);

         alternatives?.Each(alternativeGroup.AddAlternative);

         //Ensure that we have at least one default alternative (might not be the case if only calculated alternatives were saved)
         var defaultAlternative = alternativeGroup.DefaultAlternative;
         if (defaultAlternative != null)
            defaultAlternative.IsDefault = true;
      }

      private void updatePkaTypes(ModelCompound compound, SnapshotCompound snapshot)
      {
         snapshot.PkaTypes?.Each((pkaType, i) => updatePkaType(compound, pkaType, i));
         synchronizePkaValueOrigins(snapshot.PkaTypes?.FirstOrDefault(), compound);
      }

      private void synchronizeMolWeightValueOrigins(ModelCompound compound)
      {
         //Mol weight parameter and halogens value share the same value origin and should be updated as such
         var molWeight = compound.Parameter(MOLECULAR_WEIGHT);
         var halogens = compound.AllParameters(x => x.NameIsOneOf(Halogens));
         halogens.Each(x => x.ValueOrigin.UpdateAllFrom(molWeight.ValueOrigin));
      }

      private void synchronizePkaValueOrigins(PkaType pkaType, ModelCompound compound)
      {
         var valueOrigin = pkaType?.ValueOrigin;
         if (valueOrigin == null)
            return;

         //Making sure that all pKa parameters have the same value origin, even neutral ones
         Enumerable.Range(0, CoreConstants.NUMBER_OF_PKA_PARAMETERS).Each(index =>
         {
            var (compoundTypeParameter, pKaParameter) = pkaParametersFor(compound, index);
            _valueOriginMapper.UpdateValueOrigin(compoundTypeParameter.ValueOrigin, valueOrigin);
            _valueOriginMapper.UpdateValueOrigin(pKaParameter.ValueOrigin, valueOrigin);
         });
      }

      private void updatePkaType(ModelCompound compound, PkaType pkaType, int index)
      {
         var (compoundTypeParameter, pKaParameter) = pkaParametersFor(compound, index);

         compoundTypeParameter.Value = (int) pkaType.Type;
         pKaParameter.Value = pkaType.Pka;
      }

      private (IParameter compoundTypeParameter, IParameter pkaParameter) pkaParametersFor(ModelCompound compound, int index)
      {
         var compoundTypeParameter = compound.Parameter(Constants.Parameters.ParameterCompoundType(index));
         var pkaParameter = compound.Parameter(ParameterPKa(index));
         return (compoundTypeParameter, pkaParameter);
      }

      private Task<CompoundProcess[]> mapProcesses(ModelCompound compound) => _processMapper.MapToSnapshots(compound.AllProcesses());

      private Task<PkaType[]> mapPkaTypes(ModelCompound compound)
      {
         return SnapshotMapperBaseExtensions.MapTo(Enumerable.Range(0, CoreConstants.NUMBER_OF_PKA_PARAMETERS), i => mapPkaType(compound, i));
      }

      private async Task<PkaType> mapPkaType(ModelCompound compound, int index)
      {
         var (compoundTypeParameter, pKaParameter) = pkaParametersFor(compound, index);
         var pKa = pKaParameter.Value;
         var compoundType = (CompoundType) compoundTypeParameter.Value;
         if (compoundType == CompoundType.Neutral)
            return null;

         var valueOrigin = await _valueOriginMapper.MapToSnapshot(pKaParameter.ValueOrigin);

         return new PkaType {Pka = pKa, Type = compoundType, ValueOrigin = valueOrigin};
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
            .Where(ShouldExportParameterToSnapshot);
      }

      private async Task<Alternative[]> mapAlternatives(ModelCompound compound, string alternativeGroupName)
      {
         var alternativeGroup = compound.ParameterAlternativeGroup(alternativeGroupName);
         var alternatives = await _alternativeMapper.MapToSnapshots(alternativeGroup.AllAlternatives);
         var definedAlternatives = alternatives?.ToArray();
         if (definedAlternatives == null || !definedAlternatives.Any())
            return null;

         return definedAlternatives;
      }
   }
}
using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Batch.Mapper;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using SnapshotIndividual = PKSim.Core.Snapshots.Individual;
using ModelIndividual = PKSim.Core.Model.Individual;

namespace PKSim.Core.Snapshots.Mappers
{
   public class IndividualMapper : SnapshotMapperBase<ModelIndividual, SnapshotIndividual>
   {
      private readonly ParameterMapper _parameterMapper;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly MoleculeMapper _moleculeMapper;
      private readonly IIndividualFactory _individualFactory;
      private readonly IOriginDataMapper _originDataMapper;
      private readonly IContainerTask _containerTask;

      public IndividualMapper(ParameterMapper parameterMapper, IDimensionRepository dimensionRepository, MoleculeMapper moleculeMapper, 
         IIndividualFactory individualFactory, IOriginDataMapper originDataMapper, IContainerTask containerTask)
      {
         _parameterMapper = parameterMapper;
         _dimensionRepository = dimensionRepository;
         _moleculeMapper = moleculeMapper;
         _individualFactory = individualFactory;
         _originDataMapper = originDataMapper;
         _containerTask = containerTask;
      }

      public override SnapshotIndividual MapToSnapshot(ModelIndividual individual)
      {
         var snapshot = new SnapshotIndividual();
         MapModelPropertiesToSnapshot(individual, snapshot);
         mapOriginDataToSnapshot(snapshot, individual.OriginData);
         snapshot.Seed = individual.Seed;
         snapshot.Parameters = allParametersChangedByUserFrom(individual);
         snapshot.Molecules = allMoleculesFrom(individual);
         return snapshot;
      }

      private List<Molecule> allMoleculesFrom(ModelIndividual individual)
      {
         var molecules = individual.AllDefinedMolecules();
         return molecules.Select(_moleculeMapper.MapToSnapshot).ToList();
      }

      private void mapOriginDataToSnapshot(SnapshotIndividual snapshot, OriginData originData)
      {
         snapshot.Species = originData.Species.Name;
         snapshot.Population = originData.SpeciesPopulation.Name;
         snapshot.Gender = originData.Gender.Name;
         snapshot.Age = parameterFrom(originData.Age, originData.AgeUnit, _dimensionRepository.AgeInYears);
         snapshot.GestationalAge = parameterFrom(originData.GestationalAge, originData.GestationalAgeUnit, _dimensionRepository.AgeInWeeks);
         snapshot.Height = parameterFrom(originData.Height, originData.HeightUnit, _dimensionRepository.Length);
         snapshot.Weight = parameterFrom(originData.Weight, originData.WeightUnit, _dimensionRepository.Mass);
      }

      private Parameter parameterFrom(double? parameterBaseValue, string parameterDisplayUnit, IDimension dimension)
      {
         return _parameterMapper.ParameterFrom(parameterBaseValue, parameterDisplayUnit, dimension);
      }

      private List<LocalizedParameter> allParametersChangedByUserFrom(ModelIndividual individual)
      {
         var changedParameters = individual.Organism.GetAllChildren<IParameter>(x => x.ValueDiffersFromDefault());
         return _parameterMapper.LocalizedParametersFrom(changedParameters);
      }

      public override ModelIndividual MapToModel(SnapshotIndividual individualSnapshot)
      {
         var originData = createOriginDataFrom(individualSnapshot);
         var individual = _individualFactory.CreateAndOptimizeFor(originData, individualSnapshot.Seed);
         MapSnapshotPropertiesToModel(individualSnapshot, individual);
         updateIndividualParameters(individualSnapshot, individual);
         individual.AddChildren(individualSnapshot.Molecules.Select(x=>_moleculeMapper.MapToModel(x, individual)));
         return individual;
      }

      private void updateIndividualParameters(SnapshotIndividual individualSnapshot, ModelIndividual individual)
      {
         var allParameters = _containerTask.CacheAllChildren<IParameter>(individual.Organism);
         individualSnapshot.Parameters.Each(snapshotParameter =>
         {
            var parameter = allParameters[snapshotParameter.Path];
            if (parameter == null)
               throw new SnapshotParameterNotFoundException(snapshotParameter.Path, individual.Name);

            _parameterMapper.UpdateParameterFromSnapshot(parameter, snapshotParameter);
         });
      }

      private OriginData createOriginDataFrom(Individual individualSnapshot)
      {
         var batchOriginData = new Batch.OriginData
         {
            Species = individualSnapshot.Species,
            Population = individualSnapshot.Population,
            Gender = individualSnapshot.Gender,
            Age = baseParameterValueFrom(individualSnapshot.Age, _dimensionRepository.AgeInYears),
            Height = baseParameterValueFrom(individualSnapshot.Height, _dimensionRepository.Length),
            Weight = baseParameterValueFrom(individualSnapshot.Weight, _dimensionRepository.Mass),
            GestationalAge = baseParameterValueFrom(individualSnapshot.GestationalAge, _dimensionRepository.AgeInWeeks),
         };

         return _originDataMapper.MapFrom(batchOriginData);
      }

      private double baseParameterValueFrom(Parameter parameter, IDimension dimension)
      {
         if (parameter == null)
            return double.NaN;

         var unit = dimension.Unit(parameter.Unit);
         return dimension.UnitValueToBaseUnitValue(unit, parameter.Value);
      }
   }
}
using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
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

      public IndividualMapper(ParameterMapper parameterMapper, IDimensionRepository dimensionRepository, MoleculeMapper moleculeMapper)
      {
         _parameterMapper = parameterMapper;
         _dimensionRepository = dimensionRepository;
         _moleculeMapper = moleculeMapper;
      }

      public override SnapshotIndividual MapToSnapshot(ModelIndividual individual)
      {
         var snapshot = new SnapshotIndividual();
         MapModelPropertiesToSnapshot(individual, snapshot);
         mapOriginDataToSnapshot(snapshot, individual.OriginData);
         snapshot.Seed = individual.Seed;
         snapshot.Parameters = allParametersChangedByUserFrom(individual);
         snapshot.Enzymes = mapMolecules<IndividualEnzyme>(individual);
         snapshot.Transporters = mapMolecules<IndividualTransporter>(individual);
         snapshot.OtherProteins = mapMolecules<IndividualOtherProtein>(individual);
         return snapshot;
      }

      private List<Molecule> mapMolecules<TMolecule>(ModelIndividual individual) where TMolecule : IndividualMolecule
      {
         var molecules = individual.AllDefinedMolecules().OfType<TMolecule>();
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

      public override ModelIndividual MapToModel(SnapshotIndividual snapshot)
      {
         throw new NotImplementedException();
      }
   }
}
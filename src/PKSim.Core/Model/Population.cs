using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Maths.Random;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Repositories;

namespace PKSim.Core.Model
{
   public abstract class Population : PKSimBuildingBlock, ISimulationSubject, IAdvancedParameterContainer
   {
      public virtual ParameterDistributionSettingsCache SelectedDistributions { get; private set; }

      /// <summary>
      ///    The underlying cache managing the individuals and their values
      /// </summary>
      public virtual IndividualValuesCache IndividualValuesCache { get; protected set; }

      private RandomGenerator _randomGenerator;
      private int _seed;

      public virtual RandomGenerator RandomGenerator => _randomGenerator;

      protected Population() : base(PKSimBuildingBlockType.Population)
      {
         IndividualValuesCache = new IndividualValuesCache();
         SelectedDistributions = new ParameterDistributionSettingsCache();
         Seed = Environment.TickCount;
      }

      /// <summary>
      ///    Seed used to generate random values for the population
      /// </summary>
      public int Seed
      {
         get => _seed;
         set
         {
            _seed = value;
            _randomGenerator = new RandomGenerator(Seed);
         }
      }

      public virtual IReadOnlyList<double> AllPercentilesFor(string parameterPath)
      {
         if (IndividualValuesCache.Has(parameterPath))
            return IndividualValuesCache.PercentilesFor(parameterPath);

         return defaultValuesWith(CoreConstants.DEFAULT_PERCENTILE);
      }

      public virtual int NumberOfItems => IndividualValuesCache.Count;

      /// <summary>
      ///    Returns all values defined for the organism parameter names <paramref name="parameterName" />
      /// </summary>
      public virtual IReadOnlyList<double> AllOrganismValuesFor(string parameterName, IEntityPathResolver entityPathResolver)
      {
         if (FirstIndividual == null)
            return defaultValuesWith(double.NaN);

         var parameterPath = entityPathResolver.PathFor(FirstIndividual.Organism.Parameter(parameterName));
         return AllValuesFor(parameterPath);
      }

      public virtual IReadOnlyList<double> AllValuesFor(string parameterPath)
      {
         if (IndividualValuesCache.Has(parameterPath))
            return IndividualValuesCache.ValuesFor(parameterPath);

         return defaultValuesWith(double.NaN);
      }

      private double[] defaultValuesWith(double defaultValue)
      {
         return new double[NumberOfItems].InitializeWith(defaultValue);
      }

      /// <summary>
      ///    Add one individual to the population
      /// </summary>
      /// <param name="individualValues">properties of individual added to population</param>
      public virtual void AddIndividualValues(IndividualValues individualValues)
      {
         IndividualValuesCache.Add(individualValues);
      }

      public virtual IEnumerable<IParameter> AllVectorialParameters(IEntityPathResolver entityPathResolver)
      {
         var allParameters = AllParameters(entityPathResolver);
         return IndividualValuesCache.AllParameterPaths().Select(p => allParameters[p]).Where(p => p != null);
      }

      public virtual void GenerateRandomValuesFor(AdvancedParameter advancedParameter)
      {
         IndividualValuesCache.SetValues(advancedParameter.ParameterPath, advancedParameter.GenerateRandomValues(NumberOfItems));
      }

      public IParameter ParameterByPath(string parameterPath, IEntityPathResolver entityPathResolver)
      {
         return AllParameters(entityPathResolver)[parameterPath];
      }

      public virtual IReadOnlyList<Gender> AllGenders(IGenderRepository genderRepository)
      {
         var genderCovariates = IndividualValuesCache.AllCovariateValuesFor(Constants.Population.GENDER);
         return genderCovariates?.Select(genderRepository.FindByName).ToList() ?? new List<Gender>();
      }

      public virtual IReadOnlyList<string> AllCovariateNames => new List<string>(IndividualValuesCache.AllCovariatesNames().Union(new[] {CoreConstants.Covariates.POPULATION_NAME}));

      public virtual IReadOnlyList<string> AllCovariateValuesFor(string covariateName)
      {
         if (string.Equals(covariateName, CoreConstants.Covariates.POPULATION_NAME))
            return new string[NumberOfItems].InitializeWith(Name);

         return IndividualValuesCache.AllCovariateValuesFor(covariateName);
      }

      public bool DisplayParameterUsingGroupStructure => true;

      /// <summary>
      ///    Return the fist individual defined in the population
      /// </summary>
      public abstract Individual FirstIndividual { get; }

      public override bool IsLoaded
      {
         set
         {
            base.IsLoaded = value;
            if (FirstIndividual != null)
               FirstIndividual.IsLoaded = value;
         }
      }

      public virtual OriginData OriginData => FirstIndividual?.OriginData ?? new OriginData();

      public virtual Species Species => OriginData.Species;

      public virtual bool IsPreterm => FirstIndividual?.IsPreterm ?? false;

      public virtual bool IsAgeDependent => FirstIndividual?.IsAgeDependent ?? false;

      public virtual bool IsHuman => FirstIndividual?.IsHuman ?? false;

      public Organism Organism => FirstIndividual?.Organism ?? new Organism();

      public IEnumerable<IndividualMolecule> AllMolecules() => AllMolecules<IndividualMolecule>();

      public IndividualMolecule MoleculeByName(string moleculeName) => FirstIndividual?.MoleculeByName(moleculeName);

      public IEnumerable<TMolecules> AllMolecules<TMolecules>() where TMolecules : IndividualMolecule
      {
         return FirstIndividual?.AllMolecules<TMolecules>() ?? Enumerable.Empty<TMolecules>();
      }

      public void AddMolecule(IndividualMolecule molecule)
      {
         FirstIndividual?.AddMolecule(molecule);
      }

      public void RemoveMolecule(IndividualMolecule molecule)
      {
         FirstIndividual?.RemoveMolecule(molecule);
      }

      public void AddGlobalMolecule(IContainer molecule)
      {
         FirstIndividual?.AddGlobalMolecule(molecule);
      }

      public Individual Individual => FirstIndividual;

      public virtual IEnumerable<IParameter> AllAdvancedParameters(IEntityPathResolver entityPathResolver)
      {
         return AllVectorialParameters(entityPathResolver).Where(p => !p.IsChangedByCreateIndividual);
      }

      public virtual void SetAdvancedParameters(AdvancedParameterCollection advancedParameters)
      {
         Add(advancedParameters);
      }

      private AdvancedParameterCollection advancedParameterCollection
      {
         get { return this.GetSingleChild<AdvancedParameterCollection>(x => true); }
      }

      public virtual IEnumerable<AdvancedParameter> AdvancedParameters => advancedParameterCollection.AdvancedParameters;

      public virtual void RemoveAllAdvancedParameters() => advancedParameterCollection.Clear();

      public virtual AdvancedParameter AdvancedParameterFor(IEntityPathResolver entityPathResolver, IParameter parameter)
      {
         return advancedParameterCollection.AdvancedParameterFor(entityPathResolver, parameter);
      }

      public virtual void AddAdvancedParameter(AdvancedParameter advancedParameter, bool generateRandomValues = true)
      {
         advancedParameterCollection.AddAdvancedParameter(advancedParameter);
         if (generateRandomValues)
            GenerateRandomValuesFor(advancedParameter);
      }

      public virtual void RemoveAdvancedParameter(AdvancedParameter advancedParameter)
      {
         advancedParameterCollection.RemoveAdvancedParameter(advancedParameter);
         IndividualValuesCache.Remove(advancedParameter.ParameterPath);
      }

      /// <summary>
      ///    Return the individual parameters defined in the population
      /// </summary>
      public virtual IReadOnlyList<IParameter> AllIndividualParameters()
      {
         return FirstIndividual == null ? new List<IParameter>() : FirstIndividual.GetAllChildren<IParameter>();
      }

      public virtual IEnumerable<IParameter> AllConstantParameters(IEntityPathResolver entityPathResolver)
      {
         //get all possible constant parameters and remove the parameters that should be defined as variable 
         var allParameters = AllIndividualParameters().Where(parameterShouldBeReturnedAsConstant).ToList();
         AllVectorialParameters(entityPathResolver).Each(p => allParameters.Remove(p));
         return allParameters;
      }

      private bool parameterShouldBeReturnedAsConstant(IParameter parameter)
      {
         return !parameter.IsExpressionOrOntogenyFactor();
      }

      public virtual PathCache<IParameter> AllParameters(IEntityPathResolver entityPathResolver)
      {
         return new PathCache<IParameter>(entityPathResolver).For(AllIndividualParameters());
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourcePopulation = sourceObject as Population;
         if (sourcePopulation == null) return;
         IndividualValuesCache = sourcePopulation.IndividualValuesCache.Clone();
         SelectedDistributions = sourcePopulation.SelectedDistributions.Clone();
         Seed = sourcePopulation.Seed;
      }

      public override void AcceptVisitor(IVisitor visitor)
      {
         base.AcceptVisitor(visitor);
         FirstIndividual?.AcceptVisitor(visitor);
      }
   }
}
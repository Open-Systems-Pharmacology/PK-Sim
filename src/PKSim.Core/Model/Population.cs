using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Maths.Random;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Model
{
   public abstract class Population : PKSimBuildingBlock, ISimulationSubject, IAdvancedParameterContainer
   {
      public virtual ParameterDistributionSettingsCache SelectedDistributions { get; private set; }

      /// <summary>
      ///    The underlying cache managing the individuals and their values
      /// </summary>
      public virtual IndividualPropertiesCache IndividualPropertiesCache { get; protected set; }

      private RandomGenerator _randomGenerator;
      private int _seed;

      public virtual RandomGenerator RandomGenerator => _randomGenerator;

      protected Population() : base(PKSimBuildingBlockType.Population)
      {
         IndividualPropertiesCache = new IndividualPropertiesCache();
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
         if (IndividualPropertiesCache.Has(parameterPath))
            return IndividualPropertiesCache.PercentilesFor(parameterPath);

         return defaultValuesWith(CoreConstants.DEFAULT_PERCENTILE);
      }

      public virtual int NumberOfItems => IndividualPropertiesCache.Count;

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
         if (IndividualPropertiesCache.Has(parameterPath))
            return IndividualPropertiesCache.ValuesFor(parameterPath);

         return defaultValuesWith(double.NaN);
      }

      private double[] defaultValuesWith(double defaultValue)
      {
         return new double[NumberOfItems].InitializeWith(defaultValue);
      }

      /// <summary>
      ///    Add one individual to the population
      /// </summary>
      /// <param name="individualProperties">properties of individual added to population</param>
      public virtual void AddIndividualProperties(IndividualProperties individualProperties)
      {
         IndividualPropertiesCache.Add(individualProperties);
      }

      public virtual IEnumerable<IParameter> AllVectorialParameters(IEntityPathResolver entityPathResolver)
      {
         var allParameters = AllParameters(entityPathResolver);
         return IndividualPropertiesCache.AllParameterPaths().Select(p => allParameters[p]).Where(p => p != null);
      }

      public virtual void GenerateRandomValuesFor(AdvancedParameter advancedParameter)
      {
         IndividualPropertiesCache.SetValues(advancedParameter.ParameterPath, advancedParameter.GenerateRandomValues(NumberOfItems));
      }

      public IParameter ParameterByPath(string parameterPath, IEntityPathResolver entityPathResolver)
      {
         return AllParameters(entityPathResolver)[parameterPath];
      }

      public virtual IReadOnlyList<Gender> AllGenders => IndividualPropertiesCache.Genders;
      
      public virtual IReadOnlyList<SpeciesPopulation> AllRaces => IndividualPropertiesCache.Races;

      public virtual IReadOnlyList<string> AllCovariateNames => new List<string>(IndividualPropertiesCache.AllCovariatesNames().Union(new[] {CoreConstants.Covariates.POPULATION_NAME}));

      public virtual IReadOnlyList<string> AllCovariateValuesFor(string covariateName)
      {
         if (string.Equals(covariateName, CoreConstants.Covariates.POPULATION_NAME))
            return new string[NumberOfItems].InitializeWith(Name);

         return IndividualPropertiesCache.AllCovariates.Select(x => x.Covariate(covariateName)).ToList();
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

      public IEnumerable<IndividualMolecule> AllMolecules()
      {
         if (FirstIndividual != null)
            return FirstIndividual.AllMolecules();

         return Enumerable.Empty<IndividualMolecule>();
      }

      public IEnumerable<TMolecules> AllMolecules<TMolecules>() where TMolecules : IndividualMolecule
      {
         if (FirstIndividual != null)
            return FirstIndividual.AllMolecules<TMolecules>();

         return Enumerable.Empty<TMolecules>();
      }

      public void AddMolecule(IndividualMolecule molecule)
      {
         FirstIndividual?.AddMolecule(molecule);
      }

      public void RemoveMolecule(IndividualMolecule molecule)
      {
         FirstIndividual?.RemoveMolecule(molecule);
      }

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
         IndividualPropertiesCache.Remove(advancedParameter.ParameterPath);
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
         if (parameter.IsNamed(CoreConstants.Parameter.REFERENCE_CONCENTRATION))
            return true;

         return !parameter.IsIndividualMolecule();
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
         IndividualPropertiesCache = sourcePopulation.IndividualPropertiesCache.Clone();
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
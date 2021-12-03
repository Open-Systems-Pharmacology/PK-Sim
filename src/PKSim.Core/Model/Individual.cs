using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model.Extensions;

namespace PKSim.Core.Model
{
   public class Individual : PKSimBuildingBlock, ISimulationSubject
   {
      private readonly List<ExpressionProfile> _allExpressionProfiles = new List<ExpressionProfile>();

      /// <summary>
      ///    Seed used to initialize the random generator while creating the individual
      /// </summary>
      public virtual int Seed { get; set; }

      /// <summary>
      ///    Origin data defining the property used to create the individual
      /// </summary>
      public virtual OriginData OriginData { get; set; }

      public Individual() : base(PKSimBuildingBlockType.Individual)
      {
         Seed = Environment.TickCount;
      }

      Individual ISimulationSubject.Individual => this;

      public virtual Organism Organism => this.GetSingleChild<Organism>();

      public virtual IContainer Neighborhoods => this.GetSingleChildByName<IContainer>(Constants.NEIGHBORHOODS);

      /// <summary>
      ///    Population used to create the individual (based on the selected species)
      /// </summary>
      public virtual SpeciesPopulation Population => OriginData.SpeciesPopulation;

      /// <summary>
      ///    all available organs in the individual
      /// </summary>
      public virtual IEnumerable<Organ> AllOrgans() => Organism.OrgansByType(OrganType.Tissue | OrganType.VascularSystem);

      /// <summary>
      ///    Returns the available genders defined for the population in which the individual belongs
      /// </summary>
      public virtual IEnumerable<Gender> AvailableGenders() => OriginData.SpeciesPopulation.Genders;

      public virtual bool IsAgeDependent => OriginData.SpeciesPopulation.IsAgeDependent;

      public virtual bool IsHuman => Species.IsHuman;

      public virtual bool IsPreterm => Population.IsPreterm && OriginData.IsPreterm;

      public virtual Species Species => OriginData.Species;

      /// <summary>
      ///    All molecules defined in the individual
      /// </summary>
      public virtual IEnumerable<IndividualMolecule> AllMolecules() => AllMolecules<IndividualMolecule>();

      public IndividualMolecule MoleculeByName(string moleculeName) => MoleculeByName<IndividualMolecule>(moleculeName);

      /// <summary>
      ///    All defined molecules defined in the individual
      /// </summary>
      public virtual IEnumerable<IndividualMolecule> AllDefinedMolecules() => AllMolecules().Where(x => !x.IsUndefinedMolecule());

      /// <summary>
      ///    All protein of type <typeparamref name="TMolecule" /> in the individual
      /// </summary>
      /// <typeparam name="TMolecule"> Type of molecule to be retrieved </typeparam>
      public virtual IEnumerable<TMolecule> AllMolecules<TMolecule>() where TMolecule : IndividualMolecule => GetChildren<TMolecule>();

      public ExpressionProfile ExpressionProfileFor(IndividualMolecule molecule) => AllExpressionProfiles().Find(x => string.Equals(x.MoleculeName, molecule.Name));

      public void AddExpressionProfile(ExpressionProfile expressionProfile)
      {
         if (Uses(expressionProfile))
            return;

         _allExpressionProfiles.Add(expressionProfile);
      }

      public void RemoveExpressionProfile(ExpressionProfile expressionProfile) => _allExpressionProfiles.Remove(expressionProfile);

      public IReadOnlyList<ExpressionProfile> AllExpressionProfiles() => _allExpressionProfiles;

      public IndividualMolecule MoleculeFor(ExpressionProfile expressionProfile)
      {
         return MoleculeByName(expressionProfile.MoleculeName);
      }

      public bool Uses(ExpressionProfile expressionProfile) => _allExpressionProfiles.Contains(expressionProfile);

      public virtual void AddMolecule(IndividualMolecule molecule) => Add(molecule);

      public virtual void RemoveMolecule(IndividualMolecule molecule)
      {
         RemoveChild(molecule);
         var allMoleculeBuildingBlocks = GetAllChildren<IContainer>(x => x.IsNamed(molecule.Name));
         allMoleculeBuildingBlocks.Each(x => x.ParentContainer.RemoveChild(x));
      }

      /// <summary>
      ///    Return the protein with the name <paramref name="name" /> if defined in the individual, otherwise null
      /// </summary>
      /// <typeparam name="TMolecule"> Type of protein </typeparam>
      /// <param name="name"> Name of protein </param>
      public virtual TMolecule MoleculeByName<TMolecule>(string name) where TMolecule : IndividualMolecule
      {
         return MoleculeBy<TMolecule>(enz => string.Equals(enz.Name, name));
      }

      /// <summary>
      ///    Return the first protein meeting the search criteria, or null if not found
      /// </summary>
      /// <typeparam name="TMolecule"> Type of protein </typeparam>
      /// <param name="criteria"> search criteria </param>
      public virtual TMolecule MoleculeBy<TMolecule>(Func<TMolecule, bool> criteria) where TMolecule : IndividualMolecule
      {
         return AllMolecules<TMolecule>().FirstOrDefault(criteria);
      }

      /// <summary>
      ///    Input age of the individual.
      /// </summary>
      public virtual double Age => OriginData.Age?.Value ?? 0;

      /// <summary>
      ///    Input Weight of the individual.
      /// </summary>
      public virtual double InputWeight => OriginData.Weight.Value;

      /// <summary>
      ///    Input Height of the individual.
      /// </summary>
      public virtual double InputHeight => OriginData.Height?.Value ?? 0;

      /// <summary>
      ///    Mean height as defined in the database for the organism
      /// </summary>
      public virtual double MeanHeight
      {
         get
         {
            if (OriginData == null)
               return double.NaN;

            if (OriginData.SpeciesPopulation.IsHeightDependent)
               return Organism.Parameter(CoreConstants.Parameters.MEAN_HEIGHT).Value;

            return double.NaN;
         }
      }

      /// ///
      /// <summary>
      ///    Mean weight as defined in the database for the organism
      /// </summary>
      public virtual double MeanWeight => Organism.Parameter(CoreConstants.Parameters.MEAN_WEIGHT).Value;

      /// <summary>
      ///    Actual weight of the individual (might differ from input weight and mean weight if volumes were changed)
      /// </summary>
      public virtual IParameter WeightParameter => Organism.Parameter(CoreConstants.Parameters.WEIGHT);

      /// <summary>
      ///    Actual age of the individual
      /// </summary>
      public virtual IParameter AgeParameter => Organism.Parameter(CoreConstants.Parameters.AGE);

      /// <summary>
      ///    Returns <c>true</c> if at least one molecule is defined in the individual otherwise false
      /// </summary>
      public bool HasMolecules() => HasMolecules<IndividualMolecule>();

      /// <summary>
      ///    Returns <c>true</c> if at least one molecule of type <typeparamref name="TIndividualMolecule" />is defined in the
      ///    individual otherwise false
      /// </summary>
      public bool HasMolecules<TIndividualMolecule>() where TIndividualMolecule : IndividualMolecule => AllMolecules<TIndividualMolecule>().Any();

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         var individual = sourceObject as Individual;
         if (individual == null) return;

         base.UpdatePropertiesFrom(individual, cloneManager);
         OriginData = individual.OriginData.Clone();
         Seed = individual.Seed;
         individual.AllExpressionProfiles().Each(AddExpressionProfile);
      }

      /// <summary>
      ///    Returns all possible (physical) containers of the organism in which <paramref name="molecule" /> will be defined or
      ///    an empty array if the organism is not defined
      /// </summary>
      public virtual IReadOnlyList<IContainer> AllPhysicalContainersWithMoleculeFor(IndividualMolecule molecule) =>
         Organism?.GetAllChildren<IContainer>(x => x.IsNamed(molecule.Name)).Select(x => x.ParentContainer).ToArray() ?? Array.Empty<IContainer>();

      /// <summary>
      ///    Returns all possible molecule parameters defined for <paramref name="molecule" /> in the individual.
      ///    This also returns the global molecule parameters
      /// </summary>
      public virtual IReadOnlyList<IParameter> AllMoleculeParametersFor(IndividualMolecule molecule) =>
         GetAllChildren<IContainer>(x => x.IsNamed(molecule.Name)).SelectMany(x => x.AllParameters()).ToList();

      /// <summary>
      ///    Returns all possible molecule containers of the individual in which <paramref name="molecule" /> will be defined.
      ///    This also returns global container under the global molecule named after <paramref name="molecule" />
      /// </summary>
      public virtual IReadOnlyList<MoleculeExpressionContainer> AllMoleculeContainersFor(IndividualMolecule molecule) =>
         AllMoleculeContainersFor<MoleculeExpressionContainer>(molecule);

      /// <summary>
      ///    Returns all possible molecule containers of the individual in which <paramref name="molecule" /> will be defined.
      ///    This also returns global container under the global molecule named after <paramref name="molecule" />
      /// </summary>
      public IReadOnlyList<T> AllMoleculeContainersFor<T>(IndividualMolecule molecule) where T : MoleculeExpressionContainer =>
         GetAllChildren<T>(x => x.IsNamed(molecule.Name) || x.ParentContainer.IsNamed(molecule.Name));

      public virtual ICache<string, IParameter> AllExpressionParametersFor(IndividualMolecule molecule)
      {
         var cache = new Cache<string, IParameter>(onMissingKey: x => null);
         var allExpressionParameters = GetAllChildren<IParameter>(x => x.IsExpression() && x.ParentContainer.IsNamed(molecule.Name));
         allExpressionParameters.Each(p =>
         {
            if (p.IsGlobalExpression())
               cache[CoreConstants.ContainerName.GlobalExpressionContainerNameFor(p.Name)] = p;
            else
            {
               var container = p.ParentContainer.ParentContainer;
               var key = container.IsNamed(CoreConstants.Compartment.INTRACELLULAR) ? container.ParentContainer.Name : container.Name;
               if (p.IsInLumen())
                  key = CoreConstants.ContainerName.LumenSegmentNameFor(key);

               cache[key] = p;
            }
         });
         return cache;
      }
   }
}
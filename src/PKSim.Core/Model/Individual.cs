using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class Individual : PKSimBuildingBlock, ISimulationSubject
   {
      /// <summary>
      ///    Seed used to intialize the random generator while creating the individual
      /// </summary>
      public virtual int Seed { get;  set; }

      /// <summary>
      ///    Origin data defining the property used to create the individual
      /// </summary>
      public virtual OriginData OriginData { get; set; }

      public Individual() : base(PKSimBuildingBlockType.Individual)
      {
         Seed = Environment.TickCount;
      }


      public virtual Organism Organism => this.GetSingleChild<Organism>();

      public virtual IContainer Neighborhoods => this.GetSingleChildByName<IContainer>(Constants.NEIGHBORHOODS);

      /// <summary>
      ///    Population used to create the individul (based on the selected species)
      /// </summary>
      public virtual SpeciesPopulation Population => OriginData.SpeciesPopulation;

 
      /// <summary>
      ///    all available organs in the individual
      /// </summary>
      public virtual IEnumerable<Organ> AllOrgans()
      {
         return Organism.OrgansByType(OrganType.Tissue | OrganType.VascularSystem);
      }

      /// <summary>
      ///    Returns the availabe genders defined for the population in which the individual belongs
      /// </summary>
      public virtual IEnumerable<Gender> AvailableGenders()
      {
         return OriginData.SpeciesPopulation.Genders;
      }

      public virtual bool IsAgeDependent => OriginData.SpeciesPopulation.IsAgeDependent;

      public virtual bool IsHuman => Species.IsHuman;

      public virtual bool IsPreterm
      {
         get
         {
            if (!Population.IsPreterm)
               return false;

            return OriginData.GestationalAge.HasValue && OriginData.GestationalAge.Value <= CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS;
         }
      }

      public virtual Species Species => OriginData.Species;

      /// <summary>
      ///    All molecules defined in the individual
      /// </summary>
      public virtual IEnumerable<IndividualMolecule> AllMolecules() => AllMolecules<IndividualMolecule>();

      /// <summary>
      ///    All defined molecules defined in the individual
      /// </summary>
      public virtual IEnumerable<IndividualMolecule> AllDefinedMolecules() => AllMolecules().Where(x => !x.IsUndefinedMolecule());

      /// <summary>
      ///    All protein of type
      ///    <typeparam name="TMolecule" />
      ///    in the individual
      /// </summary>
      /// <typeparam name="TMolecule"> Type of molecule to be retrieved </typeparam>
      public virtual IEnumerable<TMolecule> AllMolecules<TMolecule>() where TMolecule : IndividualMolecule => GetChildren<TMolecule>();

      public virtual void AddMolecule(IndividualMolecule molecule) => Add(molecule);

      public virtual void RemoveMolecule(IndividualMolecule molecule) => RemoveChild(molecule);

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
      ///    Input age of the indvidual.
      /// </summary>
      public virtual double Age => OriginData.Age ?? 0;

      /// <summary>
      ///    Input height of the indvidual.
      /// </summary>
      public virtual double InputWeight => OriginData.Weight;

      /// <summary>
      ///    Input Weight of the indvidual.
      /// </summary>
      public virtual double InputHeight => OriginData.Height ?? 0;

      /// <summary>
      ///    Mean height as defined in the databse for the organism
      /// </summary>
      public virtual double MeanHeight
      {
         get
         {
            if (OriginData == null)
               return double.NaN;

            if (OriginData.SpeciesPopulation.IsHeightDependent)
               return Organism.Parameter(CoreConstants.Parameter.MEAN_HEIGHT).Value;

            return double.NaN;
         }
      }

      /// ///
      /// <summary>
      ///    Mean weight as defined in the databse for the organism
      /// </summary>
      public virtual double MeanWeight => Organism.Parameter(CoreConstants.Parameter.MEAN_WEIGHT).Value;

      /// <summary>
      ///    Actual weight of the individual (might differ from input weight and mean weight if volumina were changed)
      /// </summary>
      public virtual IParameter WeightParameter => Organism.Parameter(CoreConstants.Parameter.WEIGHT);

      /// <summary>
      ///    Returns <c>true</c> if at least one molecule is defined in the individual otherwise false
      /// </summary>
      public bool HasMolecules()
      {
         return HasMolecules<IndividualMolecule>();
      }

      /// <summary>
      ///    Returns <c>true</c> if at least one molecule of type <typeparamref name="TIndividualMolecule"/>is defined in the individual otherwise false
      /// </summary>
      public bool HasMolecules<TIndividualMolecule>() where TIndividualMolecule : IndividualMolecule
      {
         return AllMolecules<TIndividualMolecule>().Any();
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         var individual = sourceObject as Individual;
         if (individual == null) return;

         base.UpdatePropertiesFrom(individual, cloneManager);
         OriginData = individual.OriginData.Clone();
         Seed = individual.Seed;
      }
   }
}
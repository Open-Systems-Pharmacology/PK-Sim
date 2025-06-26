using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;

namespace PKSim.Core.Model
{
   public class PKSimProject : Project
   {
      private readonly List<IPKSimBuildingBlock> _allBuildingBlocks = new List<IPKSimBuildingBlock>();
      private readonly List<ISimulationComparison> _simulationComparisons = new List<ISimulationComparison>();
      private readonly List<QualificationPlan> _qualificationPlans = new List<QualificationPlan>();

      /// <summary>
      ///    returns the default settings for the given settings type or null if not found
      /// </summary>
      public OutputSelections OutputSelections { get; set; }

      private bool _hasChanged;

      public override bool HasChanged
      {
         get => _hasChanged;
         set
         {
            _hasChanged = value;
            if (_hasChanged) return;
            _allBuildingBlocks.Each(bb => bb.HasChanged = false);
         }
      }

      public override IEnumerable<IUsesObservedData> AllUsersOfObservedData => AllParameterAnalysables.OfType<IUsesObservedData>().Union(All<Simulation>()).Union(AllSimulationComparisons);

      /// <summary>
      ///    Returns all <see cref="ISimulationComparison" /> defined in the project
      /// </summary>
      public virtual IReadOnlyCollection<ISimulationComparison> AllSimulationComparisons => _simulationComparisons;


      /// <summary>
      ///    Returns all <see cref="QualificationPlan" /> defined in the project
      /// </summary>
      public virtual IReadOnlyCollection<QualificationPlan> AllQualificationPlans => _qualificationPlans;   

      
      /// <summary>
      ///    Adds the <paramref name="simulationComparison" /> to the project
      /// </summary>
      public virtual void AddSimulationComparison(ISimulationComparison simulationComparison)
      {
         _simulationComparisons.Add(simulationComparison);
         _hasChanged = true;
      }

      /// <summary>
      ///    Adds the <paramref name="qualificationPlan" /> to the project
      /// </summary>
      public virtual void AddQualificationPlan(QualificationPlan qualificationPlan)
      {
         _qualificationPlans.Add(qualificationPlan);
         _hasChanged = true;
      }


      /// <summary>
      ///    Removes the <paramref name="simulationComparison" /> from the project
      /// </summary>
      public virtual void RemoveSimulationComparison(ISimulationComparison simulationComparison)
      {
         if (!_simulationComparisons.Contains(simulationComparison))
            return;

         _simulationComparisons.Remove(simulationComparison);
         RemoveClassifiableForWrappedObject(simulationComparison);
         _hasChanged = true;
      }

      public override IReadOnlyCollection<T> All<T>() => All<T>(x => true);

      /// <summary>
      ///    Returns all the building block of a given type <paramref name="buildingBlockType" />
      /// </summary>
      public virtual IReadOnlyCollection<IPKSimBuildingBlock> All(PKSimBuildingBlockType buildingBlockType) => All<IPKSimBuildingBlock>(c => c.BuildingBlockType.Is(buildingBlockType));

      /// <summary>
      ///    Returns all the building block of a given type <typeparamref name="T" /> matching a given predicate
      /// </summary>
      /// <typeparam name="T">Type if building block</typeparam>
      public virtual IReadOnlyCollection<T> All<T>(Func<T, bool> predicate) where T : class
      {
         var query = from child in _allBuildingBlocks
            let castChild = child as T
            where castChild != null
            where predicate(castChild)
            select castChild;

         return query.ToList();
      }

      public virtual IPKSimBuildingBlock BuildingBlockById(string templateBuildingBlockId) => BuildingBlockById<IPKSimBuildingBlock>(templateBuildingBlockId);

      public virtual T BuildingBlockById<T>(string templateBuildingBlockId) where T : class, IPKSimBuildingBlock => _allBuildingBlocks.FindById(templateBuildingBlockId) as T;

      public virtual T BuildingBlockByName<T>(string templateBuildingBlockName) where T : class, IPKSimBuildingBlock => _allBuildingBlocks.OfType<T>().FindByName(templateBuildingBlockName);

      public virtual IPKSimBuildingBlock BuildingBlockByName(string templateBuildingBlockName, PKSimBuildingBlockType type) => All(type).FindByName(templateBuildingBlockName);

      /// <summary>
      ///    Add a building block to the project
      /// </summary>
      public virtual void AddBuildingBlock(IPKSimBuildingBlock buildingBlockToAdd)
      {
         //that should never happen. Just in case
         var bbWithTheSameName = All(buildingBlockToAdd.BuildingBlockType)
            .Where(bb => bb.IsNamed(buildingBlockToAdd.Name));

         if (bbWithTheSameName.Any())
            throw new BuildingBlockAlreadyExistsInProjectException(buildingBlockToAdd);

         _allBuildingBlocks.Add(buildingBlockToAdd);
      }

      /// <summary>
      ///    Remove a building block from the project
      /// </summary>
      public virtual void RemoveBuildingBlock(IPKSimBuildingBlock buildingBlockToRemove)
      {
         _allBuildingBlocks.Remove(buildingBlockToRemove);
         RemoveClassifiableForWrappedObject(buildingBlockToRemove);
      }

      public override void AcceptVisitor(IVisitor visitor)
      {
         base.AcceptVisitor(visitor);
         _allBuildingBlocks.ToList().Each(x => x.AcceptVisitor(visitor));
         _simulationComparisons.ToList().Each(x => x.AcceptVisitor(visitor));
      }

      /// <summary>
      /// Returns all <see cref="DataRepository"/> defined in the project. They are collected from all <see cref="IndividualSimulation"/> having results and all ObservedData defined in the project
      /// </summary>
      public IEnumerable<DataRepository> AllDataRepositories()
      {
         return All<IndividualSimulation>()
            .Where(s => s.HasResults)
            .Select(s => s.DataRepository)
            .Union(AllObservedData);
      }
   }
}
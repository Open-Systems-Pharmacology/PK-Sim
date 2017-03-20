using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.SensitivityAnalyses;

namespace PKSim.Core.Model
{
   public interface IPKSimProject : IProject
   {

      /// <summary>
      ///    Returns all the building block of a given type <paramref name="buildingBlockType" />
      /// </summary>
      IReadOnlyCollection<IPKSimBuildingBlock> All(PKSimBuildingBlockType buildingBlockType);

      /// <summary>
      ///    Returns all the building block of a given type <typeparamref name="T" /> matching a given predicate
      /// </summary>
      /// <typeparam name="T">Type if building block</typeparam>
      IReadOnlyCollection<T> All<T>(Func<T, bool> predicate) where T : class;

      /// <summary>
      ///    Add a building block to the project
      /// </summary>
      void AddBuildingBlock(IPKSimBuildingBlock buildingBlockToAdd);

      /// <summary>
      ///    Remove a building block from the project
      /// </summary>
      void RemoveBuildingBlock(IPKSimBuildingBlock buildingBlockToRemove);


      /// <summary>
      ///    Returns all <see cref="ISimulationComparison"/> defined in the project
      /// </summary>
      IReadOnlyCollection<ISimulationComparison> AllSimulationComparisons { get; }

      /// <summary>
      ///    Adds the <paramref name="simulationComparison" /> to the project
      /// </summary>
      void AddSimulationComparison(ISimulationComparison simulationComparison);

      /// <summary>
      ///    Removes the <paramref name="simulationComparison" /> from the project
      /// </summary>
      void RemoveSimulationComparison(ISimulationComparison simulationComparison);

      /// <summary>
      ///    returns the default settings for the given settings type or null if not found
      /// </summary>
      OutputSelections OutputSelections { get; set; }
   }

   public class PKSimProject : Project, IPKSimProject
   {
      private readonly List<IPKSimBuildingBlock> _allBuildingBlocks = new List<IPKSimBuildingBlock>();
      private readonly List<ISimulationComparison> _simulationComparisons = new List<ISimulationComparison>();

      public OutputSelections OutputSelections { get; set; }

      private bool _hasChanged;


      public override bool HasChanged
      {
         get { return _hasChanged; }
         set
         {
            _hasChanged = value;
            if (_hasChanged) return;
            _allBuildingBlocks.Each(bb => bb.HasChanged = false);
         }
      }

      public override IEnumerable<IUsesObservedData> AllUsersOfObservedData => AllParameterAnalysables.OfType<IUsesObservedData>().Union(All<Simulation>());

      public IReadOnlyCollection<ISimulationComparison> AllSimulationComparisons => _simulationComparisons;

      public void AddSimulationComparison(ISimulationComparison simulationComparison)
      {
         _simulationComparisons.Add(simulationComparison);
         _hasChanged = true;
      }

      public void RemoveSimulationComparison(ISimulationComparison simulationComparison)
      {
         if (!_simulationComparisons.Contains(simulationComparison)) return;
         _simulationComparisons.Remove(simulationComparison);
         RemoveClassifiableForWrappedObject(simulationComparison);
         _hasChanged = true;
      }

      public override IReadOnlyCollection<T> All<T>()
      {
         return All<T>(x => true);
      }

      public IReadOnlyCollection<IPKSimBuildingBlock> All(PKSimBuildingBlockType buildingBlockType)
      {
         return All<IPKSimBuildingBlock>(c => c.BuildingBlockType.Is(buildingBlockType));
      }

      public IReadOnlyCollection<T> All<T>(Func<T, bool> predicate) where T : class
      {
         var query = from child in _allBuildingBlocks
                     let castChild = child as T
                     where castChild != null
                     where predicate(castChild)
                     select castChild;

         return query.ToList();
      }

      public void AddBuildingBlock(IPKSimBuildingBlock buildingBlockToAdd)
      {
         //that should never happen. Just in case
         var bbWithTheSameName = All(buildingBlockToAdd.BuildingBlockType)
            .Where(bb => string.Equals(bb.Name, buildingBlockToAdd.Name));

         if (bbWithTheSameName.Count() != 0)
            throw new BuildingBlockAlreadyExistsInProjectException(buildingBlockToAdd);

         _allBuildingBlocks.Add(buildingBlockToAdd);
      }

      public void RemoveBuildingBlock(IPKSimBuildingBlock buildingBlockToRemove)
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
   }
}
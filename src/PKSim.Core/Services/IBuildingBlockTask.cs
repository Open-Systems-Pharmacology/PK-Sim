using System.Collections.Generic;
using System.Threading.Tasks;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IBuildingBlockTask
   {
      void AddCommandToHistory(ICommand command);
      void Load<TBuildingBlock>(TBuildingBlock buildingBlockToLoad) where TBuildingBlock : class, IPKSimBuildingBlock;

      /// <summary>
      ///    Load the simulation and its results in memory
      /// </summary>
      /// <typeparam name="TBuildingBlock">Type of simulation</typeparam>
      /// <param name="simulationToLoad">Simulation to load</param>
      void LoadResults<TBuildingBlock>(TBuildingBlock simulationToLoad) where TBuildingBlock : Simulation;

      void Edit(IPKSimBuildingBlock buildingBlock);
      void Clone<TBuildingBlock>(TBuildingBlock buildingBlockToClone) where TBuildingBlock : class, IPKSimBuildingBlock;
      bool Delete<TBuildingBlock>(TBuildingBlock buildingBlockToDelete) where TBuildingBlock : class, IPKSimBuildingBlock;
      bool Delete<TBuildingBlock>(IReadOnlyList<TBuildingBlock> buildingBlocksToDelete) where TBuildingBlock : class, IPKSimBuildingBlock;
      void Rename<TBuildingBlock>(TBuildingBlock buildingBlockToRename) where TBuildingBlock : class, IPKSimBuildingBlock;
      IReadOnlyList<TBuildingBlock> LoadFromTemplate<TBuildingBlock>(PKSimBuildingBlockType buildingBlockType) where TBuildingBlock : class, IPKSimBuildingBlock;

      IReadOnlyList<TBuildingBlock> LoadFromSnapshot<TBuildingBlock>(PKSimBuildingBlockType buildingBlockType) where TBuildingBlock : class, IPKSimBuildingBlock;

      /// <summary>
      ///    Saves the building blocks defined as key in <paramref name="buildingBlocksWithReferenceToSave" /> and all their
      ///    references as well (defined as value of the cache <paramref name="buildingBlocksWithReferenceToSave" />).
      ///    It is expected that each reference is available as key in the cache as well.
      /// </summary>
      void SaveAsTemplate(ICache<IPKSimBuildingBlock, IReadOnlyList<IPKSimBuildingBlock>> buildingBlocksWithReferenceToSave, TemplateDatabaseType templateDatabaseType);

      void EditDescription(IPKSimBuildingBlock buildingBlock);
      string TypeFor<TBuildingBlock>(TBuildingBlock buildingBlock) where TBuildingBlock : class, IPKSimBuildingBlock;
      IEnumerable<TBuildingBlock> All<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock;

      /// <summary>
      ///    Add the given building block to the project and returns the command resulting from the action.
      /// </summary>
      /// <param name="buildingBlock">Building block to add to the project</param>
      /// <param name="editBuildingBlock"> if set to <c>true</c>, the edit action is started as well. Default is false</param>
      /// <param name="addToHistory">if set to <c>true</c>, the add command is also added to the history. Default is true</param>
      IPKSimCommand AddToProject<TBuildingBlock>(TBuildingBlock buildingBlock, bool editBuildingBlock = false, bool addToHistory = true) where TBuildingBlock : class, IPKSimBuildingBlock;

      /// <summary>
      ///    Returns true if a <see cref="IPKSimBuildingBlock" /> with the same name and <see cref="PKSimBuildingBlockType" /> as
      ///    the
      ///    given <paramref name="buildingBlock" /> already exists in the project otherwise false
      /// </summary>
      bool BuildingBlockNameIsAlreadyUsed(IPKSimBuildingBlock buildingBlock);

      /// <summary>
      ///    If a a <see cref="IPKSimBuildingBlock" /> does not exist in the project with the same name and
      ///    <see cref="PKSimBuildingBlockType" /> as the
      ///    given <paramref name="buildingBlock" /> returns <c>true</c>. Otherwise, asks the user to rename the building block.
      ///    If the rename as performed, returns
      ///    <c>true</c> otherwise <c>false</c>
      /// </summary>
      bool RenameBuildingBlockIfAlreadyUsed(IPKSimBuildingBlock buildingBlock);

      IPKSimCommand DeleteCommand<TBuildingBlock>(TBuildingBlock buildingBlock) where TBuildingBlock : class, IPKSimBuildingBlock;
      void RemovePresenterSettings<TBuildingBlock>(TBuildingBlock buildingBlock) where TBuildingBlock : class, IPKSimBuildingBlock;
   }

   public interface IBuildingBlockTask<TBuildingBlock> where TBuildingBlock : IPKSimBuildingBlock
   {
      TBuildingBlock AddToProject();
      IPKSimCommand AddToProject(TBuildingBlock buildingBlock, bool editBuildingBlock = true, bool addToHistory = true);
      void Edit(TBuildingBlock buildingBlockToEdit);
      TBuildingBlock LoadSingleFromTemplate();
      IReadOnlyList<TBuildingBlock> LoadFromTemplate();
      IReadOnlyList<TBuildingBlock> LoadFromSnapshot();
      void Load(TBuildingBlock buildingBlockToLoad);
      IEnumerable<TBuildingBlock> All();
      void SaveAsTemplate(TBuildingBlock buildingBlockToSave);
      void SaveAsSystemTemplate(TBuildingBlock buildingBlockToSave);
   }
}
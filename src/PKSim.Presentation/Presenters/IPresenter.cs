using PKSim.Core.Commands;
using PKSim.Core.Model;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters
{
   public interface IEditBuildingBockPresenter<TBuildingBlock> : ISingleStartPresenter<TBuildingBlock> where TBuildingBlock : IPKSimBuildingBlock
   {
   }

   public interface ICreateBuildingBlockPresenter<out TBuildingBlock> : IDisposablePresenter where TBuildingBlock : IPKSimBuildingBlock
   {
      /// <summary>
      /// Starts the creation workflow of a new building block of type <typeparamref name="TBuildingBlock"/>
      /// </summary>
      /// <returns></returns>
      IPKSimCommand Create();

      /// <summary>
      /// Returns the created <typeparamref name="TBuildingBlock"/> 
      /// </summary>
      TBuildingBlock BuildingBlock { get; }
   }
}
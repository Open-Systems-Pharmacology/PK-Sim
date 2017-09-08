using PKSim.Core.Model;
using OSPSuite.Core;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Events
{
   public interface IEntityEvent
   {
      IEntity Subject { get; }
   }

   public abstract class EntityEvent<TEntity> : IEntityEvent where TEntity : IEntity
   {
      public TEntity Entity { get; set; }

      public IEntity Subject
      {
         get { return Entity; }
      }
   }

   public interface IEntityContainerEvent : IEntityEvent
   {
      IEntity ContainerSubject { get; }
   }

   public abstract class EntityContainerEvent<TEntity, TContainer> : EntityEvent<TEntity>, IEntityContainerEvent
      where TEntity : IEntity
      where TContainer : IEntity
   {
      public TContainer Container { get; set; }

      public IEntity ContainerSubject
      {
         get { return Container; }
      }
   }

   public abstract class AddEntityEvent<TEntity, TContainer> : EntityContainerEvent<TEntity, TContainer>
      where TEntity : IEntity
      where TContainer : IEntity

   {
   }

   public abstract class RemoveEntityEvent<TEntity, TContainer> : EntityContainerEvent<TEntity, TContainer>
      where TEntity : IEntity
      where TContainer : IEntity

   {
   }

   public class AddBuildingBlockEvent<TBuildingBlock> where TBuildingBlock : IPKSimBuildingBlock
   {
      public AddBuildingBlockEvent(TBuildingBlock buildingBlock, PKSimProject project)
      {
         BuildingBlock = buildingBlock;
         Project = project;
      }

      public TBuildingBlock BuildingBlock { get; set; }
      public PKSimProject Project { get; set; }
   }

   public class RemoveBuildingBlockEvent<TBuildingBlock> where TBuildingBlock : IPKSimBuildingBlock
   {
      public RemoveBuildingBlockEvent(TBuildingBlock buildingBlock, PKSimProject project)
      {
         BuildingBlock = buildingBlock;
         Project = project;
      }

      public TBuildingBlock BuildingBlock { get; set; }
      public PKSimProject Project { get; set; }
   }

   public class BuildingBlockAddedEvent : AddBuildingBlockEvent<IPKSimBuildingBlock>
   {
      public BuildingBlockAddedEvent(IPKSimBuildingBlock buildingBlock, PKSimProject project) : base(buildingBlock, project)
      {
      }
   }

   public class BuildingBlockRemovedEvent : RemoveBuildingBlockEvent<IPKSimBuildingBlock>
   {
      public bool DueToSwap { get; private set; }

      public BuildingBlockRemovedEvent(IPKSimBuildingBlock buildingBlock, PKSimProject project) : this(buildingBlock, project, false)
      {
      }

      public BuildingBlockRemovedEvent(IPKSimBuildingBlock buildingBlock, PKSimProject project, bool dueToSwap)
         : base(buildingBlock, project)
      {
         DueToSwap = dueToSwap;
      }
   }

   public class BuildingBlockUpdatedEvent
   {
      public IPKSimBuildingBlock BuildingBlock { get; private set; }

      public BuildingBlockUpdatedEvent(IPKSimBuildingBlock buildingBlock)
      {
         BuildingBlock = buildingBlock;
      }
   }

   public class ObjectBaseConvertedEvent
   {
      public IObjectBase ConvertedObject { get; private set; }
      public ProjectVersion FromVersion { get; private set; }

      public ObjectBaseConvertedEvent(IObjectBase convertedObject, ProjectVersion fromVersion)
      {
         ConvertedObject = convertedObject;
         FromVersion = fromVersion;
      }
   }


   public class RemoveParameterFromContainerEvent : RemoveEntityEvent<IParameter, IContainer>
   {
   }

   public class AddParameterToContainerEvent : AddEntityEvent<IParameter, IContainer>
   {
   }

   public class SwapSimulationEvent
   {
      public Simulation OldSimulation { get; private set; }
      public Simulation NewSimulation { get; private set; }

      public SwapSimulationEvent(Simulation oldSimulation, Simulation newSimulation)
      {
         OldSimulation = oldSimulation;
         NewSimulation = newSimulation;
      }
   }

   public class SwapBuildingBlockEvent
   {
      public IPKSimBuildingBlock OldBuildingBlock { get; private set; }
      public IPKSimBuildingBlock NewBuildingBlock { get; private set; }

      public SwapBuildingBlockEvent(IPKSimBuildingBlock oldBuildingBlock, IPKSimBuildingBlock newBuildingBlock)
      {
         OldBuildingBlock = oldBuildingBlock;
         NewBuildingBlock = newBuildingBlock;
      }
   }

   public class NoTranporterTemplateAvailableEvent
   {
      public IndividualTransporter Transporter { get; private set; }

      public NoTranporterTemplateAvailableEvent(IndividualTransporter transporter)
      {
         Transporter = transporter;
      }
   }
}
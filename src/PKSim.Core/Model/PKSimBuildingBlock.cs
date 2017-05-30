using System;
using System.Collections;
using System.Collections.Generic;
using OSPSuite.Utility.Visitor;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public interface IPKSimBuildingBlock : ILazyLoadable, IVersionable, IContainer, IWithCreationMetaData
   {
      PKSimBuildingBlockType BuildingBlockType { get; }

      /// <summary>
      ///    Root Container of the building block (container of all sub containers and paraemters)
      /// </summary>
      IRootContainer Root { get; set; }

      /// <summary>
      ///    Indicates if a loaded building block was changed and hence needs to be saved
      /// </summary>
      bool HasChanged { get; set; }
   }

   public abstract class PKSimBuildingBlock : Entity, IPKSimBuildingBlock
   {
      public virtual bool IsLoaded { get; set; }
      public virtual bool HasChanged { get; set; }
      public virtual string ExtendedDescription { get; set; }
      public virtual CreationMetaData Creation { get; set; }
      public virtual PKSimBuildingBlockType BuildingBlockType { get; }
      public virtual int Version { get; set; }
      public virtual int StructureVersion { get; set; }
      private IRootContainer _root;

      public virtual IRootContainer Root
      {
         get => _root;
         set
         {
            _root = value;
            if (_root == null) return;
            _root.ParentContainer = this;
         }
      }

      protected PKSimBuildingBlock()
      {
         Root = new RootContainer();
         HasChanged = true;
         Creation = new CreationMetaData();
      }

      protected PKSimBuildingBlock(PKSimBuildingBlockType buildingBlockType) : this()
      {
         BuildingBlockType = buildingBlockType;
         Icon = buildingBlockType.ToString();
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var buildingBlock = sourceObject as IPKSimBuildingBlock;
         if (buildingBlock == null) return;

         IsLoaded = buildingBlock.IsLoaded;
         Version = buildingBlock.Version;
         StructureVersion = buildingBlock.StructureVersion;
         Creation = buildingBlock.Creation.Clone();
         //no need to clone the root as all children will be added during the clone operation
         //Root = cloneManager.Clone(buildingBlock.Root);
      }

      public override void AcceptVisitor(IVisitor visitor)
      {
         base.AcceptVisitor(visitor);
         Root.AcceptVisitor(visitor);
      }

      public virtual void Add(IEntity newChild)
      {
         Root.Add(newChild);
      }

      public virtual void RemoveChild(IEntity childToRemove)
      {
         Root.RemoveChild(childToRemove);
      }

      public void RemoveChildren()
      {
         Root.RemoveChildren();
      }

      public virtual IReadOnlyList<T> GetAllChildren<T>(Func<T, bool> predicate) where T : class, IEntity
      {
         return Root.GetAllChildren(predicate);
      }

      public virtual IReadOnlyList<T> GetAllChildren<T>() where T : class, IEntity
      {
         return Root.GetAllChildren<T>();
      }

      public virtual IEnumerable<T> GetChildren<T>(Func<T, bool> predicate) where T : class, IEntity
      {
         return Root.GetChildren(predicate);
      }

      public virtual IEnumerable<T> GetChildren<T>() where T : class, IEntity
      {
         return Root.GetChildren<T>();
      }

      public virtual IEnumerable<IContainer> GetNeighborsFrom(IEnumerable<INeighborhood> neighborhoods)
      {
         return Root.GetNeighborsFrom(neighborhoods);
      }

      public virtual IEnumerable<INeighborhood> GetNeighborhoods(IEnumerable<INeighborhood> neighborhoods)
      {
         return Root.GetNeighborhoods(neighborhoods);
      }

      public virtual IReadOnlyList<TContainer> GetAllContainersAndSelf<TContainer>() where TContainer : class, IContainer
      {
         return Root.GetAllContainersAndSelf<TContainer>();
      }

      public virtual IReadOnlyList<TContainer> GetAllContainersAndSelf<TContainer>(Func<TContainer, bool> predicate) where TContainer : class, IContainer
      {
         return Root.GetAllContainersAndSelf(predicate);
      }

      public virtual ContainerType ContainerType
      {
         get => Root.ContainerType;
         set => Root.ContainerType = value;
      }

      public virtual ContainerMode Mode
      {
         get => Root.Mode;
         set => Root.Mode = value;
      }

      public virtual IEnumerable<IEntity> Children => Root.Children;

      public IEnumerator<IEntity> GetEnumerator()
      {
         return Children.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}
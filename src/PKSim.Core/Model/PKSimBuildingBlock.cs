using System;
using System.Collections;
using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Visitor;

namespace PKSim.Core.Model
{
   public interface IPKSimBuildingBlock : ILazyLoadable, IVersionable, IContainer, IWithCreationMetaData
   {
      PKSimBuildingBlockType BuildingBlockType { get; }

      /// <summary>
      ///    Root Container of the building block (container of all sub containers and parameters)
      /// </summary>
      IRootContainer Root { get; set; }

      /// <summary>
      ///    Indicates if a loaded building block was changed and hence needs to be saved
      /// </summary>
      bool HasChanged { get; set; }

      /// <summary>
      /// A building block might use building block internally to hold structure
      /// (ExpressionProfile or Population have an internal individual instance).
      /// In this case, this would be the reference to the parent building block
      /// </summary>
      PKSimBuildingBlock OwnedBy { get; set; }
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

      public virtual PKSimBuildingBlock OwnedBy { get; set; }

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

      public virtual void Add(IEntity newChild) => Root.Add(newChild);

      public virtual void RemoveChild(IEntity childToRemove) => Root.RemoveChild(childToRemove);

      public void RemoveChildren() => Root.RemoveChildren();

      public virtual IReadOnlyList<T> GetAllChildren<T>(Func<T, bool> predicate) where T : class, IEntity => Root.GetAllChildren(predicate);

      public virtual IReadOnlyList<T> GetAllChildren<T>() where T : class, IEntity => Root.GetAllChildren<T>();

      public virtual IEnumerable<T> GetChildren<T>(Func<T, bool> predicate) where T : class, IEntity => Root.GetChildren(predicate);

      public virtual IEnumerable<T> GetChildren<T>() where T : class, IEntity => Root.GetChildren<T>();

      public virtual IReadOnlyList<IContainer> GetNeighborsFrom(IReadOnlyList<INeighborhood> neighborhoods) => Root.GetNeighborsFrom(neighborhoods);

      public virtual IReadOnlyList<INeighborhood> GetNeighborhoods(IReadOnlyList<INeighborhood> neighborhoods) => Root.GetNeighborhoods(neighborhoods);

      public virtual IReadOnlyList<TContainer> GetAllContainersAndSelf<TContainer>() where TContainer : class, IContainer =>
         Root.GetAllContainersAndSelf<TContainer>();

      public virtual IReadOnlyList<TContainer> GetAllContainersAndSelf<TContainer>(Func<TContainer, bool> predicate)
         where TContainer : class, IContainer => Root.GetAllContainersAndSelf(predicate);

      public virtual ContainerType ContainerType
      {
         get => Root.ContainerType;
         set => Root.ContainerType = value;
      }

      public string ContainerTypeAsString => Root.ContainerTypeAsString;

      public virtual ContainerMode Mode
      {
         get => Root.Mode;
         set => Root.Mode = value;
      }

      public virtual IReadOnlyList<IEntity> Children => Root.Children;
      
      public ObjectPath ParentPath { get; set; }

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
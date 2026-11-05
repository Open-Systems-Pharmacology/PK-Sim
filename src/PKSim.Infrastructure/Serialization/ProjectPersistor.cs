using System.Collections.Generic;
using System.Linq;
using NHibernate;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.Mappers;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Serialization;

namespace PKSim.Infrastructure.Serialization
{
   public interface IProjectPersistor : ISessionPersistor<PKSimProject>
   {
      /// <summary>
      ///    After successful serialization, some flags can be reseted to avoid saving values that were already saved in the
      ///    database
      /// </summary>
      void UpdateProjectAfterSave(PKSimProject project);
   }

   public class ProjectPersistor : IProjectPersistor
   {
      private readonly IProjectToProjectMetaDataMapper _projectToProjectMetaDataMapper;
      private readonly IProjectMetaDataToProjectMapper _projectMetaDataToProjectMapper;
      private readonly ICompressedSerializationManager _serializationManager;
      private readonly IObjectTypeResolver _objectTypeResolver;

      public ProjectPersistor(
         IProjectToProjectMetaDataMapper projectToProjectMetaDataMapper,
         IProjectMetaDataToProjectMapper projectMetaDataToProjectMapper,
         ICompressedSerializationManager serializationManager,
         IObjectTypeResolver objectTypeResolver)
      {
         _projectToProjectMetaDataMapper = projectToProjectMetaDataMapper;
         _projectMetaDataToProjectMapper = projectMetaDataToProjectMapper;
         _serializationManager = serializationManager;
         _objectTypeResolver = objectTypeResolver;
      }

      public void Save(PKSimProject projectToSave, ISession session)
      {
         var projectMetaData = projectMetaDataFrom(projectToSave);
         var projectFromDb = projectFromDatabase(session);

         ensureNewBuildingBlocksHaveContent(projectToSave, projectMetaData, projectFromDb);

         if (projectFromDb == null)
            saveProject(session, projectMetaData);
         else
            projectFromDb.UpdateFrom(projectMetaData, session);
      }

      /// <summary>
      ///    The content of a building block is only serialized when the building block has changed. A building block that
      ///    is not in the database yet is inserted as is, so it would land without content if it was not flagged as changed
      ///    (see issue 3861). A project file containing such a building block cannot be opened anymore. We make sure here
      ///    that every new building block is written with its content, or refuse to save the project.
      /// </summary>
      private void ensureNewBuildingBlocksHaveContent(PKSimProject projectToSave, ProjectMetaData projectMetaData, ProjectMetaData projectFromDb)
      {
         var idsOfBuildingBlocksInDb = new HashSet<string>(projectFromDb?.BuildingBlocks?.Select(x => x.Id) ?? Enumerable.Empty<string>());
         var newBuildingBlocksWithoutContent = projectMetaData.BuildingBlocks
            .Where(x => x.Content?.Data == null)
            .Where(x => !idsOfBuildingBlocksInDb.Contains(x.Id))
            .ToList();

         newBuildingBlocksWithoutContent.Each(x => serializeContentFor(x, projectToSave.BuildingBlockById(x.Id)));
      }

      private void serializeContentFor(BuildingBlockMetaData buildingBlockMetaData, IPKSimBuildingBlock buildingBlock)
      {
         if (buildingBlock == null || !buildingBlock.IsLoaded)
            throw new PKSimException(PKSimConstants.Error.CannotSaveBuildingBlockWithoutContent(buildingBlockMetaData.Name, _objectTypeResolver.TypeFor(buildingBlock)));

         //same compression as in the mapper: population data is mostly double arrays and is not compressed
         var compress = !buildingBlock.IsAnImplementationOf<Population>();
         buildingBlockMetaData.Content.Data = _serializationManager.Serialize(buildingBlock, compress);
      }

      private static void saveProject(ISession session, ProjectMetaData projectMetaData)
      {
         session.Save(projectMetaData);
      }

      public PKSimProject Load(ISession session)
      {
         var projectFromDb = projectFromDatabase(session);
         if (projectFromDb == null)
            throw new InvalidProjectFileException();

         if (ProjectVersions.CanLoadVersion(projectFromDb.Version))
            return projectFrom(projectFromDb);

         //Project was created with a newer version of the software
         throw new InvalidProjectVersionException(projectFromDb.Version);
      }

      public void UpdateProjectAfterSave(PKSimProject project)
      {
         foreach (var simulation in project.All<Simulation>())
         {
            simulation.ResultsHaveChanged = false;
         }
      }

      private ProjectMetaData projectFromDatabase(ISession session)
      {
         var projectsFromDb = session.CreateCriteria<ProjectMetaData>().List<ProjectMetaData>();

         //Project is corrupt or is not a pk-sim project
         if (projectsFromDb.Count == 0)
            return null;

         return projectsFromDb[0];
      }

      private ProjectMetaData projectMetaDataFrom(PKSimProject project)
      {
         return _projectToProjectMetaDataMapper.MapFrom(project);
      }

      private PKSimProject projectFrom(ProjectMetaData projectMetaData)
      {
         return _projectMetaDataToProjectMapper.MapFrom(projectMetaData);

      }
   }
}
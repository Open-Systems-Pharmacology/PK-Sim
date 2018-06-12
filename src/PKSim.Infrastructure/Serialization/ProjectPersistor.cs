using NHibernate;
using PKSim.Core;
using PKSim.Core.Model;
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

      public ProjectPersistor(IProjectToProjectMetaDataMapper projectToProjectMetaDataMapper, 
         IProjectMetaDataToProjectMapper projectMetaDataToProjectMapper)
      {
         _projectToProjectMetaDataMapper = projectToProjectMetaDataMapper;
         _projectMetaDataToProjectMapper = projectMetaDataToProjectMapper;
      }

      public void Save(PKSimProject projectToSave, ISession session)
      {
         var projectMetaData = projectMetaDataFrom(projectToSave);
         var projectFromDb = projectFromDatabase(session);

         if (projectFromDb == null)
            saveProject(session, projectMetaData);
         else
            projectFromDb.UpdateFrom(projectMetaData, session);
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
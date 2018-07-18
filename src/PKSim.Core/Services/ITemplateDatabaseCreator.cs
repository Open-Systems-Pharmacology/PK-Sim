namespace PKSim.Core.Services
{
   public interface ITemplateDatabaseCreator
   {
      /// <summary>
      /// Creates a default user template database if not already available for the current user
      /// </summary>
      void CreateDefaultTemplateDatabase();
   }
}
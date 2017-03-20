namespace PKSim.Infrastructure.ORM.Core
{
   public class UpdatableLinkDbConfiguration
   {
      /// <summary>
      /// Full path to front end protein expressions database.
      /// </summary>
      public string FrontEndDatabase { get; set; }

      /// <summary>
      /// Full path to back end protein expressions database.
      /// </summary>
      public string BackEndDatabase { get; set; }
   }
}
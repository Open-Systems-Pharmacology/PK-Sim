using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class RemoteTemplateRepository : StartableRepository<RemoteTemplate>, IRemoteTemplateRepository
   {
      private readonly IPKSimConfiguration _configuration;
      private readonly IJsonSerializer _jsonSerializer;
      private readonly ISnapshotTask _snapshotTask;
      private readonly List<RemoteTemplate> _allTemplates = new List<RemoteTemplate>();
      public string Version { get; private set; }

      public RemoteTemplateRepository(
         IPKSimConfiguration configuration,
         IJsonSerializer jsonSerializer,
         ISnapshotTask snapshotTask)
      {
         _configuration = configuration;
         _jsonSerializer = jsonSerializer;
         _snapshotTask = snapshotTask;
      }

      public IReadOnlyList<RemoteTemplate> AllTemplatesFor(TemplateType templateType)
      {
         Start();
         return _allTemplates.Where(x => x.Type.Is(templateType)).ToList();
      }

      public async Task<T> LoadTemplateAsync<T>(RemoteTemplate remoteTemplate)
      {
         
         var localFile = Path.Combine(_configuration.RemoteTemplateFolderPath, fileNameWithVersionFor(remoteTemplate));
         if (!FileHelper.FileExists(localFile))
            await downloadRemoteFile(remoteTemplate.Url, localFile);

         var buildingBlockType = EnumHelper.ParseValue<PKSimBuildingBlockType>(remoteTemplate.Type.ToString());
         var model = await _snapshotTask.LoadModelFromProjectFileAsync<T>(localFile, buildingBlockType, remoteTemplate.Name);
         return model;
      }

      public Task<IReadOnlyList<RemoteTemplate>> AllReferenceTemplatesForAsync<T>(T loadedTemplate)
      {
         throw new NotImplementedException();
      }

      protected override void DoStart()
      {
         var snapshots = Task.Run(() => _jsonSerializer.Deserialize<RemoteTemplates>(_configuration.RemoteTemplateSummaryPath)).Result;
         snapshots.Templates.Each(x => x.DatabaseType = TemplateDatabaseType.Remote);
         _allTemplates.AddRange(snapshots.Templates);
         Version = snapshots.Version;
      }

      public override IEnumerable<RemoteTemplate> All()
      {
         Start();
         return _allTemplates;
      }

      private string fileNameWithVersionFor(RemoteTemplate template)
      {
         var url = template.Url;
         var fileName = new Uri(url).Segments.Last();
         return $"{template.Version}_{fileName}";
      }

      private async Task downloadRemoteFile(string url, string destination)
      {
         using (var wc = new WebClient())
         {
            try
            {
               await wc.DownloadFileTaskAsync(url, destination);
            }
            catch (Exception e)
            {
               //It is required to delete the file that will be created with a size of zero
               FileHelper.DeleteFile(destination);
               throw new OSPSuiteException(PKSimConstants.Error.CannotDownloadTemplateLocatedAt(url), e);
            }
         }
      }
   }
}
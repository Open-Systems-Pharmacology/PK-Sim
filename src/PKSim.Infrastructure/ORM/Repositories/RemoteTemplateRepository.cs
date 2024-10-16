﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
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

      public RemoteTemplate TemplateBy(TemplateType templateType, string name) => AllTemplatesFor(templateType).FindByName(name);

      public async Task<T> LoadTemplateAsync<T>(RemoteTemplate remoteTemplate)
      {
         var localFile = Path.Combine(_configuration.RemoteTemplateFolderPath, fileNameWithVersionFor(remoteTemplate));
         if (!FileHelper.FileExists(localFile))
            await downloadRemoteFile(remoteTemplate.Url, localFile);

         var buildingBlockType = EnumHelper.ParseValue<PKSimBuildingBlockType>(remoteTemplate.Type.ToString());
         return await _snapshotTask.LoadModelFromProjectFileAsync<T>(localFile, buildingBlockType, remoteTemplate.Name);
      }

      public IReadOnlyList<RemoteTemplate> AllReferenceTemplatesFor<T>(RemoteTemplate remoteTemplate, T loadedTemplate)
      {
         //We only have reference templates for very specific building block types. So we filter for those
         switch (loadedTemplate)
         {
            case Compound compound:
               return metabolitesFor(remoteTemplate, compound);
            case ISimulationSubject simulationSubject:
               return expressionProfileFor(remoteTemplate, simulationSubject);
            default:
               return Array.Empty<RemoteTemplate>();
         }
      }

      public async Task UpdateLocalTemplateSummaryFile()
      {
         var tempFile = FileHelper.GenerateTemporaryFileName();
         try
         {
            await downloadRemoteFile(CoreConstants.REMOTE_TEMPLATE_FILE_URL, tempFile);
            FileHelper.Copy(tempFile, _configuration.RemoteTemplateSummaryPath);
         }
         catch (Exception)
         {
            //could not download the file. Do nothing
         }
      }

      private IReadOnlyList<RemoteTemplate> expressionProfileFor(RemoteTemplate remoteTemplate, ISimulationSubject simulationSubject)
      {
         //This will for now assume that the expression profile templates are in the same file
         return simulationSubject.AllExpressionProfiles()
            .Select(x => x.Name)
            .Select(name => new RemoteTemplate
            {
               Url = remoteTemplate.Url,
               Type = TemplateType.ExpressionProfile,
               Name = name
            })
            .ToList();
      }

      private IReadOnlyList<RemoteTemplate> metabolitesFor(RemoteTemplate remoteTemplate, Compound compound)
      {
         //This will for now assume that the metabolite template are in the same file
         return compound.AllProcesses<EnzymaticProcess>()
            .Select(x => x.MetaboliteName)
            .Where(x => x.StringIsNotEmpty())
            .Distinct()
            .Select(meta => new RemoteTemplate
            {
               Url = remoteTemplate.Url,
               Type = TemplateType.Compound,
               Name = meta
            })
            .ToList();
      }

      protected override void DoStart()
      {
         var snapshots = Task.Run(() => _jsonSerializer.Deserialize<RemoteTemplates>(_configuration.RemoteTemplateSummaryPath)).Result;
         
         snapshots.Templates.Each(x =>
         {
            var (version, repositoryUrl) = extractDataFromUrl(x.Url);
            x.Version = version;
            x.RepositoryUrl = repositoryUrl;
         });
         _allTemplates.AddRange(snapshots.Templates);
         Version = snapshots.Version;
      }

      private (string version, string repositoryUrl) extractDataFromUrl(string url)
      {
         var invalidUrl = (CoreConstants.DEFAULT_TEMPLATE_VERSION, url);
         if (string.IsNullOrEmpty(url))
            return invalidUrl;

         //a Url looks like so (5 segments, 1, 2 and 3 of interest for our use case)
         //https://raw.githubusercontent.com/Open-Systems-Pharmacology/Rifampicin-Model/v1.1/Rifampicin-Model.json
         //and we want to create something like this
         //https://github.com/Open-Systems-Pharmacology/Rifampicin-Model/tree/v1.1

         var segments = new Uri(url).Segments;
         //The url does not respect the expected format. Returned the default raw url
         if(segments.Length != 5)
            return invalidUrl;

         var versionSegment = segments[3];
         if(!versionSegment.StartsWith("v") || !versionSegment.EndsWith("/"))
            return invalidUrl;

         //Removes the v at the beginning end the "/" at the ned
         var version = versionSegment.Substring(1, versionSegment.Length -2);
         return (version, $"https://github.com/{segments[1]}{segments[2]}tree/{versionSegment}");
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
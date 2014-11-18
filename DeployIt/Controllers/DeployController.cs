using System;
using System.IO;
using System.Net;
using System.Net.Http;
using DeployIt.Common;
using DeployIt.Hubs;
using DeployIt.Models;
using Microsoft.VisualBasic.FileIO;
using Tasks = System.Threading.Tasks;

namespace DeployIt.Controllers
{
    public class DeployController : ApiHubController<NotificationHub>
    {
        bool _inProgress = true;

        public HttpResponseMessage Get()
        {
            NotifyAndLog("Get: The server time is {0}", DateTime.Now);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public async Tasks.Task<HttpResponseMessage> Post(DeployRequest request)
        {
            try
            {
                if (request == null || request.ProjectId > 0)
                {
                    NotifyAndLog("<p class='red'>Deployment request is rejected because of invalid data. </p>");
                    return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                }

                request.RequestAt = DateTime.Now;

                NotifyAndLog("<p>Deployment request received for project <span class='yellow'>{0}</span>.</p>", request.ProjectName);
                await ProcessDeployment(request);

                request.DeploySuccess = true;
                DocumentSession.Store(request);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _inProgress = false;

                NotifyAndLog("<p class='red'>{0} </p>", ex.Message);
                NotifyAndLog("<p class='red'>{0} </p>", ex.StackTrace);
                NotifyAndLog("<p class='red'>Deployment failed! </p>", ex.StackTrace);
                //todo: rollback process

                if (request != null)
                {
                    request.DeploySuccess = false;
                    DocumentSession.Store(request);
                }

                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            }
        }

        private async Tasks.Task ProcessDeployment(DeployRequest request)
        {
            //Backup
            var backupFolder = Path.Combine(request.DestinationRootLocation, "_Backup",
                string.Format("{0}_{1}", request.DestinationProjectFolder, DateTime.Now.ToString("yyyyMMdd_hhmmss")));
            var projectPath = Path.Combine(request.DestinationRootLocation, request.DestinationProjectFolder);

            await BackupTask(projectPath, backupFolder).WithNotifyProgress(BroadcastProgress);

            //Deployment
            await DeploymentTask(request, projectPath).WithNotifyProgress(BroadcastProgress);

            //copy web.config
            var sourceConfig = Path.Combine(backupFolder, "Web.config");
            var destConfig = Path.Combine(projectPath, "Web.config");

            await CopyConfigFileTask(sourceConfig, destConfig).WithNotifyProgress(BroadcastProgress);

            //update version number
            await UpdateVersionNumberTask(destConfig, request.VersionKeyName, request.NextVersion);

            NotifyAndLog("<p class='greenyellow'>Deployment process completed!</p>");
        }

        private Tasks.Task UpdateVersionNumberTask(string destConfig, string versionKeyName, string value)
        {
            NotifyAndLog("<p>Update application version number [{0}] to <span class='yellow'>{1}</span> </p>", versionKeyName, value);

            return Tasks.Task.Run(() => Helper.SetVersionNumber(destConfig, versionKeyName, value));
        }

        private Tasks.Task CopyConfigFileTask(string sourceConfig, string destConfig)
        {
            NotifyAndLog("<p>Copying <span class='yellow'>{0}</span> to <span class='yellow'>{1}</span> </p>", sourceConfig, destConfig);
            return Tasks.Task.Run(() => FileSystem.CopyFile(sourceConfig, destConfig, true));
        }

        private Tasks.Task DeploymentTask(DeployRequest model, string projectPath)
        {
            var source = Path.Combine(model.BuildDropLocation, model.PublishedWebsiteFolder);
            NotifyAndLog("<p>Deploying files from folder <span class='yellow'>{0}</span> to <span class='yellow'>{1}</span> </p>", source,
                projectPath);

            return Tasks.Task.Run(() => FileSystem.CopyDirectory(source, projectPath, true));
        }

        private Tasks.Task BackupTask(string projectPath, string backupFolder)
        {
            NotifyAndLog("<p>Backup process started.<p/>", projectPath, backupFolder);
            NotifyAndLog("<p>Copying folder <span class='yellow'>{0}</span> to <span class='yellow'>{1}</span> <p/>",
                projectPath, backupFolder);

            return Tasks.Task.Run(() => FileSystem.CopyDirectory(projectPath, backupFolder));
        }

        private void NotifyAndLog(string message, params object[] args)
        {
            var text = string.Format(message, args);

            Broadcast(text);
            DocumentSession.Store(new DeploymentLog
            {
                User = User.Identity.Name,
                LogTime = DateTime.Now,
                Description = text,
            });
        }

        private void BroadcastProgress()
        {
            Broadcast(". ");
        }
    }

    
}

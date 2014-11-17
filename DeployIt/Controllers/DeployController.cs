using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using DeployIt.Common;
using DeployIt.Hubs;
using DeployIt.Models;
using Microsoft.TeamFoundation.Client.Reporting;
using Microsoft.VisualBasic.FileIO;

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

        public HttpResponseMessage Post(DeployRequest request)
        {
            try
            {
                if (request == null || request.ProjectId > 0)
                {
                    NotifyAndLog("Deployment request is rejected because of invalid data");
                    return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                }

                request.RequestAt = DateTime.Now;

                NotifyAndLog("Deployment request received for project '{0}'", request.ProjectName);
                ProcessDeployment(request);

                request.DeploySuccess = true;
                DocumentSession.Store(request);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                NotifyAndLog(ex.Message);
                NotifyAndLog(ex.StackTrace);
                //todo: rollback process

                if (request != null)
                {
                    request.DeploySuccess = false;
                    DocumentSession.Store(request);
                }

                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            }
        }

        private void ProcessDeployment(DeployRequest model)
        {
            //Backup
            var backupFolder = Path.Combine(model.DestinationRootLocation, "_Backup",
                string.Format("{0}_{1}", model.DestinationProjectFolder, DateTime.Now.ToString("yyyyMMdd_hhmmss")));
            var projectPath = Path.Combine(model.DestinationRootLocation, model.DestinationProjectFolder);

            NotifyAndLog("<p>Copying folder {0} to {1} <p/>", projectPath, backupFolder);
            BroadCastProgress();
            FileSystem.CopyDirectory(projectPath, backupFolder);
            _inProgress = false;

            //Copy files to project folder
            var source = Path.Combine(model.BuildDropLocation, model.PublishedWebsiteFolder);

            NotifyAndLog("<p>Deploying files from folder {0} to {1} </p>", source, projectPath);
            BroadCastProgress();
            FileSystem.CopyDirectory(source, projectPath, true);
            _inProgress = false;

            //copy web.config
            var sourceConfig = Path.Combine(backupFolder, "Web.config");
            var destConfig = Path.Combine(projectPath, "Web.config");

            NotifyAndLog("<p>Copying {0} to {1} </p>", sourceConfig, destConfig);
            BroadCastProgress();
            FileSystem.CopyFile(sourceConfig, destConfig, true);
            _inProgress = false;

            //update version number
            NotifyAndLog("<p>Update application version number [{0}] to {1} </p>", model.VersionKeyName, model.NextVersion);
            Helper.SetVersionNumber(destConfig, model.VersionKeyName, model.NextVersion);

            NotifyAndLog("Deployment process completed");
        }

        private void BroadCastProgress()
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                while (_inProgress)
                {
                    Thread.Sleep(1000);
                    Broadcast(". ");
                }
            });
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
    }
}

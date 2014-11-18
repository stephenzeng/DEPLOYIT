﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using DeployIt.Common;
using DeployIt.Hubs;
using DeployIt.Models;
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
                    NotifyAndLog("<p class='red'>Deployment request is rejected because of invalid data. </p>");
                    return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                }

                request.RequestAt = DateTime.Now;

                NotifyAndLog("<p>Deployment request received for project <span class='yellow'>{0}</span>.</p>", request.ProjectName);
                ProcessDeployment(request);

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

        private void ProcessDeployment(DeployRequest model)
        {
            //Backup
            var backupFolder = Path.Combine(model.DestinationRootLocation, "_Backup",
                string.Format("{0}_{1}", model.DestinationProjectFolder, DateTime.Now.ToString("yyyyMMdd_hhmmss")));
            var projectPath = Path.Combine(model.DestinationRootLocation, model.DestinationProjectFolder);

            NotifyAndLog("<p>Backup process started.<p/>", projectPath, backupFolder);
            NotifyAndLog("<p>Copying folder <span class='yellow'>{0}</span> to <span class='yellow'>{1}</span> <p/>", projectPath, backupFolder);
            BroadCastProgress();
            FileSystem.CopyDirectory(projectPath, backupFolder);
            _inProgress = false;

            //Copy files to project folder
            var source = Path.Combine(model.BuildDropLocation, model.PublishedWebsiteFolder);

            NotifyAndLog("<p>Deploying files from folder <span class='yellow'>{0}</span> to <span class='yellow'>{1}</span> </p>", source, projectPath);
            BroadCastProgress();
            FileSystem.CopyDirectory(source, projectPath, true);
            _inProgress = false;

            //copy web.config
            var sourceConfig = Path.Combine(backupFolder, "Web.config");
            var destConfig = Path.Combine(projectPath, "Web.config");

            NotifyAndLog("<p>Copying <span class='yellow'>{0}</span> to <span class='yellow'>{1}</span> </p>", sourceConfig, destConfig);
            FileSystem.CopyFile(sourceConfig, destConfig, true);

            //update version number
            NotifyAndLog("<p>Update application version number [{0}] to <span class='yellow'>{1}</span> </p>", model.VersionKeyName, model.NextVersion);
            Helper.SetVersionNumber(destConfig, model.VersionKeyName, model.NextVersion);

            NotifyAndLog("<p class='greenyellow'>Deployment process completed!</p>");
        }

        private void BroadCastProgress()
        {
            _inProgress = true;

            System.Threading.Tasks.Task.Run(async () =>
            {
                while (_inProgress)
                {
                    await System.Threading.Tasks.Task.Delay(1000);
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

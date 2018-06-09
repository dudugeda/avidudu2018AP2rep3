﻿using Communication.Client;
using ImageService.Infastructure;
using ImageService.Infastructure.Event;
using ImageService.Infrastructure.Enums;
using ImageService.Infrastructure.Event;
using ImageService.Logging.Modal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication2.Models.MainTab
{
    public class MainTabModel : IMainTabModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        #region Client communication data members
        private Client LocalClient;
        private bool AnswerRecieved;
        private string Answer;
        public event EventHandler<CommandRecievedEventArgs> SendCommand;
        #endregion
        /// <summary>
        /// Lets the viewmodel know that a property has changed.
        /// </summary>
        /// <param name="propertyName">The property that has changed.</param>
        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Constructor,
        /// gets a client and loads data from it
        /// </summary>
        public MainTabModel()
        {
            LocalClient = Client.GetInstance;
        }
        /// <summary>
        /// Function that is called when a command has been completed by the server,
        /// </summary>
        /// <param name="sender">The client.</param>
        /// <param name="args">The data that was sent by the client.</param>
        public void CommandDone(object sender, CommandDoneEventArgs args)
        {
            JObject JSONObjectLogs = JObject.Parse(args.Message);
            if ((string)JSONObjectLogs["type"] == "log")//TODO: what is the type
            {
                Answer = args.Message;
                AnswerRecieved = true;
            }
        }
    }
}
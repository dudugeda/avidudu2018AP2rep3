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

namespace ImageService.GUI
{
    /// <summary>
    /// The model for the logs tab.
    /// Stores data and executes function on that data.
    /// Namely, it stores a list of logs and refreshes it when told to do so.
    /// </summary>
    class LogsTabModel : ILogsTabModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<LogData> ModelLogList;
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
        /// The list of logs that will be displayed in the window.
        /// </summary>
        public ObservableCollection<LogData> LogList
        {
            get
            {
                return ModelLogList;
            }
            set
            {
                ModelLogList = value;
                NotifyPropertyChanged("LogList");
            }
        }
        /// <summary>
        /// Constructor,
        /// gets a client and loads data from it
        /// (via the RefreshButtonPress function).
        /// </summary>
        public LogsTabModel()
        {
            ModelLogList = new ObservableCollection<LogData>();
            LocalClient = Client.GetInstance;
            RefreshButtonPress();
        }
        /// <summary>
        /// Function that is called when a command has been completed by the server,
        /// if the action was a log request, the logs screen responds to it by parsing the logs and displaying them.
        /// </summary>
        /// <param name="sender">The client.</param>
        /// <param name="args">The data that was sent by the client.</param>
        public void CommandDone(object sender, CommandDoneEventArgs args)
        {
            JObject JSONObjectLogs = JObject.Parse(args.Message);
            if ((string)JSONObjectLogs["type"] == "log")
            {
                Answer = args.Message;
                AnswerRecieved = true;
            }
        }
        /// <summary>
        /// Adds a log to the log list.
        /// </summary>
        /// <param name="type">The type of log, 0 for INFO, 1 for ERROR, 2 for WARNING</param>
        /// <param name="messege">The messege itself that was logged.</param>
        private void AddLog(MessageTypeEnum type, string messege)
        {
            ModelLogList.Add(new LogData(type, messege));
        }
        /// <summary>
        /// A function that is executed when the user presses the F5 button in the GUI,
        /// and also once in the constructor.
        /// Clears the log list and reloads it from the server (via the client).
        /// </summary>
        public void RefreshButtonPress()
        {
            ModelLogList.Clear();
            if (!LocalClient.GetStatus())
            {
                AddLog(MessageTypeEnum.FAIL, "Connection error, failed to load data.");
                return;
            }
            SendCommand += LocalClient.CommandRecieved;
            LocalClient.CommandDone += CommandDone;
            CommandRecievedEventArgs args = new CommandRecievedEventArgs((int)CommandEnum.LogCommand, null, null);
            AnswerRecieved = false;
            SendCommand(this, args);
            while (!AnswerRecieved);
            JObject obj = JObject.Parse(Answer);
            List<ISPair> htmlAttributes = JsonConvert.DeserializeObject<List<ISPair>>((string)obj["Dict"]);
            foreach (ISPair entry in htmlAttributes)
            {
                AddLog(entry.Type, entry.Message);
            }
        }
    }
}

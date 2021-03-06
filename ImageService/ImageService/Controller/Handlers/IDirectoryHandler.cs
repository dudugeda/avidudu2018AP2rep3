﻿using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure.Event;
using ImageService.Infastructure.Event;

namespace ImageService.Controller.Handlers
{
    /// <summary>
    /// An interface for classes that handle directories.
    /// </summary>
    public interface IDirectoryHandler
    {
        /// <summary>
        /// The Event That Notifies that the Directory is being closed
        /// </summary>
        event EventHandler<LogChangedEventArgs> LogChanged;
        /// <summary>
        /// method to start listening on a given directory
        /// </summary>
        /// <param name="dirPath">path to given directory</param>
        /// <returns>false if an error occured, true otherwise</returns>
        bool StartHandleDirectory(string dirPath);                               // The Function Recieves the directory to Handle
        /// <summary>
        /// method to be activated when command enters
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="e"> args for the command</param>
        void OnCommandRecieved(object sender, ClientCommandEventArgs e);     // The Event that will be activated upon new Command
        event EventHandler<CommandDoneEventArgs> CommandDone;
    }
}

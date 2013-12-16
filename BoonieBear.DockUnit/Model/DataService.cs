// ***********************************************************************
// Assembly         : BoonieBear.DockUnit
// Author           : Fuxiang
// Created          : 12-10-2013
//
// Last Modified By : Fuxiang
// Last Modified On : 12-11-2013
// ***********************************************************************
// <copyright file="DataService.cs" company="BoonieBear">
//     Copyright (c) BoonieBear. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

/// <summary>
/// The Model namespace.
/// </summary>
namespace BoonieBear.DockUnit.Model
{
    /// <summary>
    /// Class DataService.
    /// </summary>
    public class DataService : IDataService
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to connect to the actual data service

            var item = new DataItem("Welcome to MVVM Light");
            callback(item, null);
        }
    }
}
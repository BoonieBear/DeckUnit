// ***********************************************************************
// Assembly         : BoonieBear.DockUnit
// Author           : Fuxiang
// Created          : 12-10-2013
//
// Last Modified By : Fuxiang
// Last Modified On : 12-10-2013
// ***********************************************************************
// <copyright file="IDataService.cs" company="BoonieBear">
//     Copyright (c) BoonieBear. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// The Model namespace.
/// </summary>
namespace BoonieBear.DockUnit.Model
{
    /// <summary>
    /// Interface IDataService
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="callback">The callback.</param>
        void GetData(Action<DataItem, Exception> callback);
    }
}

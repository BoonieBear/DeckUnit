// ***********************************************************************
// Assembly         : BoonieBear.DockUnit
// Author           : Fuxiang
// Created          : 12-10-2013
//
// Last Modified By : Fuxiang
// Last Modified On : 12-10-2013
// ***********************************************************************
// <copyright file="ViewModelLocator.cs" company="BoonieBear">
//     Copyright (c) BoonieBear. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:BoonieBear.DockUnit.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using BoonieBear.DockUnit.Model;

/// <summary>
/// The ViewModel namespace.
/// </summary>
namespace BoonieBear.DockUnit.ViewModel
{
    /// <summary>
    /// Class ViewModelLocator.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes static members of the <see cref="ViewModelLocator"/> class.
        /// </summary>
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        /// <value>The main.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}
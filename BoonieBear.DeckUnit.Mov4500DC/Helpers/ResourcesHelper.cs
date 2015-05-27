using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace BoonieBear.DeckUnit.Mov4500UI.Helpers
{
    /// <summary>
    /// Resources.resx helper helps collection how many language packages there
    /// are during runtime, provide cultural varied strings according to their
    /// keys, change current culture and remember current culture.
    /// </summary>
    public class ResourcesHelper
    {
        #region Properties
        /// <summary>
        /// All members are static, this variable makes sure every member is
        /// initialized only once.
        /// </summary>
        private static bool _isFoundInstalledCultures = false;
        /// <summary>
        /// Static property ResourceProvider is actually defined in App.xaml,
        /// a resource named 'Resources'.
        /// </summary>
        private static ObjectDataProvider _objectDataProvider;
        public static ObjectDataProvider ResourceProvider
        {
            get
            {
                if (_objectDataProvider == null)
                {
                    _objectDataProvider = (ObjectDataProvider)Application.Current
                        .FindResource("Resources");
                }
                return _objectDataProvider;
            }
        }
        /// <summary>
        /// en-US is used at design time.
        /// </summary>
        private static CultureInfo _designTimeCulture =
            new CultureInfo("en-US");
        /// <summary>
        /// Culture collection that this software supported right now.
        /// </summary>
        private static List<CultureInfo> _supportedCultures =
            new List<CultureInfo>();
        public static List<CultureInfo> SupportedCultures
        {
            get { return _supportedCultures; }
        }
        #endregion //Properties

        #region Constructors
        /// <summary>
        /// Actually the App.xaml creates the object of this class as a
        /// resource.
        /// </summary>
        public ResourcesHelper()
        {
            if (!_isFoundInstalledCultures)
            {
                CultureInfo cultureInfo = new CultureInfo("");
                /*
                 * find resources dlls in the startup directory.
                 */
                CultureInfo[] allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
                HashSet<string> cultureNameSet = new HashSet<string>();
                foreach (var item in allCultures)
                {
                    cultureNameSet.Add(item.Name);
                }
                foreach (string dir in Directory.GetDirectories(System.Windows
                    .Forms.Application.StartupPath))
                {
                    try
                    {
                        DirectoryInfo dirinfo = new DirectoryInfo(dir);
                        if (!cultureNameSet.Contains(dirinfo.Name))
                        {
                            continue;
                        }
                        cultureInfo = CultureInfo.GetCultureInfo(dirinfo.Name);
                        //if (dirinfo.GetFiles(Path.GetFileNameWithoutExtension
                        //(System.Windows.Forms.Application.ExecutablePath)
                        //+ ".resources.dll").Length > 0)
                        if (cultureInfo != null)
                        {
                            _supportedCultures.Add(cultureInfo);
                        }
                    }
                    catch (ArgumentException ex)
                    {
                    }
                }
                /*
                 * Set default culture
                 */
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    Properties.Resources.Culture = _designTimeCulture;
                    //Properties.Settings.Default.DefaultCulture =
                    //    _designTimeCulture;
                }
                else
                {
                    CultureInfo ci = new CultureInfo("en-US");;
                    if (ci == null)
                    {
                        ci = CultureInfo.GetCultureInfo("en-US");
                    }
                    Properties.Resources.Culture = ci;
                }
                _isFoundInstalledCultures = true;
            }
        }
        #endregion //Constructors

        #region APIs
        /// <summary>
        /// Provide Resources object
        /// </summary>
        /// <returns></returns>
        public Properties.Resources GetResourceInstance()
        {
            return new Properties.Resources();
        }
        /// <summary>
        /// Hasn't been used yet.
        /// </summary>
        /// <param name="culture"></param>
        public Properties.Resources GetResourceInstance(string cultureName)
        {
            ChangeCulture(new CultureInfo(cultureName));

            return new Properties.Resources();
        }
        /// <summary>
        /// Change culture and store selected culture.
        /// </summary>
        /// <param name="culture"></param>
        public static void ChangeCulture(CultureInfo culture)
        {
            if (_supportedCultures.Contains(culture))
            {
                //Properties.Resources.Culture = culture;
                //Save culture...
                //ResourceProvider.Refresh();
                //CultureChanged();
            }
        }
        public static CultureInfo CurrentCulture
        {
            get { return Properties.Resources.Culture; }
        }
        public static string TryFindResourceString(string key, CultureInfo ci = null)
        {
            if (key == null)
            {
                return null;
            }
            if (ci == null)
            {
                ci = Properties.Resources.Culture;
            }
            string value = Properties.Resources.ResourceManager.GetString(key, ci);
            return value;
        }
        /// <summary>
        /// Try to get resource string with the key, if there's no value,
        /// return the default value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string TryFindResourceString(string key, string defaultValue, CultureInfo ci = null)
        {
            if (key == null)
            {
                return defaultValue;
            }
            if (ci == null)
            {
                ci = Properties.Resources.Culture;
            }
            string value = Properties.Resources.ResourceManager.GetString(key, ci);
            if (value == null)
            {
                return defaultValue;
            }
            return value;
        }
        #endregion //APIs

        #region Culture Changed Notification
        private static List<ICultureChanged> _notifyChangedList = new List<ICultureChanged>();
        public static bool RegisterCultureChanged(ICultureChanged handler)
        {
            try
            {
                _notifyChangedList.Add(handler);
                return true;
            }
            catch (Exception e)
            {
                //Logger.LogException(e);
                return false;
            }
        }
        private static void CultureChanged()
        {
            foreach (var item in _notifyChangedList)
            {
                item.CultureChanged();
            }
        }
        #endregion Culture Changed Notification
    }
    public interface ICultureChanged
    {
        void CultureChanged();
    }
}

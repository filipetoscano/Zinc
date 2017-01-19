using Platinum;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Zinc.WebServices.Description
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationDescription
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Application Build( string application, Assembly assembly )
        {
            #region Validations

            if ( application == null )
                throw new ArgumentNullException( nameof( application ) );

            if ( assembly == null )
                throw new ArgumentNullException( nameof( assembly ) );

            #endregion


            /*
             * 
             */
            Application app = new Application();
            app.Name = application;
            app.Services = new List<Service>();
            app.Models = new List<ServiceType>();


            /*
             * TODO:
             */

            return app;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Application Build()
        {
            string app = App.Name;
            Assembly assembly = Assembly.GetEntryAssembly();

            return Build( app, assembly );
        }
    }
}

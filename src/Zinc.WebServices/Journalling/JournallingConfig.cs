using Platinum.Configuration;
using System;
using System.Linq;
using Zinc.WebServices.ServiceModel;

namespace Zinc.WebServices.Journalling
{
    public partial class JournallingConfig
    {
        public static JournallingConfig For( WcfExecutionContext ctx )
        {
            #region Validations

            if ( ctx == null )
                throw new ArgumentNullException( nameof( ctx ) );

            #endregion


            /*
             * 
             */
            var cs = AppConfiguration.SectionGet<JournallingConfigurationSection>( "zinc.webServices/logging" );

            var m = cs.Methods.FirstOrDefault( x => x.Action == ctx.Action );

            if ( m == null )
            {
                var allOk = new JournallingConfig();
                allOk.Enabled = true;
                allOk.Request = new JournallingMessageConfig() { Journal = true };
                allOk.Response = new JournallingMessageConfig() { Journal = true };

                return allOk;
            }

            return m;
        }
    }
}

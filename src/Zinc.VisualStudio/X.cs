using Platinum.VisualStudio;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Zinc.VisualStudio
{
    /// <summary />
    internal class X
    {
        /// <summary />
        internal static XslCompiledTransform LoadXslt( string name )
        {
            #region Validations

            if ( name == null )
                throw new ArgumentNullException( nameof( name ) );

            #endregion

            string fullName = "Zinc.VisualStudio.Resources." + name;

            XsltSettings settings = new XsltSettings( true, true );
            XmlResolver resolver = new XmlUrlResolver();
            XslCompiledTransform xsl = new XslCompiledTransform();

            Stream xsltStream = typeof( X ).Assembly.GetManifestResourceStream( fullName );

            if ( xsltStream == null )
                throw new ToolException( $"Resource file '{ fullName }' not found." );

            using ( XmlReader xr = XmlReader.Create( xsltStream ) )
            {
                xsl.Load( xr, settings, resolver );
            }

            return xsl;
        }


        /// <summary />
        internal static string ToText( XslCompiledTransform xslt, XsltArgumentList args, XmlDocument xdoc )
        {
            #region Validations

            if ( xslt == null )
                throw new ArgumentNullException( nameof( xslt ) );

            if ( xdoc == null )
                throw new ArgumentNullException( nameof( xdoc ) );

            #endregion

            XPathNavigator xpNav = xdoc.DocumentElement.CreateNavigator();
            StringBuilder sb = new StringBuilder();

            using ( TextWriter writer = new StringWriter( sb, CultureInfo.InvariantCulture ) )
            {
                try
                {
                    xslt.Transform( xpNav, args, writer );
                }
                catch ( Exception ex )
                {
                    throw new ToolException( "Failed to apply XSLT transformation", ex );
                }
            }

            return sb.ToString();
        }
    }
}

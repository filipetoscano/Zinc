using Platinum.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using Zinc.WebServices.ProxyGenerator.Properties;

namespace Zinc.WebServices.ProxyGenerator
{
    public class Program
    {
        static int Main( string[] args )
        {
            var cl = CommandLine.Parse( args );

            if ( cl == null )
                return -1;


            /*
             * 
             */
            string assemblyPath = Path.Combine( Environment.CurrentDirectory, cl.Assembly );

            if ( File.Exists( assemblyPath ) == false )
            {
                Konsole.Fatal( "ERR: File '{0}' not found.", cl.Assembly );
                return 1;
            }


            /*
             * 
             */
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler( ( sender, eventArgs ) =>
            {
                string folderPath = Path.GetDirectoryName( assemblyPath );
                string depPath = Path.Combine( folderPath, new AssemblyName( eventArgs.Name ).Name + ".dll" );

                if ( File.Exists( depPath ) == false )
                    return null;

                Konsole.Debug( "need: '{0}'", depPath );
                return Assembly.LoadFrom( depPath );
            } );


            /*
             * 
             */
            Konsole.Debug( "load: '{0}'", assemblyPath );
            Assembly assembly;

            try
            {
                assembly = Assembly.LoadFile( assemblyPath );
            }
            catch ( FileLoadException ex )
            {
                Konsole.Fatal( ex, "ERR: Could not load assembly '{0}'.", assemblyPath );
                return 2;
            }
            catch ( BadImageFormatException ex )
            {
                Konsole.Fatal( ex, "ERR: Could not load assembly '{0}'.", assemblyPath );
                return 3;
            }


            /*
             * 
             */
            XmlDocument doc = new XmlDocument();

            var module = doc
                .AddElement( "module" )
                .AddAttribute( "name", cl.Application );


            /*
             * 
             */
            var controllers = assembly.GetExportedTypes().Where( type => typeof( ApiController ).IsAssignableFrom( type ) );
            List<Type> types = new List<Type>();

            foreach ( var c in controllers )
            {
                var service = module
                    .AddElement( "service" )
                    .AddAttribute( "name", c.Name.Substring( 0, c.Name.Length - "Controller".Length ) )
                    .AddAttribute( "ref", ToReference( c ) );

                foreach ( var m in c.GetMethods( BindingFlags.Public | BindingFlags.Instance ) )
                {
                    var post = m.GetCustomAttribute<HttpPostAttribute>();

                    if ( post == null )
                        continue;

                    var method = service
                        .AddElement( "method" )
                        .AddAttribute( "name", m.Name );

                    Type requestType = m.GetParameters()[ 0 ].ParameterType;
                    Type responseType = m.ReturnType.GetGenericArguments()[ 0 ];

                    var request = method.AddElement( "request" ).AddAttribute( "ref", ToReference( requestType ) );
                    EmitTypeDefinition( request, requestType );
                    WalkTypes( types, requestType );

                    var response = method.AddElement( "response" ).AddAttribute( "ref", ToReference( responseType ) );
                    EmitTypeDefinition( response, responseType );
                    WalkTypes( types, responseType );
                }
            }


            /*
             * Emit all of the used types.
             */
            var typesDef = module.AddElement( "types" );

            foreach ( var t in types )
            {
                var xml = t.GetCustomAttribute<XmlTypeAttribute>();

                if ( xml == null )
                    Console.WriteLine( "warn: type '{0}' does not have XmlType attribute.", t.FullName );

                if ( t.IsEnum == true )
                {
                    var enumDef = typesDef.AddElement( "enumeration" )
                        .AddAttribute( "name", t.Name )
                        .AddAttribute( "ref", ToReference( t ) );

                    if ( xml != null )
                        enumDef.AddAttribute( "ns", xml.Namespace );

                    foreach ( string n in t.GetEnumNames() )
                        enumDef.AddElement( "enum" ).AddAttribute( "value", n );
                }
                else
                {
                    var typeDef = typesDef.AddElement( "type" )
                        .AddAttribute( "name", t.Name )
                        .AddAttribute( "ref", ToReference( t ) );

                    if ( xml != null )
                        typeDef.AddAttribute( "ns", xml.Namespace );

                    EmitTypeDefinition( typeDef, t );
                }
            }


            /*
             * Add documentation
             */
            Dictionary<string, XmlDocument> xmldocs = new Dictionary<string, XmlDocument>();

            foreach ( XmlElement elem in doc.SelectNodes( " //*[ @ref ] " ) )
            {
                string[] tref = elem.Attributes[ "ref" ].Value.Split( ',' );

                XmlDocument xmldoc = LoadDocumentation( xmldocs, Path.GetDirectoryName( assemblyPath ), tref[ 1 ] );

                if ( xmldoc == null )
                    continue;

                if ( elem.LocalName == "service" )
                {
                    foreach ( XmlElement method in elem.SelectNodes( " method " ) )
                    {
                        var memberName = "M:" + tref[ 0 ] + "." + method.Attributes[ "name" ].Value;
                        var summaryNode = xmldoc.SelectSingleNode( $" /doc/members/member[ starts-with( @name, '{ memberName }' ) ]/summary " );

                        if ( summaryNode != null )
                        {
                            string summary = summaryNode.InnerText.Trim();

                            if ( summary.Length > 0 )
                                method.AddElement( "summary" ).InnerText = summary;
                        }
                    }
                }

                if ( elem.LocalName == "request" || elem.LocalName == "response" || elem.LocalName == "type" )
                {
                    foreach ( XmlElement property in elem.SelectNodes( " p " ) )
                    {
                        string memberName = "P:" + tref[ 0 ] + "." + property.Attributes[ "name" ].Value;
                        var summaryNode = xmldoc.SelectSingleNode( $" /doc/members/member[ starts-with( @name, '{ memberName }' ) ]/summary " );

                        if ( summaryNode != null )
                        {
                            string summary = summaryNode.InnerText.Trim();

                            if ( summary.Length > 0 )
                                property.AddElement( "summary" ).InnerText = summary;
                        }
                    }
                }
            }

            // doc.Save( Console.Out );


            /*
             * 
             */
            XslCompiledTransform xslt = new XslCompiledTransform();

            using ( StringReader sr = new StringReader( Resources.ToClient ) )
            {
                using ( XmlReader xr = XmlReader.Create( sr ) )
                {
                    xslt.Load( xr );
                }
            }


            /*
             * 
             */
            string output = Path.Combine( Environment.CurrentDirectory, cl.Output );
            Konsole.Info( "Generating {0}...", output );

            using ( TextWriter xw = new StreamWriter( output, false, Encoding.UTF8 ) )
            {
                XsltArgumentList xargs = new XsltArgumentList();
                xargs.AddParam( "Namespace", "", cl.Namespace );
                xargs.AddParam( "Async", "", cl.Async );
                xargs.AddParam( "Sync", "", cl.Sync );

                xargs.AddExtensionObject( "urn:eo-util", new XsltExtensionObject() );

                try
                {
                    xslt.Transform( doc, xargs, xw );
                }
                catch ( Exception ex )
                {
                    Konsole.Fatal( ex );
                }
            }

            return 0;
        }


        private static XmlDocument LoadDocumentation( Dictionary<string, XmlDocument> xmldocs, string path, string assembly )
        {
            #region Validations

            if ( xmldocs == null )
                throw new ArgumentNullException( nameof( xmldocs ) );

            if ( path == null )
                throw new ArgumentNullException( nameof( path ) );

            if ( assembly == null )
                throw new ArgumentNullException( nameof( assembly ) );

            #endregion


            /*
             * 
             */
            if ( xmldocs.ContainsKey( assembly ) == true )
                return xmldocs[ assembly ];


            /*
             * 
             */
            string file = Path.Combine( path, assembly + ".xml" );
            

            if ( File.Exists( file ) == false )
            {
                Konsole.Debug( "miss: '{0}'", file );

                xmldocs.Add( assembly, null );
                return null;
            }


            /*
             * 
             */
            Konsole.Debug( "xmld: '{0}'", file );

            XmlDocument doc = new XmlDocument();
            doc.Load( file );

            xmldocs.Add( assembly, doc );

            return doc;
        }


        private static string ToReference( Type type )
        {
            #region Validations

            if ( type == null )
                throw new ArgumentNullException( nameof( type ) );

            #endregion

            string assembly = type.Assembly.FullName;
            assembly = assembly.Substring( 0, assembly.IndexOf( ',' ) );

            return type.FullName + "," + assembly;
        }


        private static void WalkTypes( List<Type> all, Type type )
        {
            #region Validations

            if ( all == null )
                throw new ArgumentNullException( nameof( all ) );

            if ( type == null )
                throw new ArgumentNullException( nameof( type ) );

            #endregion

            foreach ( var p in type.GetProperties() )
            {
                Type propertyType = p.PropertyType;

                if ( propertyType.IsArray == true )
                    propertyType = propertyType.GetElementType();

                if ( propertyType.IsNullable() == true )
                    propertyType = propertyType.GetGenericArguments()[ 0 ];

                if ( propertyType.IsCustomClass() == true )
                {
                    if ( all.Contains( propertyType ) == false )
                    {
                        all.Add( propertyType );
                        WalkTypes( all, propertyType );
                    }
                }

                if ( propertyType.IsEnum == true )
                {
                    if ( all.Contains( propertyType ) == false )
                        all.Add( propertyType );
                }
            }
        }



        private static void EmitTypeDefinition( XmlElement typeDef, Type type )
        {
            #region Validations

            if ( typeDef == null )
                throw new ArgumentNullException( nameof( typeDef ) );

            if ( type == null )
                throw new ArgumentNullException( nameof( type ) );

            #endregion

            foreach ( var p in type.GetProperties() )
            {
                string typeName = ToTypeName( p.PropertyType );
                var xml = p.GetCustomAttribute<XmlElementAttribute>();

                var pe = typeDef.AddElement( "p" );
                pe.AddAttribute( "name", p.Name );
                pe.AddAttribute( "type", typeName );

                if ( xml != null && xml.DataType != null )
                    pe.AddAttribute( "spec", xml.DataType );
            }
        }


        private static Dictionary<Type, string> maps = new Dictionary<Type, string>()
        {
            {  typeof( string ), "string" },
            {  typeof( char ), "char" },
            {  typeof( byte ), "byte" },
            {  typeof( bool ), "bool" },
            {  typeof( short ), "short" },
            {  typeof( int ), "int" },
            {  typeof( long ), "long" },
            {  typeof( float ), "float" },
            {  typeof( double ), "double" },
            {  typeof( decimal ), "decimal" },
            {  typeof( DateTime ), "DateTime" },
        };


        private static string ToTypeName( Type type )
        {
            #region Validations

            if ( type == null )
                throw new ArgumentNullException( nameof( type ) );

            #endregion

            if ( type.IsArray == true )
                return ToTypeName( type.GetElementType() ) + "[]";

            if ( type.IsNullable() == true )
                return ToTypeName( type.GetGenericArguments()[ 0 ] ) + "?";


            string name = type.ToString();

            if ( maps.ContainsKey( type ) == true )
                return maps[ type ];

            if ( name.StartsWith( "System." ) == true )
                return name;

            name = name.Substring( name.LastIndexOf( '.' ) + 1 );

            return name;
        }
    }
}

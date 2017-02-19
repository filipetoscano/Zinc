using Platinum.VisualStudio;
using System;
using System.IO;
using System.Xml;

namespace Zinc.VisualStudio
{
    public class ZnTool : BaseTool
    {
        /// <summary>
        /// Executes tool.
        /// </summary>
        protected override string Execute( ToolGenerateArgs args )
        {
            /*
             * #1. Apply XSLT
             */
            IGenerator xsltTool = new XsltTool();
            string outputContent = xsltTool.Generate( args );


            /*
             * #2. Check if need to generate Impl.cs
             */
            XmlNamespaceManager manager = new XmlNamespaceManager( new NameTable() );
            manager.AddNamespace( "zn", "urn:zinc" );

            XmlDocument doc = new XmlDocument();
            doc.LoadXml( args.Content );

            if ( doc.DocumentElement.NamespaceURI == "urn:zinc"
                 && doc.DocumentElement.LocalName == "method" )
            {
                string impl = Path.GetFileNameWithoutExtension( args.FileName ) + "Impl.cs";

                if ( File.Exists( impl ) == false )
                {
                    XmlElement notImpl = (XmlElement) doc.SelectSingleNode( " /zn:method/zn:notImplemented " );

                    if ( notImpl == null )
                    {
                        GenerateAddImpl( args );
                    }
                }
            }


            /*
             * 
             */
            return outputContent;
        }


        private void GenerateAddImpl( ToolGenerateArgs args )
        {
            /*

    <Compile Include="OneService\MethodOne.cs">
      <AutoGen>True</AutoGen>
      <DesignTime>True</DesignTime>
      <DependentUpon>MethodOne.xml</DependentUpon>
    </Compile>
    <Compile Include="OneService\MethodOneImpl.cs" />

             */


            /*
             * 
             */
            FileInfo file = new FileInfo( args.FileName );

            FileInfo[] projs = file.Directory.Parent.GetFiles( "*.csproj" );

            if ( projs == null || projs.Length == 0 )
                throw new ToolException( "No .csproj found in parent directory from zn:method." );

            if ( projs.Length != 1 )
                throw new ToolException( "More than one .csproj found in parent directory from zn:method." );

            string csproj = projs[ 0 ].FullName;
            Console.WriteLine( csproj );


            /*
             * 
             */
            string find = file.DirectoryName + "\\" + Path.GetFileNameWithoutExtension( file.Name ) + ".cs";
            string addf = file.DirectoryName + "\\" + Path.GetFileNameWithoutExtension( file.Name ) + "Impl.cs";

            Console.WriteLine( find );
            Console.WriteLine( addf );
        }
    }
}

﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:fn="urn:eo-util"
    exclude-result-prefixes="msxsl fn">

    <xsl:output method="text" indent="no" />

    <xsl:param name="Namespace" />
    <xsl:param name="Async" />
    <xsl:param name="Sync" />

    <xsl:variable name="new-line">
        <xsl:text>
</xsl:text>
    </xsl:variable>


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ application = service
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" module ">
        <xsl:text>// autogenerated: do NOT edit manually / do NOT commit to source control
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Zinc.WebServices.RestClient;
using Fwk = Zinc.WebServices.RestClient;

namespace </xsl:text>
        <xsl:value-of select=" $Namespace " />
        <xsl:text>
{
</xsl:text>
        <xsl:apply-templates select=" . " mode="client" />

        <xsl:apply-templates select=" service " mode="client">
            <xsl:sort select=" @name " />
        </xsl:apply-templates>

        <xsl:apply-templates select=" service " mode="contract">
            <xsl:sort select=" @name " />
        </xsl:apply-templates>

        <xsl:if test=" types/* ">
            <xsl:text>

</xsl:text>
        </xsl:if>

        <xsl:apply-templates select=" types/* " mode="type">
            <xsl:sort select=" @name " />
        </xsl:apply-templates>

        <xsl:text>}</xsl:text>
    </xsl:template>


    <xsl:template match=" module " mode="client">
        <xsl:text>    /// &lt;summary /&gt;</xsl:text>
        <xsl:value-of select=" $new-line " />

        <xsl:text>    public class </xsl:text>
        <xsl:value-of select=" @name " />
        <xsl:text>SvcClient : ServiceClient</xsl:text>
        <xsl:value-of select=" $new-line " />

        <xsl:text>    {</xsl:text>
        <xsl:value-of select=" $new-line " />

        <xsl:text>        /// &lt;summary /&gt;</xsl:text>
        <xsl:value-of select=" $new-line " />

        <xsl:text>        public </xsl:text>
        <xsl:value-of select=" @name " />
        <xsl:text>SvcClient() : base( "</xsl:text>
        <xsl:value-of select=" @name " />
        <xsl:text>" )</xsl:text>
        <xsl:value-of select=" $new-line " />

        <xsl:text>        {</xsl:text>
        <xsl:value-of select=" $new-line " />

        <xsl:text>        }</xsl:text>
        <xsl:value-of select=" $new-line " />

        <xsl:text>
        /// &lt;summary /&gt;
        public Task&lt;Fwk.PingResponse&gt; PingAsync()
        {
            return InvokeAsync&lt;Fwk.PingRequest, Fwk.PingResponse&gt;( "##ping", new Fwk.PingRequest(), CancellationToken.None );
        }
    }


</xsl:text>
    </xsl:template>


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ service
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" service " mode="client">
        <xsl:text>    /// &lt;summary /&gt;</xsl:text>
        <xsl:value-of select=" $new-line " />
        <xsl:text>    public partial class </xsl:text>
        <xsl:value-of select=" @name " />
        <xsl:text>Client : ServiceClient
    {
        /// &lt;summary /&gt;
        public </xsl:text>
        <xsl:value-of select=" @name " />
        <xsl:text>Client() : base( "</xsl:text>
        <xsl:value-of select=" ../@name " />
        <xsl:text>" )
        {
        }

</xsl:text>
        <xsl:apply-templates select=" method " mode="client">
            <xsl:sort select=" @name " />
        </xsl:apply-templates>
        <xsl:text>    }
</xsl:text>

        <xsl:if test=" position() != last() ">
            <xsl:text>

</xsl:text>
        </xsl:if>
    </xsl:template>

    <xsl:template match=" service " mode="contract">
        <xsl:text>

    namespace </xsl:text>
        <xsl:value-of select=" @name " />
        <xsl:text>Svc
    {
</xsl:text>
        <xsl:apply-templates select=" method " mode="contract" />
        <xsl:text>    }
</xsl:text>
    </xsl:template>


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ method
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" method " mode="client">
        <xsl:variable name="messageName">
            <xsl:value-of select=" ../@name " />
            <xsl:text>Svc.</xsl:text>
            <xsl:value-of select=" @name " />
        </xsl:variable>


        <xsl:if test=" $Sync ">
            <xsl:call-template name="summary" />
            <xsl:text>        public </xsl:text>
            <xsl:value-of select=" $messageName "/>
            <xsl:text>Response </xsl:text>
            <xsl:value-of select=" @name "/>
            <xsl:text>( </xsl:text>
            <xsl:value-of select=" $messageName "/>
            <xsl:text>Request request )
        {
            return Invoke&lt;</xsl:text>
            <xsl:value-of select=" $messageName "/>
            <xsl:text>Request,</xsl:text>
            <xsl:value-of select=" $messageName "/>
            <xsl:text>Response&gt;( "</xsl:text>
            <xsl:value-of select=" @name " />
            <xsl:text>", request );
        }
</xsl:text>
        </xsl:if>

        <xsl:if test=" $Sync and $Async ">
            <xsl:text>
</xsl:text>
        </xsl:if>

        <xsl:if test=" $Async">
            <xsl:call-template name="summary" />
            <xsl:text>        public Task&lt;</xsl:text>
            <xsl:value-of select=" $messageName "/>
            <xsl:text>Response&gt; </xsl:text>
            <xsl:value-of select=" @name "/>
            <xsl:text>Async( </xsl:text>
            <xsl:value-of select=" $messageName "/>
            <xsl:text>Request request )
        {
            return </xsl:text>
            <xsl:value-of select=" @name " />
            <xsl:text>Async( request, CancellationToken.None );
        }

</xsl:text>

            <xsl:call-template name="summary" />
            <xsl:text>        public Task&lt;</xsl:text>
            <xsl:value-of select=" $messageName "/>
            <xsl:text>Response&gt; </xsl:text>
            <xsl:value-of select=" @name "/>
            <xsl:text>Async( </xsl:text>
            <xsl:value-of select=" $messageName "/>
            <xsl:text>Request request, CancellationToken cancellationToken )
        {
            return InvokeAsync&lt;</xsl:text>
            <xsl:value-of select=" $messageName "/>
            <xsl:text>Request, </xsl:text>
            <xsl:value-of select=" $messageName "/>
            <xsl:text>Response&gt;( "</xsl:text>
            <xsl:value-of select=" @name "/>
            <xsl:text>", request, cancellationToken );
        }
</xsl:text>
        </xsl:if>

        <xsl:if test=" ( $Sync or $Async ) and ( position() != last() ) ">
            <xsl:text>
</xsl:text>
        </xsl:if>
    </xsl:template>


    <xsl:template match=" method " mode="contract">
        <xsl:variable name="messageName">
            <xsl:value-of select=" @name " />
        </xsl:variable>

        <xsl:text>        /// &lt;summary /&gt;
        public partial class </xsl:text>
        <xsl:value-of select=" $messageName " />
        <xsl:text>Request
        {
</xsl:text>
        <xsl:apply-templates select=" request/p " mode="prop" />
        <xsl:text>        }

        /// &lt;summary /&gt;
        public partial class </xsl:text>
        <xsl:value-of select=" $messageName " />
        <xsl:text>Response
        {
</xsl:text>
        <xsl:apply-templates select=" response/p " mode="prop" />
        <xsl:text>        }
</xsl:text>

        <xsl:if test=" position() != last() ">
            <xsl:text>
</xsl:text>
        </xsl:if>
    </xsl:template>


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ types
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" type " mode="type">
        <xsl:text>    /// &lt;summary /&gt;</xsl:text>
        <xsl:text>
</xsl:text>

        <xsl:call-template name="xml-namespace" />
        <xsl:text>    public partial class </xsl:text>
        <xsl:value-of select=" @name " />
        <xsl:text>
    {
</xsl:text>
        <xsl:apply-templates select=" p " mode="prop">
            <xsl:with-param name="indent" select=" '        ' " />
        </xsl:apply-templates>
        <xsl:text>    }
</xsl:text>

        <xsl:if test=" position() != last() ">
            <xsl:text>

</xsl:text>
        </xsl:if>
    </xsl:template>


    <xsl:template match=" enumeration | enumerate " mode="type">
        <xsl:text>    /// &lt;summary /&gt;</xsl:text>
        <xsl:text>
</xsl:text>

        <xsl:call-template name="xml-namespace" />
        <xsl:text>    public enum </xsl:text>
        <xsl:value-of select=" @name " />
        <xsl:text>
    {
</xsl:text>
        <xsl:for-each select=" enum ">
            <xsl:text>        /// &lt;summary /&gt;</xsl:text>
            <xsl:text>
</xsl:text>

            <xsl:text>        </xsl:text>
            <xsl:value-of select=" @value "/>
            <xsl:text>,</xsl:text>
            <xsl:text>
</xsl:text>
        </xsl:for-each>
        <xsl:text>    }
</xsl:text>

        <xsl:if test=" position() != last() ">
            <xsl:text>

</xsl:text>
        </xsl:if>
    </xsl:template>


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ properties
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" p " mode="prop">
        <xsl:param name="indent" select=" '            ' " />

        <xsl:call-template name="summary">
            <xsl:with-param name="indent" select=" $indent " />
        </xsl:call-template>

        <!-- Specialized types -->
        <xsl:if test=" @spec ">
            <xsl:choose>
                <!-- date[ @optional='true' ] -->
                <xsl:when test=" @spec = 'date' and @type = 'DateTime?' ">
                    <xsl:value-of select=" $indent " />
                    <xsl:text>[JsonConverter( typeof( Zinc.Json.NullableDateConverter ) )]</xsl:text>
                    <xsl:value-of select=" $new-line " />

                    <xsl:value-of select=" $indent " />
                    <xsl:text>[XmlElement( DataType = "date" )]</xsl:text>
                    <xsl:value-of select=" $new-line " />
                </xsl:when>

                <!-- date -->
                <xsl:when test=" @spec = 'date' ">
                    <xsl:value-of select=" $indent " />
                    <xsl:text>[JsonConverter( typeof( Zinc.Json.DateConverter ) )]</xsl:text>
                    <xsl:value-of select=" $new-line " />

                    <xsl:value-of select=" $indent " />
                    <xsl:text>[XmlElement( DataType = "date" )]</xsl:text>
                    <xsl:value-of select=" $new-line " />
                </xsl:when>

                <!-- time[ @optional='true' ] -->
                <xsl:when test=" @spec = 'time' and @type = 'DateTime?' ">
                    <xsl:value-of select=" $indent " />
                    <xsl:text>[JsonConverter( typeof( Zinc.Json.NullableTimeConverter ) )]</xsl:text>
                    <xsl:value-of select=" $new-line " />

                    <xsl:value-of select=" $indent " />
                    <xsl:text>[XmlElement( DataType = "time" )]</xsl:text>
                    <xsl:value-of select=" $new-line " />
                </xsl:when>

                <!-- time -->
                <xsl:when test=" @spec = 'time' ">
                    <xsl:value-of select=" $indent " />
                    <xsl:text>[JsonConverter( typeof( Zinc.Json.TimeConverter ) )]</xsl:text>
                    <xsl:value-of select=" $new-line " />

                    <xsl:value-of select=" $indent " />
                    <xsl:text>[XmlElement( DataType = "time" )]</xsl:text>
                    <xsl:value-of select=" $new-line " />
                </xsl:when>
            </xsl:choose>
        </xsl:if>


        <!-- Custom types -->
        <xsl:if test=" @type = 'Platinum.Duration?' ">
            <xsl:value-of select=" $indent " />
            <xsl:text>[JsonConverter( typeof( Zinc.Json.NullableDurationConverter ) )]</xsl:text>
            <xsl:value-of select=" $new-line " />
        </xsl:if>

        <xsl:if test=" @type = 'Platinum.Duration' ">
            <xsl:value-of select=" $indent " />
            <xsl:text>[JsonConverter( typeof( Zinc.Json.DurationConverter ) )]</xsl:text>
            <xsl:value-of select=" $new-line " />
        </xsl:if>


        <xsl:value-of select=" $indent "/>
        <xsl:text>public </xsl:text>
        <xsl:value-of select=" @type " />
        <xsl:text> </xsl:text>
        <xsl:value-of select=" @name " />
        <xsl:text> { get; set; }
</xsl:text>

        <xsl:if test=" position() != last() ">
            <xsl:text>
</xsl:text>
        </xsl:if>
    </xsl:template>


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ summary
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template name="summary">
        <xsl:param name="indent" select=" '        ' " />

        <xsl:choose>
            <xsl:when test=" summary ">
                <xsl:value-of select=" $indent "/>
                <xsl:text>/// &lt;summary&gt;</xsl:text>
                <!-- This new line not needed -->

                <xsl:value-of select=" fn:ToWrap( summary, concat( $indent, '/// ' ), 80 ) " />
                <xsl:value-of select=" $new-line " />

                <xsl:value-of select=" $indent "/>
                <xsl:text>/// &lt;/summary&gt;</xsl:text>
                <xsl:value-of select=" $new-line " />
            </xsl:when>

            <xsl:otherwise>
                <xsl:value-of select=" $indent "/>
                <xsl:text>/// &lt;summary /&gt;</xsl:text>
                <xsl:value-of select=" $new-line " />
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ xml-namespace
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template name="xml-namespace">
        <xsl:param name="indent" select=" '    ' " />

        <xsl:if test=" @ns ">
            <xsl:value-of select=" $indent " />
            <xsl:text>[XmlType( Namespace = "</xsl:text>
            <xsl:value-of select=" @ns " />
            <xsl:text>" )]</xsl:text>
            <xsl:value-of select=" $new-line" />
        </xsl:if>
    </xsl:template>

</xsl:stylesheet>

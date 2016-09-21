<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"
    exclude-result-prefixes="msxsl soap">

    <xsl:output method="text" />

    <xsl:template match=" soap:Envelope ">
        <xsl:apply-templates select=" soap:Body/*[ 1 ] " mode="json-obj" />
    </xsl:template>


    <xsl:template match=" * " mode="json-obj">
        <xsl:text>{</xsl:text>
        <xsl:apply-templates select=" * " mode="json-prop" />
        <xsl:text>}</xsl:text>

        <xsl:if test=" position() != last() ">
            <xsl:text>,</xsl:text>
        </xsl:if>
    </xsl:template>


    <xsl:template match=" * " mode="json-prop">
        <xsl:choose>
            <xsl:when test=" count( * ) = 0 ">
                <xsl:text>"</xsl:text>
                <xsl:value-of select=" local-name(.) " />
                <xsl:text>":"</xsl:text>
                <xsl:value-of select=" text() " />
                <xsl:text>"</xsl:text>
            </xsl:when>

            <xsl:when test=" count( * ) > 1 and local-name( *[1] ) = local-name( *[2] ) ">
                <xsl:text>"</xsl:text>
                <xsl:value-of select=" local-name(.) " />
                <xsl:text>":[</xsl:text>
                <xsl:apply-templates select=" * " mode="json-obj" />
                <xsl:text>]</xsl:text>
            </xsl:when>
            
            <xsl:when test=" count( * ) > 0 ">
                <xsl:text>"</xsl:text>
                <xsl:value-of select=" local-name(.) " />
                <xsl:text>":{</xsl:text>
                <xsl:apply-templates select=" * " mode="json-prop" />
                <xsl:text>}</xsl:text>
            </xsl:when>

            <xsl:otherwise>
            </xsl:otherwise>
        </xsl:choose>

        <xsl:if test=" position() != last() ">
            <xsl:text>,</xsl:text>
        </xsl:if>
    </xsl:template>

</xsl:stylesheet>

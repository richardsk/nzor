<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema">
	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>
	<xsl:template match="/">
		<Dictionary xmlns:xs="http://www.w3.org/2001/XMLSchema">
			<xsl:for-each select="//xs:element | //xs:attribute">
				<xsl:choose>
					<xsl:when test="name()='xs:element'">
						<Field>
							<Name><xsl:value-of select="@name"/></Name>
							<Type><xsl:value-of select="@type"/></Type>
							<Path>	<xsl:value-of select="string-join(ancestor-or-self::xs:element/@name, '/')"/></Path>
							<Annotation><xsl:value-of select="xs:annotation/xs:documenation"/>	</Annotation>
						</Field>
					</xsl:when>
					<xsl:otherwise>
						<Field>
							<Name>@<xsl:value-of select="@name"/></Name>
							<Type><xsl:value-of select="@type"/></Type>
							<Path><xsl:value-of select="string-join(ancestor-or-self::xs:element/@name, '/')"/>/@<xsl:value-of select="@name"/></Path>
							<Annotation><xsl:value-of select="xs:annotation/xs:documenation"/></Annotation>
					</Field>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:for-each>
		</Dictionary>
	</xsl:template>
</xsl:stylesheet>

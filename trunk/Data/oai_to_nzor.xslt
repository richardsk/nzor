<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"   xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	 xmlns:nzor="http://www.nzor.org.nz/schema/provider/1.0">
<xsl:output method="xml" encoding="utf-8"/>
	<xsl:template match="/">
		<DataSet>			
			<xsl:if test="count(//nzor:Metadata) &gt; 0">
				<Metadata>
					<xsl:variable name="md" select="//nzor:Metadata"/>
					<xsl:copy>
						<xsl:apply-templates select="$md[1]/*" />
					</xsl:copy>
				</Metadata>				
			</xsl:if>
			
			<xsl:if test="count(//nzor:Publication) &gt; 0">
				<Publications>
					<xsl:copy>
						<xsl:apply-templates select="//nzor:Publication" />												
					</xsl:copy>
				</Publications>				
			</xsl:if>
			
			<xsl:if test="count(//nzor:TaxonNames) &gt; 0">
				<TaxonNames>
					<xsl:copy>
						<xsl:apply-templates select="//nzor:TaxonName" />												
					</xsl:copy>
					<xsl:copy>
						<xsl:apply-templates select="//nzor:VernacularName" />												
					</xsl:copy>
				</TaxonNames>				
			</xsl:if>	
			
			<xsl:if test="count(//nzor:TaxonConcepts) &gt; 0">
				<TaxonConcepts>
					<xsl:copy>
						<xsl:apply-templates select="//nzor:TaxonConcept" />												
					</xsl:copy>
				</TaxonConcepts>				
			</xsl:if>		
			
			<xsl:if test="count(//nzor:TaxonProperties) &gt; 0">
				<TaxonProperties>
					<BiostatusValues>
						<xsl:copy>
							<xsl:apply-templates select="//nzor:Biostatus" />												
						</xsl:copy>
					</BiostatusValues>
					<ManagementStatusValues>
						<xsl:copy>
							<xsl:apply-templates select="//nzor:ManagementStatus" />												
						</xsl:copy>
					</ManagementStatusValues>
				</TaxonProperties>				
			</xsl:if>		
			
		</DataSet>
	</xsl:template>
	
	<xsl:template match="//nzor:Publication">	
		<Publication>
			<xsl:apply-templates select="*" />						
		</Publication>
	</xsl:template>
	
	<xsl:template match="//nzor:TaxonName">	
		<TaxonName>
			<xsl:apply-templates select="*" />						
		</TaxonName>
	</xsl:template>
	
	<xsl:template match="//nzor:VernacularName">	
		<VernacularName>
			<xsl:apply-templates select="*" />						
		</VernacularName>
	</xsl:template>
	
	<xsl:template match="//nzor:TaxonConcept">	
		<TaxonConcept>
			<xsl:apply-templates select="*" />						
		</TaxonConcept>
	</xsl:template>
	
	<xsl:template match="//nzor:Biostatus">	
		<Biostatus>
			<xsl:apply-templates select="*" />						
		</Biostatus>
	</xsl:template>
	
	<xsl:template match="//nzor:ManagementStatus">	
		<ManagementStatus>
			<xsl:apply-templates select="*" />						
		</ManagementStatus>
	</xsl:template>
	
	
	<xsl:template match="*">
			<xsl:element name="{local-name()}">
				<xsl:apply-templates select="@* | node()" />
			</xsl:element>		
	</xsl:template>
	
	<xsl:template match="@*">
		<xsl:attribute name="{local-name()}"><xsl:value-of select="."/></xsl:attribute>
	</xsl:template>
	
	<xsl:template match="text() | processing-instruction() | comment()">
		<xsl:copy />
	</xsl:template>

	<xsl:template match="nzor:TaxonName">		
			<xsl:value-of select="."/>
	</xsl:template>
	
</xsl:stylesheet>

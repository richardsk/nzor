<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"  
	 xmlns:nzor="http://www.nzor.org.nz/schema/provider/103" exclude-result-prefixes="nzor">
<xsl:output method="xml" encoding="utf-8"/>
	<xsl:template match="/">
		<DataSet>			
			<xsl:if test="count(//nzor:Metadata) &gt; 0">
				<Metadata>
					<xsl:variable name="md" select="//nzor:Metadata"/>
					<xsl:copy>
						<xsl:apply-templates select="$md[1]/*"/>
					</xsl:copy>
				</Metadata>				
			</xsl:if>
			
			<DataSource>
				<xsl:attribute name="id"><xsl:value-of select="//nzor:DataSource[1]/@id"/></xsl:attribute>
							
				<xsl:if test="count(//nzor:Publications/nzor:Publication) &gt; 0">
					<Publications>
						<xsl:copy>
							<xsl:apply-templates select="//nzor:Publications/nzor:Publication" />												
						</xsl:copy>
					</Publications>				
				</xsl:if>
				
				<xsl:if test="count(//nzor:TaxonNames) &gt; 0">
					<TaxonNames>
						<xsl:copy>
							<xsl:apply-templates select="//nzor:TaxonNames/nzor:TaxonName" />												
						</xsl:copy>
						<xsl:copy>
							<xsl:apply-templates select="//nzor:TaxonNames/nzor:VernacularName" />												
						</xsl:copy>
					</TaxonNames>				
				</xsl:if>	
				
				<xsl:if test="count(//nzor:TaxonConcepts) &gt; 0">
					<TaxonConcepts>
						<xsl:copy>
							<xsl:apply-templates select="//nzor:TaxonConcepts/nzor:TaxonConcept" />			
							<xsl:apply-templates select="//nzor:TaxonConcepts/nzor:VernacularConcept" />			
							<xsl:apply-templates select="//nzor:TaxonConcepts/nzor:NameBasedConcept" />									
						</xsl:copy>
					</TaxonConcepts>				
				</xsl:if>	
								
				<xsl:if test="count(//nzor:TaxonProperties) &gt; 0">
					<TaxonProperties>
						<xsl:if test="count(//nzor:TaxonProperties/nzor:BiostatusValues) &gt; 0">
						<BiostatusValues>
							<xsl:copy>
								<xsl:apply-templates select="//nzor:BiostatusValues/nzor:Biostatus" />												
							</xsl:copy>
						</BiostatusValues>
						</xsl:if>
						<xsl:if test="count(//nzor:TaxonProperties/nzor:ManagementStatusValues) &gt; 0">
						<ManagementStatusValues>
							<xsl:copy>
								<xsl:apply-templates select="//nzor:ManagementStatusValues/nzor:ManagementStatus" />												
							</xsl:copy>
						</ManagementStatusValues>
						</xsl:if>
					</TaxonProperties>				
				</xsl:if>		

		</DataSource>
			
		</DataSet>
	</xsl:template>
	
	<xsl:template match="//nzor:Publication">	
		<Publication>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates select="*" />						
		</Publication>
	</xsl:template>
	
	<xsl:template match="//nzor:TaxonName">	
		<TaxonName>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates select="*" />						
		</TaxonName>
	</xsl:template>
	
	<xsl:template match="//nzor:VernacularName">	
		<VernacularName>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates select="*" />						
		</VernacularName>
	</xsl:template>
	
	<xsl:template match="//nzor:TaxonConcept">	
		<TaxonConcept>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates select="*" />						
		</TaxonConcept>
	</xsl:template>
	
	<xsl:template match="//nzor:VernacularConcept">	
		<VernacularConcept>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates select="*" />						
		</VernacularConcept>
	</xsl:template>
	
	<xsl:template match="//nzor:NameBasedConcept">	
		<NameBasedConcept>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates select="*" />						
		</NameBasedConcept>
	</xsl:template>
	
	<xsl:template match="//nzor:Biostatus">	
		<Biostatus>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates select="*" />						
		</Biostatus>
	</xsl:template>
	
	<xsl:template match="//nzor:ManagementStatus">	
		<ManagementStatus>
			<xsl:copy-of select="@*"/>
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

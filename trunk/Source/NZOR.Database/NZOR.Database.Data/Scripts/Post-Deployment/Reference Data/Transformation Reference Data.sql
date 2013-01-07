
delete dbo.Transformation
go

insert dbo.Transformation
values (NEWID(), 'NameText_FullName', 'Full name generation XSLT', 'NameText', 
				'<?xml version="1.0" encoding="UTF-8"?>
				<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
				  <xsl:output method="text" version="1.0" encoding="UTF-8" indent="yes" />
				  <xsl:variable name="whitespace">&#x20;&#x9;&#xD;&#xA;&#xA0;</xsl:variable>
				  <xsl:template match="/">
					<xsl:call-template name="trim-start">
					  <xsl:with-param name="text">
						<xsl:apply-templates />
					  </xsl:with-param>
					</xsl:call-template>
				  </xsl:template>
				  <xsl:template match="Name | Rank | Authors | Year">
					<xsl:choose>
						<xsl:when test="local-name()=''Year''">, <xsl:value-of select="." /></xsl:when>
						<xsl:otherwise>&#160;<xsl:value-of select="." /></xsl:otherwise>
					</xsl:choose>
				  </xsl:template>
				  <xsl:template name="trim-start">
					<xsl:param name="text" />
					<xsl:variable name="first-char" select="substring($text, 1, 1)" />
					<xsl:choose>
					  <xsl:when test="contains($whitespace, $first-char)">
						<xsl:call-template name="trim-start">
						  <xsl:with-param name="text" select="substring($text, 2, string-length($text) -1)" />
						</xsl:call-template>
					  </xsl:when>
					  <xsl:otherwise>
						<xsl:value-of select="$text" />
					  </xsl:otherwise>
					</xsl:choose>
				  </xsl:template>
				</xsl:stylesheet>'),
	(NEWID(), 'NameText_FullNameFormatted', 'Formatted full name (including HTML tags) generation XSLT', 'NameText', '<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
		  <xsl:output method="text" version="1.0" encoding="UTF-8" indent="yes" />
		  <xsl:variable name="whitespace"> 	
 		 </xsl:variable>
		  <xsl:template match="/">
			<xsl:call-template name="trim-start">
			  <xsl:with-param name="text">
				<xsl:apply-templates />
			  </xsl:with-param>
			</xsl:call-template>
		  </xsl:template>
		  <xsl:template match="Name">
			<xsl:choose>
			  <xsl:when test="name(preceding-sibling::*[1]) !=''Name''">
				<xsl:text disable-output-escaping="yes">&lt;i&gt;</xsl:text>
			  </xsl:when>
			  <xsl:otherwise xml:space="preserve"> </xsl:otherwise>
			</xsl:choose>
			<xsl:value-of select="." />
			<xsl:if test="name(following-sibling::*[1]) !=''Name''">
			  <xsl:text disable-output-escaping="yes">&lt;/i&gt;</xsl:text>
			</xsl:if>
		  </xsl:template>
		  <xsl:template match="Rank | Authors | Year | Status">
			<xsl:choose>
			  <xsl:when test="local-name()=''Year''">, <xsl:value-of select="." /></xsl:when>
			  <xsl:otherwise> <xsl:value-of select="." /></xsl:otherwise>
			</xsl:choose>
		  </xsl:template>
		  <xsl:template name="trim-start">
			<xsl:param name="text" />
			<xsl:variable name="first-char" select="substring($text, 1, 1)" />
			<xsl:choose>
			  <xsl:when test="contains($whitespace, $first-char)">
				<xsl:call-template name="trim-start">
				  <xsl:with-param name="text" select="substring($text, 2, string-length($text) -1)" />
				</xsl:call-template>
			  </xsl:when>
			  <xsl:otherwise>
				<xsl:value-of select="$text" />
			  </xsl:otherwise>
			</xsl:choose>
		  </xsl:template>
		</xsl:stylesheet>'),
	(NEWID(), 'NameText_PartialName', 'Partial name (no authors) generation XSLT', 'NameText', '<?xml version="1.0" encoding="UTF-8"?>
				<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
                <xsl:output method="text" version="1.0" encoding="UTF-8" indent="yes"/>
                <xsl:variable name="whitespace">&#x20;&#x9;&#xD;&#xA;&#xA0;</xsl:variable> 

                <xsl:template match="/">

                  <xsl:call-template name="trim-end"> 
					<xsl:with-param name="text">
						<xsl:apply-templates/>
					</xsl:with-param>
				  </xsl:call-template> 

                </xsl:template>
                <xsl:template match="Name | Rank"><xsl:value-of select="."/>&#xA0;</xsl:template>
				<xsl:template match="Authors | Year | Status"></xsl:template>

                <xsl:template name="trim-end"> 
				   <xsl:param name="text"/> 
				   <xsl:variable name="last-char" select="substring($text, string-length($text), 1)" /> 
				   <xsl:choose> 
					 <xsl:when test="contains($whitespace, $last-char)"> 
					   <xsl:call-template name="trim-end"> 
						 <xsl:with-param name="text" select="substring($text, 1, string-length($text) - 1)" />
					   </xsl:call-template> 
					 </xsl:when> 
					 <xsl:otherwise> 
					   <xsl:value-of select="$text" /> 
					 </xsl:otherwise> 
				   </xsl:choose> 
				</xsl:template>
				</xsl:stylesheet>'),
	(NEWID(), 'NameText_PartialNameFormatted', 'Formatted partial name (no authors, but includes HTML tags) generation XSLT', 'NameText', '<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
		  <xsl:output method="text" version="1.0" encoding="UTF-8" indent="yes" />
		  <xsl:variable name="whitespace"> 	
 		 </xsl:variable>
		  <xsl:template match="/">
			<xsl:call-template name="trim-end">
			  <xsl:with-param name="text">
				<xsl:apply-templates />
			  </xsl:with-param>
			</xsl:call-template>
		  </xsl:template>
		  <xsl:template match="Name">
			<xsl:choose>
			  <xsl:when test="name(preceding-sibling::*[1]) !=''Name''">
				<xsl:text disable-output-escaping="yes">&lt;i&gt;</xsl:text>
			  </xsl:when>
			  <xsl:otherwise xml:space="preserve"> </xsl:otherwise>
			</xsl:choose>
			<xsl:value-of select="." />
			<xsl:if test="name(following-sibling::*[1]) !=''Name''">
			  <xsl:text disable-output-escaping="yes">&lt;/i&gt;</xsl:text>
			</xsl:if> </xsl:template>
		  <xsl:template match="Rank">
			<xsl:value-of select="." /> </xsl:template>
		  <xsl:template match="Authors | Year | Status" />
		  <xsl:template name="trim-end">
			<xsl:param name="text" />
			<xsl:variable name="last-char" select="substring($text, string-length($text), 1)" />
			<xsl:choose>
			  <xsl:when test="contains($whitespace, $last-char)">
				<xsl:call-template name="trim-end">
				  <xsl:with-param name="text" select="substring($text, 1, string-length($text) - 1)" />
				</xsl:call-template>
			  </xsl:when>
			  <xsl:otherwise>
				<xsl:value-of select="$text" />
			  </xsl:otherwise>
			</xsl:choose>
		  </xsl:template>
		</xsl:stylesheet>')

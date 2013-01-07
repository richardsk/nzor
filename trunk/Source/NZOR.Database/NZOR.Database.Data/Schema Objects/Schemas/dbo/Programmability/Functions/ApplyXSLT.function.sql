create function dbo.ApplyXSLT(@xml nvarchar(max), @xslt nvarchar(max), @AddRootNode bit) returns nvarchar(max)
external name TextUtility.[LCR.Common.TextUtility.TextUtility].ApplyXSLT
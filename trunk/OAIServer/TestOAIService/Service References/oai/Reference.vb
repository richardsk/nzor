﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:2.0.50727.3603
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace oai
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0"),  _
     System.ServiceModel.ServiceContractAttribute(ConfigurationName:="oai.IOAIPMHService")>  _
    Public Interface IOAIPMHService
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IOAIPMHService/Identify", ReplyAction:="http://tempuri.org/IOAIPMHService/IdentifyResponse")>  _
        Function Identify(ByVal repository As String) As System.Xml.Linq.XElement
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IOAIPMHService/GetRecord", ReplyAction:="http://tempuri.org/IOAIPMHService/GetRecordResponse")>  _
        Function GetRecord(ByVal repository As String, ByVal id As String, ByVal mdPrefix As String) As System.Xml.Linq.XElement
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IOAIPMHService/ListIdentifiers", ReplyAction:="http://tempuri.org/IOAIPMHService/ListIdentifiersResponse")>  _
        Function ListIdentifiers() As System.Xml.Linq.XElement
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IOAIPMHService/ListMetadataFormats", ReplyAction:="http://tempuri.org/IOAIPMHService/ListMetadataFormatsResponse")>  _
        Function ListMetadataFormats() As System.Xml.Linq.XElement
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IOAIPMHService/ListRecords", ReplyAction:="http://tempuri.org/IOAIPMHService/ListRecordsResponse")>  _
        Function ListRecords(ByVal repository As String, ByVal fromDate As String, ByVal toDate As String, ByVal [set] As String, ByVal resumptionToken As String, ByVal mdPrefix As String) As System.Xml.Linq.XElement
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IOAIPMHService/ListSets", ReplyAction:="http://tempuri.org/IOAIPMHService/ListSetsResponse")>  _
        Function ListSets() As System.Xml.Linq.XElement
    End Interface
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")>  _
    Public Interface IOAIPMHServiceChannel
        Inherits oai.IOAIPMHService, System.ServiceModel.IClientChannel
    End Interface
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")>  _
    Partial Public Class OAIPMHServiceClient
        Inherits System.ServiceModel.ClientBase(Of oai.IOAIPMHService)
        Implements oai.IOAIPMHService
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String)
            MyBase.New(endpointConfigurationName)
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String, ByVal remoteAddress As String)
            MyBase.New(endpointConfigurationName, remoteAddress)
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String, ByVal remoteAddress As System.ServiceModel.EndpointAddress)
            MyBase.New(endpointConfigurationName, remoteAddress)
        End Sub
        
        Public Sub New(ByVal binding As System.ServiceModel.Channels.Binding, ByVal remoteAddress As System.ServiceModel.EndpointAddress)
            MyBase.New(binding, remoteAddress)
        End Sub
        
        Public Function Identify(ByVal repository As String) As System.Xml.Linq.XElement Implements oai.IOAIPMHService.Identify
            Return MyBase.Channel.Identify(repository)
        End Function
        
        Public Function GetRecord(ByVal repository As String, ByVal id As String, ByVal mdPrefix As String) As System.Xml.Linq.XElement Implements oai.IOAIPMHService.GetRecord
            Return MyBase.Channel.GetRecord(repository, id, mdPrefix)
        End Function
        
        Public Function ListIdentifiers() As System.Xml.Linq.XElement Implements oai.IOAIPMHService.ListIdentifiers
            Return MyBase.Channel.ListIdentifiers
        End Function
        
        Public Function ListMetadataFormats() As System.Xml.Linq.XElement Implements oai.IOAIPMHService.ListMetadataFormats
            Return MyBase.Channel.ListMetadataFormats
        End Function
        
        Public Function ListRecords(ByVal repository As String, ByVal fromDate As String, ByVal toDate As String, ByVal [set] As String, ByVal resumptionToken As String, ByVal mdPrefix As String) As System.Xml.Linq.XElement Implements oai.IOAIPMHService.ListRecords
            Return MyBase.Channel.ListRecords(repository, fromDate, toDate, [set], resumptionToken, mdPrefix)
        End Function
        
        Public Function ListSets() As System.Xml.Linq.XElement Implements oai.IOAIPMHService.ListSets
            Return MyBase.Channel.ListSets
        End Function
    End Class
End Namespace
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


Namespace consumer
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0"),  _
     System.ServiceModel.ServiceContractAttribute(ConfigurationName:="consumer.IService")>  _
    Public Interface IService
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IService/GetName", ReplyAction:="http://tempuri.org/IService/GetNameResponse")>  _
        Function GetName(ByVal token As String, ByVal nameId As String) As String
    End Interface
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")>  _
    Public Interface IServiceChannel
        Inherits consumer.IService, System.ServiceModel.IClientChannel
    End Interface
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")>  _
    Partial Public Class ServiceClient
        Inherits System.ServiceModel.ClientBase(Of consumer.IService)
        Implements consumer.IService
        
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
        
        Public Function GetName(ByVal token As String, ByVal nameId As String) As String Implements consumer.IService.GetName
            Return MyBase.Channel.GetName(token, nameId)
        End Function
    End Class
End Namespace

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB

Namespace ReadOnlyDelayedProperty
	Friend Class Program
		Shared Sub Main(ByVal args() As String)
			Dim data() As Byte = { 5, 4, 3, 2, 1 }

			XpoDefault.DataLayer = XpoDefault.GetDataLayer(AutoCreateOption.DatabaseAndSchema)
			'new Session().ClearDatabase();

			Using uof As New UnitOfWork()
				If uof.FindObject(Of MyObject)(Nothing) Is Nothing Then
					Console.WriteLine("Creating a default object...")
					Dim obj As New MyObject(uof)
					obj.Name = "pict"
					obj.DocumentInternal = data
					uof.CommitChanges()
				End If
			End Using

			Using uof As New UnitOfWork()
				Console.WriteLine("Loading MyObject...")
				Dim obj As MyObject = uof.FindObject(Of MyObject)(Nothing)
				Console.WriteLine(obj.Name)
				Dim loaded() As Byte = obj.Document
				Console.WriteLine(loaded.Length)
			End Using

			Console.WriteLine("Press <Enter> to exit.")
			Console.ReadLine()
		End Sub
	End Class

	Public Class MyObject
		Inherits XPObject
		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub

		Private name_Renamed As String
		Public Property Name() As String
			Get
				Return name_Renamed
			End Get
			Set(ByVal value As String)
				If IsLoading Then
					Console.WriteLine("Loading Name")
				End If
				SetPropertyValue("Name", name_Renamed, value)
			End Set
		End Property

		<Delayed, Persistent("Document")> _
		Friend Property DocumentInternal() As Byte()
			Get
				Console.WriteLine("Reading DocumentInternal")
				Return GetDelayedPropertyValue(Of Byte())("DocumentInternal")
			End Get
			Set(ByVal value As Byte())
				SetDelayedPropertyValue("DocumentInternal", value)
			End Set
		End Property

		<PersistentAlias("DocumentInternal")> _
		Public ReadOnly Property Document() As Byte()
			Get
				Return DocumentInternal
			End Get
		End Property

	End Class

End Namespace

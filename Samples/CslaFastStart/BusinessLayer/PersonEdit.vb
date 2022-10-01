Imports System
Imports Csla
Imports DataAccessLayer
Imports System.ComponentModel.DataAnnotations

Namespace BusinessLayer
  <Serializable>
  Public Class PersonEdit
    Inherits BusinessBase(Of PersonEdit)
    Public Shared ReadOnly IdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.Id)
    Public Property Id As Integer
      Get
        Return GetProperty(IdProperty)
      End Get
      Private Set(ByVal value As Integer)
        LoadProperty(IdProperty, value)
      End Set
    End Property

    Public Shared ReadOnly FirstNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.FirstName)
    <Required>
    Public Property FirstName As String
      Get
        Return GetProperty(FirstNameProperty)
      End Get
      Set(ByVal value As String)
        SetProperty(FirstNameProperty, value)
      End Set
    End Property

    Public Shared ReadOnly LastNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.LastName)
    <Required>
    Public Property LastName As String
      Get
        Return GetProperty(LastNameProperty)
      End Get
      Set(ByVal value As String)
        SetProperty(LastNameProperty, value)
      End Set
    End Property

    <Create>
    Private Sub Create()
      Dim dal = New PersonDal()
      Dim dto = dal.Create()
      Using BypassPropertyChecks
        Id = dto.Id
        FirstName = dto.FirstName
        LastName = dto.LastName
      End Using
      BusinessRules.CheckRules()
    End Sub

    <Fetch>
    Private Sub Fetch(ByVal id As Integer)
      Dim dal = New PersonDal()
      Dim dto = dal.GetPerson(id)
      Using BypassPropertyChecks
        Me.Id = dto.Id
        FirstName = dto.FirstName
        LastName = dto.LastName
      End Using
    End Sub

    <Insert>
    Private Sub Insert()
      Dim dal = New PersonDal()
      Using BypassPropertyChecks
        Dim dto = New PersonDto With {
  .FirstName = FirstName,
  .LastName = LastName
}
        Id = dal.InsertPerson(dto)
      End Using
    End Sub

    <Update>
    Private Sub Update()
      Dim dal = New PersonDal()
      Using BypassPropertyChecks
        Dim dto = New PersonDto With {
  .Id = Id,
  .FirstName = FirstName,
  .LastName = LastName
}
        dal.UpdatePerson(dto)
      End Using
    End Sub

    <Delete>
    Private Sub Delete(ByVal id As Integer)
      Dim dal = New PersonDal()
      Using BypassPropertyChecks
        dal.DeletePerson(id)
      End Using
    End Sub

    <DeleteSelf>
    Private Sub DeleteSelf()
      Delete(Id)
    End Sub
  End Class
End Namespace

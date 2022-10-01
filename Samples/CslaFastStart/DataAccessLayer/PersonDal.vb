Imports System
Imports System.Linq
Imports System.Collections.Generic

Namespace DataAccessLayer
  Public Class PersonDal
    ' this is our in-memory database
    Private Shared _list As List(Of PersonDto) = New List(Of PersonDto)()

    Public Function Create() As PersonDto
      Return New PersonDto With {
        .Id = -1
      }
    End Function

    Public Function GetPerson(ByVal id As Integer) As PersonDto
      Dim entity = _list.FirstOrDefault(Function(__) __.Id = id)
      If entity Is Nothing Then Throw New Exception("Index not found")
      Return entity
    End Function

    Public Function InsertPerson(ByVal data As PersonDto) As Integer
      Dim newId = 1
      If _list.Count > 0 Then newId = _list.Max(Function(__) __.Id) + 1
      data.Id = newId
      _list.Add(data)
      Return newId
    End Function

    Public Sub UpdatePerson(ByVal data As PersonDto)
      Dim entity = GetPerson(data.Id)
      entity.FirstName = data.FirstName
      entity.LastName = data.LastName
    End Sub

    Public Sub DeletePerson(ByVal id As Integer)
      Dim entity = GetPerson(id)
      _list.Remove(entity)
    End Sub
  End Class
End Namespace
